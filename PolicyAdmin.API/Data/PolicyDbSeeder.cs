using PolicyAdmin.API.Models;
using System;
using System.Linq;

namespace PolicyAdmin.API.Data
{
    public static class PolicyDbSeeder
    {
        public static void Seed(PolicyDbContext context)
        {
            if (context.Quotes.Any() || context.Policies.Any())
                return; 

            var now = DateTime.UtcNow.Date;

            // Seed Quotes
            context.Quotes.AddRange(
                new Quote
                {
                    QuoteId = "QT-1001",
                    CustomerId = "CUST-501",
                    CustomerName = "Sarah Jenkins",
                    CustomerEmail = "sarah.jenkins@example.com",
                    CustomerPhone = "555-0192",
                    PetId = "PET-901",
                    PetName = "Bella",
                    PetType = "Dog",
                    Breed = "Golden Retriever",
                    Age = 3,
                    AnnualPremium = 650.00m,
                    PaymentFrequency = "Monthly",
                    Status = "Approved",
                    CoverageType = "Comprehensive",
                    AnnualLimit = 15000m,
                    Deductible = 250m,
                    ReimbursementPct = 80m
                },
                new Quote
                {
                    QuoteId = "QT-1002",
                    CustomerId = "CUST-502",
                    CustomerName = "Michael Vance",
                    CustomerEmail = "michael.vance@example.com",
                    CustomerPhone = "555-0144",
                    PetId = "PET-902",
                    PetName = "Oliver",
                    PetType = "Cat",
                    Breed = "Siamese",
                    Age = 2,
                    AnnualPremium = 420.00m,
                    PaymentFrequency = "Annual",
                    Status = "Approved",
                    CoverageType = "Illness & Accident",
                    AnnualLimit = 10000m,
                    Deductible = 200m,
                    ReimbursementPct = 90m
                },
                new Quote
                {
                    QuoteId = "QT-1003",
                    CustomerId = "CUST-503",
                    CustomerName = "Elena Rostova",
                    CustomerEmail = "elena.rostova@example.com",
                    CustomerPhone = "555-0188",
                    PetId = "PET-903",
                    PetName = "Max",
                    PetType = "Dog",
                    Breed = "German Shepherd",
                    Age = 5,
                    AnnualPremium = 890.00m,
                    PaymentFrequency = "Annual",
                    Status = "Converted",
                    CoverageType = "Comprehensive",
                    AnnualLimit = 20000m,
                    Deductible = 500m,
                    ReimbursementPct = 80m
                }
            );

            // Seed Policies
            var p1Start = now.AddMonths(-6);
            var p1End = p1Start.AddYears(1);

            var p2Start = now.AddMonths(-11).AddDays(-15);
            var p2End = p2Start.AddYears(1); // Expiring Soon

            var p3Start = now.AddYears(-2);
            var p3End = now.AddYears(-1); // Expired

            var p4Start = now.AddMonths(-4);
            var p4End = p4Start.AddYears(1); // Cancelled

            var pol1 = new Policy
            {
                PolicyNumber = "1",
                QuoteId = "QT-1000",
                CustomerId = "CUST-500",
                CustomerName = "David Miller",
                CustomerEmail = "david.miller@example.com",
                CustomerPhone = "555-0111",
                PetId = "PET-900",
                PetName = "Luna",
                PetType = "Dog",
                Breed = "French Bulldog",
                Age = 4,
                StartDate = p1Start,
                EndDate = p1End,
                Premium = 720.00m,
                PaymentFrequency = "Monthly",
                Status = "Active"
            };
            pol1.PolicyCoverages.Add(new PolicyCoverage
            {
                CoverageType = "Comprehensive",
                AnnualLimit = 12000m,
                Deductible = 250m,
                ReimbursementPct = 80m
            });
            pol1.PolicyTransactions.Add(new PolicyTransaction
            {
                TransactionType = "Created",
                EffectiveDate = p1Start,
                Notes = "Initial policy creation from quote QT-1000"
            });
            pol1.PolicyRenewals.Add(new PolicyRenewal
            {
                PreviousEndDate = p1Start,
                NewStartDate = p1Start,
                NewEndDate = p1End,
                PreviousPremium = 680.00m,
                NewPremium = 720.00m,
                RenewalDate = p1Start
            });

            var pol2 = new Policy
            {
                PolicyNumber = "POL-2026-002",
                QuoteId = "QT-999",
                CustomerId = "CUST-499",
                CustomerName = "Amanda Lewis",
                CustomerEmail = "amanda.lewis@example.com",
                CustomerPhone = "555-0222",
                PetId = "PET-899",
                PetName = "Milo",
                PetType = "Cat",
                Breed = "Maine Coon",
                Age = 3,
                StartDate = p2Start,
                EndDate = p2End,
                Premium = 510.00m,
                PaymentFrequency = "Annual",
                Status = "Expiring Soon"
            };
            pol2.PolicyCoverages.Add(new PolicyCoverage
            {
                CoverageType = "Illness & Accident",
                AnnualLimit = 10000m,
                Deductible = 200m,
                ReimbursementPct = 90m
            });
            pol2.PolicyTransactions.Add(new PolicyTransaction
            {
                TransactionType = "Created",
                EffectiveDate = p2Start,
                Notes = "Initial policy creation from quote QT-999"
            });

            var pol3 = new Policy
            {
                PolicyNumber = "POL-2025-089",
                QuoteId = "QT-888",
                CustomerId = "CUST-488",
                CustomerName = "Robert Chen",
                CustomerEmail = "robert.chen@example.com",
                CustomerPhone = "555-0333",
                PetId = "PET-788",
                PetName = "Rocky",
                PetType = "Dog",
                Breed = "Boxer",
                Age = 6,
                StartDate = p3Start,
                EndDate = p3End,
                Premium = 600.00m,
                PaymentFrequency = "Annual",
                Status = "Expired"
            };
            pol3.PolicyCoverages.Add(new PolicyCoverage
            {
                CoverageType = "Standard Accident",
                AnnualLimit = 5000m,
                Deductible = 300m,
                ReimbursementPct = 70m
            });
            pol3.PolicyTransactions.Add(new PolicyTransaction
            {
                TransactionType = "Created",
                EffectiveDate = p3Start,
                Notes = "Initial policy creation from quote QT-888"
            });

            var pol4 = new Policy
            {
                PolicyNumber = "POL-2026-003",
                QuoteId = "QT-777",
                CustomerId = "CUST-477",
                CustomerName = "Jessica Taylor",
                CustomerEmail = "jessica.taylor@example.com",
                CustomerPhone = "555-0444",
                PetId = "PET-677",
                PetName = "Charlie",
                PetType = "Dog",
                Breed = "Beagle",
                Age = 2,
                StartDate = p4Start,
                EndDate = p4End,
                Premium = 540.00m,
                PaymentFrequency = "Monthly",
                Status = "Cancelled"
            };
            pol4.PolicyCoverages.Add(new PolicyCoverage
            {
                CoverageType = "Comprehensive",
                AnnualLimit = 10000m,
                Deductible = 250m,
                ReimbursementPct = 80m
            });
            pol4.PolicyTransactions.Add(new PolicyTransaction
            {
                TransactionType = "Created",
                EffectiveDate = p4Start,
                Notes = "Initial policy creation from quote QT-777"
            });
            pol4.PolicyTransactions.Add(new PolicyTransaction
            {
                TransactionType = "Cancelled",
                EffectiveDate = p4Start.AddMonths(2),
                Notes = "Cancelled by customer request - pet relocated"
            });

            context.Policies.AddRange(pol1, pol2, pol3, pol4);
            context.SaveChanges();
        }
    }
}
