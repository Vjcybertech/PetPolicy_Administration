namespace PolicyAdmin.API.DTOs
{
    public class PolicyDto
    {
        public int PolicyId { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public string QuoteId { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        
        public string PetId { get; set; } = string.Empty;
        public string PetName { get; set; } = string.Empty;
        public string PetType { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public int Age { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Premium { get; set; }
        public string PaymentFrequency { get; set; } = "Annual";
        public string Status { get; set; } = string.Empty;

        public List<PolicyCoverageDto> Coverages { get; set; } = new();
        public List<PolicyTransactionDto> Transactions { get; set; } = new();
        public List<PolicyRenewalDto> Renewals { get; set; } = new();
    }

    public class PolicyCoverageDto
    {
        public int PolicyCoverageId { get; set; }
        public int PolicyId { get; set; }
        public string CoverageType { get; set; } = string.Empty;
        public decimal AnnualLimit { get; set; }
        public decimal Deductible { get; set; }
        public decimal ReimbursementPct { get; set; }
    }

    public class PolicyTransactionDto
    {
        public int TransactionId { get; set; }
        public int PolicyId { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public DateTime EffectiveDate { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    public class PolicyRenewalDto
    {
        public int RenewalId { get; set; }
        public int PolicyId { get; set; }
        public DateTime PreviousEndDate { get; set; }
        public DateTime NewStartDate { get; set; }
        public DateTime NewEndDate { get; set; }
        public decimal PreviousPremium { get; set; }
        public decimal NewPremium { get; set; }
        public DateTime RenewalDate { get; set; }
    }

    public class CreatePolicyDto
    {
        public string QuoteId { get; set; } = string.Empty;
        public string PolicyNumber { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PaymentFrequency { get; set; } = "Annual";
        public decimal AnnualPremium { get; set; }
        public string CoverageType { get; set; } = "Comprehensive";
        public decimal AnnualLimit { get; set; } = 10000m;
        public decimal Deductible { get; set; } = 250m;
        public decimal ReimbursementPct { get; set; } = 80m;
    }

    public class RenewPolicyDto
    {
        public decimal ProposedPremium { get; set; }
        public int ExtensionYears { get; set; } = 1;
        public string Notes { get; set; } = "Standard annual policy renewal";
    }

    public class CancelPolicyDto
    {
        public DateTime CancellationDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }

    public class UpdatePolicyDto
    {
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string PaymentFrequency { get; set; } = "Annual";
    }

    public class PolicyCoverageReportDto
    {
        public string PolicyNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string PetName { get; set; } = string.Empty;
        public string CoverageType { get; set; } = string.Empty;
        public decimal AnnualLimit { get; set; }
        public decimal Deductible { get; set; }
        public decimal ReimbursementPct { get; set; }
    }
}
