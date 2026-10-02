
using PolicyAdmin.API.DTOs;

namespace PolicyAdmin.API.Repository
{
    public interface IQuoteRepository
    {
         Task<List<QuoteDto>> GetAllQuotesAsync(string? status = null);
        Task<QuoteDto?> GetQuoteByIdAsync(string quoteId);
        Task<QuoteDto> CreateQuoteAsync(QuoteDto quoteDto);
        
    }
}