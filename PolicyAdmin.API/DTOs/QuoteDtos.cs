namespace PolicyAdmin.API.DTOs
{
    public class QuoteDto
    {
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

        public decimal AnnualPremium { get; set; }
        public string PaymentFrequency { get; set; } = "Annual";
        public string Status { get; set; } = string.Empty;

        public string CoverageType { get; set; } = "Comprehensive";
        public decimal AnnualLimit { get; set; }
        public decimal Deductible { get; set; }
        public decimal ReimbursementPct { get; set; }
    }
}
