using Microsoft.EntityFrameworkCore;
using PolicyAdmin.API.Data;
using PolicyAdmin.API.DTOs;
using PolicyAdmin.API.Models;
using System.Text.RegularExpressions;

namespace PolicyAdmin.API.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly PolicyDbContext _context;

        public PolicyService(PolicyDbContext context)
        {
            _context = context;
        }

        public async Task<List<PolicyDto>> GetAllPoliciesAsync(string? status = null,int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Policies
                .Include(p => p.PolicyCoverages)
                .Include(p => p.PolicyTransactions)
                .Include(p => p.PolicyRenewals)
                .AsQueryable();

            await RefreshPolicyStatusesAsync();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(p =>
                    p.Status.ToLower() == status.ToLower());
            }

            
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1)
                pageSize = 10;

            if (pageSize > 100)
                pageSize = 100;

            var policies = await query
                .OrderByDescending(p => p.PolicyId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return policies.Select(MapToDto).ToList();
        }

        public async Task<PolicyDto?> GetPolicyByNumberAsync(string policyNumber)
        {
            await RefreshPolicyStatusesAsync();

            // LINQ Demonstration: SingleOrDefault with Include
            var policy = await _context.Policies
                .Include(p => p.PolicyCoverages)
                .Include(p => p.PolicyTransactions)
                .Include(p => p.PolicyRenewals)
                .SingleOrDefaultAsync(p => p.PolicyNumber == policyNumber);

            return policy == null ? null : MapToDto(policy);
        }

        public async Task<PolicyDto> CreatePolicyAsync(CreatePolicyDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.QuoteId))
                throw new ArgumentException("Quote ID is required.");

            // Validate quote
            var quote = await _context.Quotes
            .SingleOrDefaultAsync(q => q.QuoteId == dto.QuoteId);

            if (quote == null)
                throw new KeyNotFoundException($"Quote '{dto.QuoteId}' was not found.");

            if (quote.Status != "Approved")
                throw new InvalidOperationException($"Quote '{dto.QuoteId}' is not eligible for policy creation (Status: {quote.Status}).");

            
            
            var endDate = dto.EndDate;

            if (endDate == default(DateTime))
            {
                endDate = dto.StartDate.AddYears(1);
            }

            if (endDate <= dto.StartDate)
                throw new ArgumentException("Policy End Date must be after Policy Start Date.");

           
            var policy = new Policy
            {
                
                QuoteId = quote.QuoteId,
                CustomerId = quote.CustomerId,
                CustomerName = quote.CustomerName,
                CustomerEmail = quote.CustomerEmail,
                CustomerPhone = quote.CustomerPhone,
                PetId = quote.PetId,
                PetName = quote.PetName,
                PetType = quote.PetType,
                Breed = quote.Breed,
                Age = quote.Age,
                StartDate = dto.StartDate,
                EndDate = endDate,
                Premium = dto.AnnualPremium > 0
                            ? dto.AnnualPremium
                            : quote.AnnualPremium,
                PaymentFrequency = dto.PaymentFrequency,
                Status = DetermineStatus(
                            dto.StartDate,
                            endDate,
                            isCancelled: false)
            };

            
            policy.PolicyCoverages.Add(new PolicyCoverage
            {
                CoverageType = dto.CoverageType,
                AnnualLimit = dto.AnnualLimit > 0
                                ? dto.AnnualLimit
                                : quote.AnnualLimit,
                Deductible = dto.Deductible > 0
                                ? dto.Deductible
                                : quote.Deductible,
                ReimbursementPct = dto.ReimbursementPct > 0
                                ? dto.ReimbursementPct
                                : quote.ReimbursementPct
            });

            
            policy.PolicyTransactions.Add(new PolicyTransaction
            {
                TransactionType = "Created",
                EffectiveDate = DateTime.UtcNow,
                Notes = $"Policy created from approved quote {quote.QuoteId}"
            });

            
            quote.Status = "Converted";

            _context.Policies.Add(policy);

            await _context.SaveChangesAsync();

            policy.PolicyNumber = policy.PolicyId.ToString();
            
            await _context.SaveChangesAsync();

            return MapToDto(policy);
        }

        public async Task<PolicyDto> RenewPolicyAsync(string policyNumber, RenewPolicyDto dto)
        {
            await RefreshPolicyStatusesAsync();

           
            var policy = await _context.Policies
                .Include(p => p.PolicyCoverages)
                .Include(p => p.PolicyTransactions)
                .Include(p => p.PolicyRenewals)
                .SingleOrDefaultAsync(p => p.PolicyNumber == policyNumber);

            if (policy == null)
                throw new KeyNotFoundException($"Policy '{policyNumber}' was not found.");

            if (policy.Status == "Cancelled" || policy.Status == "Expired")
                throw new InvalidOperationException($"Policy cannot be renewed because current status is '{policy.Status}'.");

            // Rule: Renewal is allowed only within 30 days of expiration
            var daysToExpiration = (policy.EndDate - DateTime.UtcNow.Date).Days;
            if (daysToExpiration > 30)
                throw new InvalidOperationException($"Renewal allowed only within 30 days of expiration. Current remaining days: {daysToExpiration}.");

            var previousEndDate = policy.EndDate;
            var newStartDate = previousEndDate.AddDays(1);

            var totalMonths = (dto.ExtensionYears * 12) + dto.ExtensionMonths;

            if (totalMonths <= 0)
            {
                throw new InvalidOperationException(
                    "Renewal period must be at least 1 month."
                );
            }

            var newEndDate = newStartDate
                .AddYears(dto.ExtensionYears)
                .AddMonths(dto.ExtensionMonths)
                .AddDays(-1);

            var previousPremium = policy.Premium;
            var newPremium = dto.ProposedPremium > 0 ? dto.ProposedPremium : previousPremium;

            
            var renewalRecord = new PolicyRenewal
            {
                PolicyId = policy.PolicyId,
                PreviousEndDate = previousEndDate,
                NewStartDate = newStartDate,
                NewEndDate = newEndDate,
                PreviousPremium = previousPremium,
                NewPremium = newPremium,
                RenewalDate = DateTime.UtcNow
            };

            policy.PolicyRenewals.Add(renewalRecord);
            
            
            policy.PolicyTransactions.Add(new PolicyTransaction
            {
                PolicyId = policy.PolicyId,
                TransactionType = "Renewed",
                EffectiveDate = DateTime.UtcNow,
                Notes = dto.Notes
            });

            // Update Policy dates and premium
            policy.EndDate = newEndDate;
            policy.Premium = newPremium;
            policy.Status = DetermineStatus(policy.StartDate, policy.EndDate, isCancelled: false);

            await _context.SaveChangesAsync();

            return MapToDto(policy);
        }

        public async Task<PolicyDto> CancelPolicyAsync(string policyNumber, CancelPolicyDto dto)
        {
            
            var policy = await _context.Policies
                .Include(p => p.PolicyCoverages)
                .Include(p => p.PolicyTransactions)
                .Include(p => p.PolicyRenewals)
                .SingleOrDefaultAsync(p => p.PolicyNumber == policyNumber);

            if (policy == null)
                throw new KeyNotFoundException($"Policy '{policyNumber}' was not found.");

            if (policy.Status == "Cancelled")
                throw new InvalidOperationException("Policy is already cancelled.");

            if (dto.CancellationDate < policy.StartDate)
                throw new ArgumentException($"Cancellation date ({dto.CancellationDate:yyyy-MM-dd}) cannot be before Policy Start Date ({policy.StartDate:yyyy-MM-dd}).");

            if (string.IsNullOrWhiteSpace(dto.Reason))
                throw new ArgumentException("Cancellation reason is required.");

            policy.Status = "Cancelled";

            policy.PolicyTransactions.Add(new PolicyTransaction
            {
                PolicyId = policy.PolicyId,
                TransactionType = "Cancelled",
                EffectiveDate = dto.CancellationDate,
                Notes = $"Reason: {dto.Reason}. {dto.Notes}"
            });

            await _context.SaveChangesAsync();

            return MapToDto(policy);
        }

        public async Task<PolicyDto> UpdatePolicyAsync(string policyNumber, UpdatePolicyDto dto)
        {
            var policy = await _context.Policies
                .Include(p => p.PolicyCoverages)
                .Include(p => p.PolicyTransactions)
                .Include(p => p.PolicyRenewals)
                .SingleOrDefaultAsync(p => p.PolicyNumber == policyNumber);

            if (policy == null)
                throw new KeyNotFoundException($"Policy '{policyNumber}' was not found.");

            if (!string.IsNullOrWhiteSpace(dto.CustomerEmail))
            {
                var isValidEmail = Regex.IsMatch(dto.CustomerEmail,@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                
                if (!isValidEmail)
                throw new ArgumentException("Invalid email format.");
                
                policy.CustomerEmail = dto.CustomerEmail;
            }
                

            if (!string.IsNullOrWhiteSpace(dto.CustomerPhone))
            {
                if (!Regex.IsMatch(dto.CustomerPhone, @"^[0-9]{10}$"))
                throw new ArgumentException("Phone number must contain exactly 10 digits.");

                policy.CustomerPhone = dto.CustomerPhone;
            }
                

            if (!string.IsNullOrWhiteSpace(dto.PaymentFrequency))
                policy.PaymentFrequency = dto.PaymentFrequency;

            await _context.SaveChangesAsync();
            return MapToDto(policy);
        }

        public async Task<List<PolicyDto>> GetExpiringPoliciesAsync(int withinDays = 30)
        {
            await RefreshPolicyStatusesAsync();

            var cutoffDate = DateTime.UtcNow.Date.AddDays(withinDays);

            // LINQ Demonstration: Where (finding expiring policies)
            var expiring = await _context.Policies
                .Include(p => p.PolicyCoverages)
                .Include(p => p.PolicyTransactions)
                .Include(p => p.PolicyRenewals)
                .Where(p => p.Status != "Cancelled" && p.EndDate <= cutoffDate && p.EndDate >= DateTime.UtcNow.Date)
                .OrderBy(p => p.EndDate)
                .ToListAsync();

            return expiring.Select(MapToDto).ToList();
        }

        public async Task<DashboardDto> GetDashboardMetricsAsync()
        {
            await RefreshPolicyStatusesAsync();

            var all = await _context.Policies.ToListAsync();

            
            var totalWrittenPremium = _context.Policies
                .Where(p => p.Status == "Active" || p.Status == "Expiring Soon")
                .Sum(p => p.Premium);

            
            var statusBreakdown = _context.Policies
                .GroupBy(p => p.Status)
                .Select(g => new StatusSummaryDto
                {
                    Status = g.Key,
                    Count = g.Count(),
                    TotalPremium = g.Sum(p => p.Premium)
                })
                .ToList();

            return new DashboardDto
            {
                TotalPolicies = all.Count,
                ActiveCount = all.Count(p => p.Status == "Active"),
                ExpiringSoonCount = all.Count(p => p.Status == "Expiring Soon"),
                ExpiredCount = all.Count(p => p.Status == "Expired"),
                CancelledCount = all.Count(p => p.Status == "Cancelled"),
                PendingCount = all.Count(p => p.Status == "Pending"),
                TotalWrittenPremium = totalWrittenPremium,
                StatusBreakdown = statusBreakdown
            };
        }

        public async Task<List<PolicyCoverageReportDto>> GetFlattenedCoverageReportAsync()
        {
            
            var report = await _context.Policies
                .Include(p => p.PolicyCoverages)
                .SelectMany(p => p.PolicyCoverages, (p, c) => new PolicyCoverageReportDto
                {
                    PolicyNumber = p.PolicyNumber!,
                    CustomerName = p.CustomerName,
                    PetName = p.PetName,
                    CoverageType = c.CoverageType,
                    AnnualLimit = c.AnnualLimit,
                    Deductible = c.Deductible,
                    ReimbursementPct = c.ReimbursementPct
                })
                .ToListAsync();

            return report;
        }

        private async Task RefreshPolicyStatusesAsync()
        {
            var policies = await _context.Policies.Where(p => p.Status != "Cancelled").ToListAsync();
            bool modified = false;

            foreach (var p in policies)
            {
                var newStatus = DetermineStatus(p.StartDate, p.EndDate, isCancelled: false);
                if (p.Status != newStatus)
                {
                    p.Status = newStatus;
                    modified = true;
                }
            }

            if (modified)
            {
                await _context.SaveChangesAsync();
            }
        }

        private static string DetermineStatus(DateTime startDate, DateTime endDate, bool isCancelled)
        {
            if (isCancelled) return "Cancelled";
            var today = DateTime.UtcNow.Date;
            if (endDate < today) return "Expired";
            if ((endDate - today).Days <= 30) return "Expiring Soon";
            if (startDate > today) return "Pending";        
            return "Active";
        }

        private static PolicyDto MapToDto(Policy p) => new()
        {
            PolicyId = p.PolicyId,
            PolicyNumber = p.PolicyNumber!,
            QuoteId = p.QuoteId,
            CustomerId = p.CustomerId,
            CustomerName = p.CustomerName,
            CustomerEmail = p.CustomerEmail,
            CustomerPhone = p.CustomerPhone,
            PetId = p.PetId,
            PetName = p.PetName,
            PetType = p.PetType,
            Breed = p.Breed,
            Age = p.Age,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            Premium = p.Premium,
            PaymentFrequency = p.PaymentFrequency,
            Status = p.Status,
            Coverages = p.PolicyCoverages.Select(c => new PolicyCoverageDto
            {
                PolicyCoverageId = c.PolicyCoverageId,
                PolicyId = c.PolicyId,
                CoverageType = c.CoverageType,
                AnnualLimit = c.AnnualLimit,
                Deductible = c.Deductible,
                ReimbursementPct = c.ReimbursementPct
            }).ToList(),
            Transactions = p.PolicyTransactions.Select(t => new PolicyTransactionDto
            {
                TransactionId = t.TransactionId,
                PolicyId = t.PolicyId,
                TransactionType = t.TransactionType,
                EffectiveDate = t.EffectiveDate,
                Notes = t.Notes
            }).OrderByDescending(t => t.EffectiveDate).ToList(),
            Renewals = p.PolicyRenewals.Select(r => new PolicyRenewalDto
            {
                RenewalId = r.RenewalId,
                PolicyId = r.PolicyId,
                PreviousEndDate = r.PreviousEndDate,
                NewStartDate = r.NewStartDate,
                NewEndDate = r.NewEndDate,
                PreviousPremium = r.PreviousPremium,
                NewPremium = r.NewPremium,
                RenewalDate = r.RenewalDate
            }).OrderByDescending(r => r.RenewalDate).ToList()
        };
    }
}
