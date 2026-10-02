using Microsoft.AspNetCore.Mvc;
using PolicyAdmin.API.DTOs;
using PolicyAdmin.API.Services;
using PolicyAdmin.API.Repository;

namespace PolicyAdmin.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuotesController : ControllerBase
    {
        private readonly IQuoteRepository _repo;

        public QuotesController(IQuoteRepository quoteRepository)
        {
            _repo=quoteRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<QuoteDto>>> GetQuotes([FromQuery] string? status)
        {
            var quotes = await _repo.GetAllQuotesAsync(status);
            return Ok(quotes);
        }

        [HttpGet("{quoteId}")]
        public async Task<ActionResult<QuoteDto>> GetQuoteById(string quoteId)
        {
            var quote = await _repo.GetQuoteByIdAsync(quoteId);
            if (quote == null)
                return NotFound(new { message = $"Quote '{quoteId}' not found." });

            return Ok(quote);
        }

        [HttpPost]
        public async Task<ActionResult<QuoteDto>> CreateQuote([FromBody] QuoteDto dto)
        {
            var created = await _repo.CreateQuoteAsync(dto);
            return CreatedAtAction(nameof(GetQuoteById), new { quoteId = created.QuoteId }, created);
        }
    }
}
