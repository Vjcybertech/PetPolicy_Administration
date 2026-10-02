using Microsoft.EntityFrameworkCore;
using PolicyAdmin.API.Data;
using PolicyAdmin.API.DTOs;
using PolicyAdmin.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolicyAdmin.API.Services
{
    public class QuoteService : IQuoteService
    {
        private readonly PolicyDbContext _context;

        public QuoteService(PolicyDbContext context)
        {
            _context = context;
        }

        public async Task<List<QuoteDto>> GetAllQuotesAsync(string? status = null)
        {
            var query = _context.Quotes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(q => q.Status.ToLower() == status.ToLower());
            }

            var quotes = await query.ToListAsync();
            return quotes.Select(MapToDto).ToList();
        }

        public async Task<QuoteDto?> GetQuoteByIdAsync(string quoteId)
        {
            // LINQ Demonstration: SingleOrDefault
            var quote = await _context.Quotes.SingleOrDefaultAsync(q => q.QuoteId == quoteId);
            return quote == null ? null : MapToDto(quote);
        }

        public async Task<QuoteDto> CreateQuoteAsync(QuoteDto dto)
        {
            var entity = new Quote
            {
                QuoteId = string.IsNullOrWhiteSpace(dto.QuoteId) ? $"QT-{System.Random.Shared.Next(1000, 9999)}" : dto.QuoteId,
                CustomerId = dto.CustomerId,
                CustomerName = dto.CustomerName,
                CustomerEmail = dto.CustomerEmail,
                CustomerPhone = dto.CustomerPhone,
                PetId = dto.PetId,
                PetName = dto.PetName,
                PetType = dto.PetType,
                Breed = dto.Breed,
                Age = dto.Age,
                AnnualPremium = dto.AnnualPremium,
                PaymentFrequency = dto.PaymentFrequency,
                Status = "Approved",
                CoverageType = dto.CoverageType,
                AnnualLimit = dto.AnnualLimit,
                Deductible = dto.Deductible,
                ReimbursementPct = dto.ReimbursementPct
            };

            _context.Quotes.Add(entity);
            await _context.SaveChangesAsync();
            return MapToDto(entity);
        }

        private static QuoteDto MapToDto(Quote q) => new()
        {
            QuoteId = q.QuoteId,
            CustomerId = q.CustomerId,
            CustomerName = q.CustomerName,
            CustomerEmail = q.CustomerEmail,
            CustomerPhone = q.CustomerPhone,
            PetId = q.PetId,
            PetName = q.PetName,
            PetType = q.PetType,
            Breed = q.Breed,
            Age = q.Age,
            AnnualPremium = q.AnnualPremium,
            PaymentFrequency = q.PaymentFrequency,
            Status = q.Status,
            CoverageType = q.CoverageType,
            AnnualLimit = q.AnnualLimit,
            Deductible = q.Deductible,
            ReimbursementPct = q.ReimbursementPct
        };
    }
}
