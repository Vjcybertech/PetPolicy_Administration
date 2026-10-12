using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using PolicyAdmin.API.Data;
using PolicyAdmin.API.Models;


namespace PolicyAdmin.API.Services
{
    public class ExcelService : IExcelService
    {
        private readonly PolicyDbContext _context;

        public ExcelService(PolicyDbContext context)
        {
            _context = context;
        }

        public async Task<byte[]> ExportPoliciesToExcelAsync()
        {
            var policies = await _context.Policies
                .Include(p => p.PolicyCoverages)
                .Include(p => p.PolicyTransactions)
                .Include(p => p.PolicyRenewals)
                .ToListAsync();

            using var workbook = new XLWorkbook();

            // Sheet 1: Policies
            var wsPolicies = workbook.Worksheets.Add("Policies");
            wsPolicies.Cell(1, 1).Value = "PolicyId";
            wsPolicies.Cell(1, 2).Value = "PolicyNumber";
            wsPolicies.Cell(1, 3).Value = "QuoteId";
            wsPolicies.Cell(1, 4).Value = "CustomerId";
            wsPolicies.Cell(1, 5).Value = "CustomerName";
            wsPolicies.Cell(1, 6).Value = "CustomerEmail";
            wsPolicies.Cell(1, 7).Value = "PetId";
            wsPolicies.Cell(1, 8).Value = "PetName";
            wsPolicies.Cell(1, 9).Value = "PetType";
            wsPolicies.Cell(1, 10).Value = "StartDate";
            wsPolicies.Cell(1, 11).Value = "EndDate";
            wsPolicies.Cell(1, 12).Value = "Premium";
            wsPolicies.Cell(1, 13).Value = "PaymentFrequency";
            wsPolicies.Cell(1, 14).Value = "Status";

            var row = 2;
            foreach (var p in policies)
            {
                wsPolicies.Cell(row, 1).Value = p.PolicyId;
                wsPolicies.Cell(row, 2).Value = p.PolicyNumber;
                wsPolicies.Cell(row, 3).Value = p.QuoteId;
                wsPolicies.Cell(row, 4).Value = p.CustomerId;
                wsPolicies.Cell(row, 5).Value = p.CustomerName;
                wsPolicies.Cell(row, 6).Value = p.CustomerEmail;
                wsPolicies.Cell(row, 7).Value = p.PetId;
                wsPolicies.Cell(row, 8).Value = p.PetName;
                wsPolicies.Cell(row, 9).Value = p.PetType;
                wsPolicies.Cell(row, 10).Value = p.StartDate.ToString("yyyy-MM-dd");
                wsPolicies.Cell(row, 11).Value = p.EndDate.ToString("yyyy-MM-dd");
                wsPolicies.Cell(row, 12).Value = p.Premium;
                wsPolicies.Cell(row, 13).Value = p.PaymentFrequency;
                wsPolicies.Cell(row, 14).Value = p.Status;
                row++;
            }
            wsPolicies.Columns().AdjustToContents();

            // Sheet 2: PolicyCoverages
            var wsCoverages = workbook.Worksheets.Add("PolicyCoverages");
            wsCoverages.Cell(1, 1).Value = "PolicyCoverageId";
            wsCoverages.Cell(1, 2).Value = "PolicyNumber";
            wsCoverages.Cell(1, 3).Value = "CoverageType";
            wsCoverages.Cell(1, 4).Value = "AnnualLimit";
            wsCoverages.Cell(1, 5).Value = "Deductible";
            wsCoverages.Cell(1, 6).Value = "ReimbursementPct";

            row = 2;
            foreach (var p in policies)
            {
                foreach (var c in p.PolicyCoverages)
                {
                    wsCoverages.Cell(row, 1).Value = c.PolicyCoverageId;
                    wsCoverages.Cell(row, 2).Value = p.PolicyNumber;
                    wsCoverages.Cell(row, 3).Value = c.CoverageType;
                    wsCoverages.Cell(row, 4).Value = c.AnnualLimit;
                    wsCoverages.Cell(row, 5).Value = c.Deductible;
                    wsCoverages.Cell(row, 6).Value = c.ReimbursementPct;
                    row++;
                }
            }
            wsCoverages.Columns().AdjustToContents();

            // Sheet 3: PolicyTransactions
            var wsTx = workbook.Worksheets.Add("PolicyTransactions");
            wsTx.Cell(1, 1).Value = "TransactionId";
            wsTx.Cell(1, 2).Value = "PolicyNumber";
            wsTx.Cell(1, 3).Value = "TransactionType";
            wsTx.Cell(1, 4).Value = "EffectiveDate";
            wsTx.Cell(1, 5).Value = "Notes";

            row = 2;
            foreach (var p in policies)
            {
                foreach (var t in p.PolicyTransactions)
                {
                    wsTx.Cell(row, 1).Value = t.TransactionId;
                    wsTx.Cell(row, 2).Value = p.PolicyNumber;
                    wsTx.Cell(row, 3).Value = t.TransactionType;
                    wsTx.Cell(row, 4).Value = t.EffectiveDate.ToString("yyyy-MM-dd HH:mm:ss");
                    wsTx.Cell(row, 5).Value = t.Notes;
                    row++;
                }
            }
            wsTx.Columns().AdjustToContents();

            // Sheet 4: PolicyRenewals
            var wsRen = workbook.Worksheets.Add("PolicyRenewals");
            wsRen.Cell(1, 1).Value = "RenewalId";
            wsRen.Cell(1, 2).Value = "PolicyNumber";
            wsRen.Cell(1, 3).Value = "PreviousEndDate";
            wsRen.Cell(1, 4).Value = "NewStartDate";
            wsRen.Cell(1, 5).Value = "NewEndDate";
            wsRen.Cell(1, 6).Value = "PreviousPremium";
            wsRen.Cell(1, 7).Value = "NewPremium";
            wsRen.Cell(1, 8).Value = "RenewalDate";

            row = 2;
            foreach (var p in policies)
            {
                foreach (var r in p.PolicyRenewals)
                {
                    wsRen.Cell(row, 1).Value = r.RenewalId;
                    wsRen.Cell(row, 2).Value = p.PolicyNumber;
                    wsRen.Cell(row, 3).Value = r.PreviousEndDate.ToString("yyyy-MM-dd");
                    wsRen.Cell(row, 4).Value = r.NewStartDate.ToString("yyyy-MM-dd");
                    wsRen.Cell(row, 5).Value = r.NewEndDate.ToString("yyyy-MM-dd");
                    wsRen.Cell(row, 6).Value = r.PreviousPremium;
                    wsRen.Cell(row, 7).Value = r.NewPremium;
                    wsRen.Cell(row, 8).Value = r.RenewalDate.ToString("yyyy-MM-dd HH:mm:ss");
                    row++;
                }
            }
            wsRen.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        public async Task<int> ImportPoliciesFromExcelAsync(Stream excelStream)
        {
            using var workbook = new XLWorkbook(excelStream);

            var wsPolicies = workbook.Worksheet("Policies");

            if (wsPolicies == null)
                throw new InvalidOperationException(
                    "Workbook does not contain a 'Policies' sheet.");

            var rows = wsPolicies.RowsUsed().Skip(1);

            int importedCount = 0;

            foreach (var row in rows)
            {
                var policyNumber = row.Cell(2).GetString();

                if (string.IsNullOrWhiteSpace(policyNumber))
                    continue;

                var quoteId = row.Cell(3).GetString();

                if (string.IsNullOrWhiteSpace(quoteId))
                {
                    throw new InvalidOperationException(
                        $"Policy '{policyNumber}' does not contain a Quote Id.");
                }

                // Check Quote Exists
                var quote = await _context.Quotes
                    .SingleOrDefaultAsync(q => q.QuoteId == quoteId);

                if (quote == null)
                {
                    throw new InvalidOperationException(
                        $"Quote '{quoteId}' was not found.");
                }

                // Check Quote Status
                if (!string.Equals(
                        quote.Status,
                        "Approved",
                        StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        $"Quote '{quoteId}' is not approved. Current status: {quote.Status}");
                }

                // Check whether QuoteId is already used in Policies table
                var quoteAlreadyConverted = await _context.Policies
                    .AnyAsync(p => p.QuoteId == quoteId);

                if (quoteAlreadyConverted)
                {
                    throw new InvalidOperationException(
                        $"Quote '{quoteId}' has already been converted into a policy.");
                }

                // Check duplicate Policy Number
                var existingPolicy = await _context.Policies
                    .SingleOrDefaultAsync(p => p.PolicyNumber == policyNumber);

                if (existingPolicy != null)
                {
                    throw new InvalidOperationException(
                        $"Policy Number '{policyNumber}' already exists.");
                }

                var policy = new Policy
                {
                    PolicyNumber = policyNumber,
                    QuoteId = quoteId,
                    CustomerId = row.Cell(4).GetString(),
                    CustomerName = row.Cell(5).GetString(),
                    CustomerEmail = row.Cell(6).GetString(),
                    PetId = row.Cell(7).GetString(),
                    PetName = row.Cell(8).GetString(),
                    PetType = row.Cell(9).GetString(),
                    StartDate = DateTime.TryParse(
                        row.Cell(10).GetString(),
                        out var startDate)
                            ? startDate
                            : DateTime.UtcNow.Date,

                    EndDate = DateTime.TryParse(
                        row.Cell(11).GetString(),
                        out var endDate)
                            ? endDate
                            : DateTime.UtcNow.Date.AddYears(1),

                    Premium = decimal.TryParse(
                        row.Cell(12).GetString(),
                        out var premium)
                            ? premium
                            : 500m,

                    PaymentFrequency = row.Cell(13).GetString(),

                    Status = string.IsNullOrWhiteSpace(row.Cell(14).GetString())
                        ? "Active"
                        : row.Cell(14).GetString()
                };

                policy.PolicyCoverages.Add(new PolicyCoverage
                {
                    CoverageType = "Comprehensive (Imported)",
                    AnnualLimit = 10000m,
                    Deductible = 250m,
                    ReimbursementPct = 80m
                });

                policy.PolicyTransactions.Add(new PolicyTransaction
                {
                    TransactionType = "Created",
                    EffectiveDate = DateTime.UtcNow,
                    Notes = "Imported from Excel workbook"
                });

                quote.Status = "Converted";

                _context.Policies.Add(policy);

                importedCount++;
            }

            if (importedCount > 0)
            {
                await _context.SaveChangesAsync();
            }

            return importedCount;
        }
    }
}
