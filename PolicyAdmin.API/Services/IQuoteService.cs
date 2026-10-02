using PolicyAdmin.API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PolicyAdmin.API.Services
{
    public interface IQuoteService
    {
        Task<List<QuoteDto>> GetAllQuotesAsync(string? status = null);
        Task<QuoteDto?> GetQuoteByIdAsync(string quoteId);
        Task<QuoteDto> CreateQuoteAsync(QuoteDto quoteDto);
    }
}
