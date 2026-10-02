using PolicyAdmin.API.DTOs;
using PolicyAdmin.API.Services;


namespace PolicyAdmin.API.Repository
{
    class QuoteRepository : IQuoteRepository
    {
        private readonly IQuoteService _service;

        public QuoteRepository(IQuoteService quoteService)
        {
            _service = quoteService;
        }
        public Task<QuoteDto> CreateQuoteAsync(QuoteDto quoteDto)
        {
            return _service.CreateQuoteAsync(quoteDto);
        }

        public Task<List<QuoteDto>> GetAllQuotesAsync(string? status = null)
        {
            return _service.GetAllQuotesAsync(status);
        }

        public Task<QuoteDto?> GetQuoteByIdAsync(string quoteId)
        {
            return _service.GetQuoteByIdAsync(quoteId);
        }
    }
}