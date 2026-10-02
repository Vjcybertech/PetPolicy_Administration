using System;

namespace PolicyAdmin.API.Models
{
    public class Quote
    {
        public string QuoteId { get; set; } = string.Empty; // e.g. "QT-1001"
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;

        public string PetId { get; set; } = string.Empty;
        public string PetName { get; set; } = string.Empty;
        public string PetType { get; set; } = string.Empty; // Dog, Cat
        public string Breed { get; set; } = string.Empty;
        public int Age { get; set; }

        public decimal AnnualPremium { get; set; }
        public string PaymentFrequency { get; set; } = "Annual"; // Monthly, Annual
        public string Status { get; set; } = "Approved"; // Approved, Converted, Expired, Pending

        // Proposed Coverage Details
        public string CoverageType { get; set; } = "Comprehensive";
        public decimal AnnualLimit { get; set; } = 10000m;
        public decimal Deductible { get; set; } = 250m;
        public decimal ReimbursementPct { get; set; } = 80m;
    }
}
