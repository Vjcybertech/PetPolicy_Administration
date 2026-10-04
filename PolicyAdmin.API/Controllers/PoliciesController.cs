using Microsoft.AspNetCore.Mvc;
using PolicyAdmin.API.DTOs;
using PolicyAdmin.API.Repository;


namespace PolicyAdmin.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PoliciesController : ControllerBase
    {
        private readonly IpolicyRepository _repo;
        public PoliciesController(IpolicyRepository repository)
        {
            _repo= repository;
        }

        
        [HttpGet]
        public async Task<ActionResult<List<PolicyDto>>> GetPolicies([FromQuery] string? status)
        {
            var policies = await _repo.GetAllPoliciesAsync(status);
            return Ok(policies);
        }

        
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardDto>> GetDashboard()
        {
            var metrics = await _repo.GetDashboardMetricsAsync();
            return Ok(metrics);
        }

        
        [HttpGet("expiring")]
        public async Task<ActionResult<List<PolicyDto>>> GetExpiringPolicies([FromQuery] int withinDays = 30)
        {
            var expiring = await _repo.GetExpiringPoliciesAsync(withinDays);
            return Ok(expiring);
        }

       
        [HttpGet("{policyNumber}")]
        public async Task<ActionResult<PolicyDto>> GetPolicyByNumber(string policyNumber)
        {
            var policy = await _repo.GetPolicyByNumberAsync(policyNumber);
            if (policy == null)
                return NotFound(new { message = $"Policy '{policyNumber}' not found." });

            return Ok(policy);
        }

        
        [HttpPost]
        public async Task<ActionResult<PolicyDto>> CreatePolicy([FromBody] CreatePolicyDto dto)
        {
            var policy = await _repo.CreatePolicyAsync(dto);
            return CreatedAtAction(nameof(GetPolicyByNumber), new { policyNumber = policy.PolicyNumber }, policy);
        }

        
        [HttpPut("{policyNumber}")]
        public async Task<ActionResult<PolicyDto>> UpdatePolicy(string policyNumber, [FromBody] UpdatePolicyDto dto)
        {
            var updated = await _repo.UpdatePolicyAsync(policyNumber, dto);
            return Ok(updated);
        }

        
        [HttpPost("{policyNumber}/renew")]
        public async Task<ActionResult<PolicyDto>> RenewPolicy(string policyNumber, [FromBody] RenewPolicyDto dto)
        {
            var renewed = await _repo.RenewPolicyAsync(policyNumber, dto);
            return Ok(renewed);
        }

        
        [HttpPost("{policyNumber}/cancel")]
        public async Task<ActionResult<PolicyDto>> CancelPolicy(string policyNumber, [FromBody] CancelPolicyDto dto)
        {
            var cancelled = await _repo.CancelPolicyAsync(policyNumber, dto);
            return Ok(cancelled);
        }
    }
}
