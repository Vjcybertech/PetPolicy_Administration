using System;
using System.Text.Json.Serialization;

namespace PolicyAdmin.API.Models
{
    public class PolicyRenewal
    {
        public int RenewalId { get; set; }
        public int PolicyId { get; set; }
        public DateTime PreviousEndDate { get; set; }
        public DateTime NewStartDate { get; set; }
        public DateTime NewEndDate { get; set; }
        public decimal PreviousPremium { get; set; }
        public decimal NewPremium { get; set; }
        public DateTime RenewalDate { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public Policy? Policy { get; set; }
    }
}
