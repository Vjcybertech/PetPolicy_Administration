using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PolicyAdmin.API.Repository;
using PolicyAdmin.API.Services;
using System.Threading.Tasks;

namespace PolicyAdmin.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExcelController : ControllerBase
    {
        private readonly IExcelRepository _repo;

        public ExcelController(IExcelRepository repository)
        {
            _repo = repository;
        }


        [HttpGet("policies/export")]
        public async Task<IActionResult> ExportPolicies()
        {
            var bytes = await _repo.ExportPoliciesToExcelAsync();
            var fileName = $"Policies_Workbook_{System.DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }


        [HttpPost("policies/import")]
        public async Task<IActionResult> ImportPolicies(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Please upload a valid Excel file." });

            using var stream = file.OpenReadStream();
            var count = await _repo.ImportPoliciesFromExcelAsync(stream);

            return Ok(new { message = $"Successfully imported {count} new policy record(s) from Excel workbook.", importedCount = count });
        }
    }
}
