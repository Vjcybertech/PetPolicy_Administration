using PolicyAdmin.API.DTOs;

namespace PolicyAdmin.API.Repository
{
    public interface IpolicyRepository
    {
        Task<List<PolicyDto>> GetAllPoliciesAsync(string? status = null, int pageNumber = 1, int pageSize = 10);
        Task<PolicyDto?> GetPolicyByNumberAsync(string policyNumber);
        Task<PolicyDto> CreatePolicyAsync(CreatePolicyDto dto);
        Task<PolicyDto> RenewPolicyAsync(string policyNumber, RenewPolicyDto dto);
        Task<PolicyDto> CancelPolicyAsync(string policyNumber, CancelPolicyDto dto);
        Task<PolicyDto> UpdatePolicyAsync(string policyNumber, UpdatePolicyDto dto);
        Task<List<PolicyDto>> GetExpiringPoliciesAsync(int withinDays = 30);
        Task<DashboardDto> GetDashboardMetricsAsync();
        Task<List<PolicyCoverageReportDto>> GetFlattenedCoverageReportAsync();
    }
    
}