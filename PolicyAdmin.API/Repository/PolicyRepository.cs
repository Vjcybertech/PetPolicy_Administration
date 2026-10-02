
using PolicyAdmin.API.DTOs;

using PolicyAdmin.API.Services;


namespace PolicyAdmin.API.Repository
{
    public class PolicyRepository:IpolicyRepository
    {
        private readonly IPolicyService _service;

        public PolicyRepository(IPolicyService service)
        {
            _service=service;
        }

       public Task<List<PolicyDto>> GetAllPoliciesAsync(string? status = null)
        {
            return _service.GetAllPoliciesAsync(status);
        }

       public  Task<PolicyDto?> GetPolicyByNumberAsync(string policyNumber)
        {
            return _service.GetPolicyByNumberAsync(policyNumber);
        }

       public Task<PolicyDto> CreatePolicyAsync(CreatePolicyDto dto)
        {
            return _service.CreatePolicyAsync(dto);
        }
        public Task<PolicyDto> UpdatePolicyAsync(string policyNumber, UpdatePolicyDto dto)
        {
            return _service.UpdatePolicyAsync(policyNumber,dto);
        }


       public Task<PolicyDto> RenewPolicyAsync(string policyNumber, RenewPolicyDto dto)
        {
            return _service.RenewPolicyAsync(policyNumber,dto);
        }

      public  Task<PolicyDto> CancelPolicyAsync(string policyNumber, CancelPolicyDto dto)
        {
            return _service.CancelPolicyAsync(policyNumber,dto);
        }

      public  Task<List<PolicyDto>> GetExpiringPoliciesAsync(int withinDays = 30)
        {
            return _service.GetExpiringPoliciesAsync(withinDays);
        }

      public  Task<DashboardDto> GetDashboardMetricsAsync()
        {
            return _service.GetDashboardMetricsAsync();
        }
       public Task<List<PolicyCoverageReportDto>> GetFlattenedCoverageReportAsync()
        {
            return _service.GetFlattenedCoverageReportAsync();
        }


    }
}