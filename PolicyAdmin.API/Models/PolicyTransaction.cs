using System;
using System.Text.Json.Serialization;

namespace PolicyAdmin.API.Models
{
    public class PolicyTransaction
    {
        public int TransactionId { get; set; }
        public int PolicyId { get; set; }
        public string TransactionType { get; set; } = string.Empty; // Created, Renewed, Cancelled
        public DateTime EffectiveDate { get; set; }
        public string Notes { get; set; } = string.Empty;

        [JsonIgnore]
        public Policy? Policy { get; set; }
    }
}
