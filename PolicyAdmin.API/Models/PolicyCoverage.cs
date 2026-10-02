using System.Text.Json.Serialization;

namespace PolicyAdmin.API.Models
{
    public class PolicyCoverage
    {
        public int PolicyCoverageId { get; set; }
        public int PolicyId { get; set; }
        
        public string CoverageType { get; set; } = "Comprehensive";
        public decimal AnnualLimit { get; set; }
        public decimal Deductible { get; set; }
        public decimal ReimbursementPct { get; set; }

        [JsonIgnore]
        public Policy? Policy { get; set; }
    }
}
