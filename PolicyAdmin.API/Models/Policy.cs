using System;
using System.Collections.Generic;

namespace PolicyAdmin.API.Models
{
    public class Policy
    {
        public int PolicyId { get; set; }
        public string? PolicyNumber { get; set; } = string.Empty; 
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
        public string PaymentFrequency { get; set; } = "Annual"; // Monthly / Annual
        public string Status { get; set; } = "Active"; // Pending, Active, Expiring Soon, Expired, Cancelled

        
        public ICollection<PolicyCoverage> PolicyCoverages { get; set; } = new List<PolicyCoverage>();
        public ICollection<PolicyTransaction> PolicyTransactions { get; set; } = new List<PolicyTransaction>();
        public ICollection<PolicyRenewal> PolicyRenewals { get; set; } = new List<PolicyRenewal>();
    }
}
