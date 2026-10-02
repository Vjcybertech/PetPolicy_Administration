using Microsoft.AspNetCore.Mvc;
using PolicyAdmin.API.DTOs;
using PolicyAdmin.API.Repository;
using PolicyAdmin.API.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        /// <summary>
        /// GET /api/policies/expiring - List policies expiring within specified days
        /// </summary>
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

        /// <summary>
        /// POST /api/policies - Create policy from approved quote
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PolicyDto>> CreatePolicy([FromBody] CreatePolicyDto dto)
        {
            var policy = await _repo.CreatePolicyAsync(dto);
            return CreatedAtAction(nameof(GetPolicyByNumber), new { policyNumber = policy.PolicyNumber }, policy);
        }

        /// <summary>
        /// PUT /api/policies/{policyNumber} - Update allowable fields
        /// </summary>
        [HttpPut("{policyNumber}")]
        public async Task<ActionResult<PolicyDto>> UpdatePolicy(string policyNumber, [FromBody] UpdatePolicyDto dto)
        {
            var updated = await _repo.UpdatePolicyAsync(policyNumber, dto);
            return Ok(updated);
        }

        /// <summary>
        /// POST /api/policies/{policyNumber}/renew - Renew eligible policy
        /// </summary>
        [HttpPost("{policyNumber}/renew")]
        public async Task<ActionResult<PolicyDto>> RenewPolicy(string policyNumber, [FromBody] RenewPolicyDto dto)
        {
            var renewed = await _repo.RenewPolicyAsync(policyNumber, dto);
            return Ok(renewed);
        }

        /// <summary>
        /// POST /api/policies/{policyNumber}/cancel - Cancel policy
        /// </summary>
        [HttpPost("{policyNumber}/cancel")]
        public async Task<ActionResult<PolicyDto>> CancelPolicy(string policyNumber, [FromBody] CancelPolicyDto dto)
        {
            var cancelled = await _repo.CancelPolicyAsync(policyNumber, dto);
            return Ok(cancelled);
        }
    }
}
