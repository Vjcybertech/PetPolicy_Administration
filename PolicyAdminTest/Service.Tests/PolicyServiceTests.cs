using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PolicyAdmin.API.Data;
using PolicyAdmin.API.DTOs;
using PolicyAdmin.API.Models;
using PolicyAdmin.API.Services;

namespace PolicyAdmin.Tests.Services
{
    [TestFixture]
    public class PolicyServiceTests
    {
        private PolicyDbContext _context;
        private PolicyService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PolicyDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new PolicyDbContext(options);   
            _service = new PolicyService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void CreatePolicy_QuoteIdRequired()
        {
            var dto = new CreatePolicyDto();

            Assert.ThrowsAsync<ArgumentException>(
                async () => await _service.CreatePolicyAsync(dto));
        }

        [Test]
        public void CreatePolicy_DuplicatePolicyNumber_ShouldThrow()
        {
            _context.Quotes.Add(new Quote
            {
                QuoteId = "Q001",
                Status = "Approved"
            });

            _context.Policies.Add(new Policy
            {
                PolicyNumber = "POL001"
            });

            _context.SaveChanges();

            var dto = new CreatePolicyDto
            {
                QuoteId = "Q001",
                PolicyNumber = "POL001",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1)
            };

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _service.CreatePolicyAsync(dto));
        }

        [Test]
        public void CreatePolicy_UnapprovedQuote_ShouldThrow()
        {
            _context.Quotes.Add(new Quote
            {
                QuoteId = "Q002",
                Status = "Pending"
            });

            _context.SaveChanges();

            var dto = new CreatePolicyDto
            {
                QuoteId = "Q002",
                PolicyNumber = "POL100",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1)
            };

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _service.CreatePolicyAsync(dto));
        }

        [Test]
        public void CreatePolicy_EndDateBeforeStartDate_ShouldThrow()
        {
            _context.Quotes.Add(new Quote
            {
                QuoteId = "Q004",
                Status = "Approved"
            });

            _context.SaveChanges();

            var dto = new CreatePolicyDto
            {
                QuoteId = "Q003",
                PolicyNumber = "POL200",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(-1)
            };

            Assert.ThrowsAsync<ArgumentException>(
                async () => await _service.CreatePolicyAsync(dto));
        }

        [Test]
        public async Task CreatePolicy_ShouldSetDefaultOneYearTerm()
        {
            _context.Quotes.Add(new Quote
            {
                QuoteId = "Q004",
                Status = "Approved",
                CustomerId = "C001",
                CustomerName = "Test Customer",
                CustomerEmail = "test@test.com",
                CustomerPhone = "9999999999",
                PetId = "P001",
                PetName = "Tommy",
                PetType = "Dog",
                Breed = "Labrador",
                Age = 4,
                AnnualPremium = 1000,
                AnnualLimit = 10000,
                Deductible = 250,
                ReimbursementPct = 80
            });

            _context.SaveChanges();

            var dto = new CreatePolicyDto
            {
                QuoteId = "Q004",
                PolicyNumber = "POL300",
                StartDate = DateTime.Today
                // EndDate intentionally omitted
            };

            var result = await _service.CreatePolicyAsync(dto);

            Assert.That(
                result.EndDate,
                Is.EqualTo(result.StartDate.AddYears(1)));
        }

        [Test]
        public void RenewPolicy_EarlyRenewal_ShouldThrow()
        {
            _context.Policies.Add(new Policy
            {
                PolicyNumber = "POL400",
                StartDate = DateTime.Today.AddMonths(-2),
                EndDate = DateTime.Today.AddDays(120),
                Status = "Active"
            });

            _context.SaveChanges();

            var dto = new RenewPolicyDto
            {
                ProposedPremium = 1500
            };

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _service.RenewPolicyAsync("POL400", dto));
        }

        [Test]
        public void RenewPolicy_CancelledPolicy_ShouldThrow()
        {
            _context.Policies.Add(new Policy
            {
                PolicyNumber = "POL500",
                StartDate = DateTime.Today.AddMonths(-2),
                EndDate = DateTime.Today.AddDays(10),
                Status = "Cancelled"
            });

            _context.SaveChanges();

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _service.RenewPolicyAsync(
                    "POL500",
                    new RenewPolicyDto()));
        }

        [Test]
        public void RenewPolicy_ExpiredPolicy_ShouldThrow()
        {
            _context.Policies.Add(new Policy
            {
                PolicyNumber = "POL600",
                StartDate = DateTime.Today.AddYears(-2),
                EndDate = DateTime.Today.AddDays(-10),
                Status = "Expired"
            });

            _context.SaveChanges();

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _service.RenewPolicyAsync(
                    "POL600",
                    new RenewPolicyDto()));
        }

        [Test]
        public async Task RenewPolicy_ShouldCreateRenewalRecord()
        {
            var policy = new Policy
            {
                PolicyNumber = "POL700",
                StartDate = DateTime.Today.AddYears(-1),
                EndDate = DateTime.Today.AddDays(10),
                Premium = 1000,
                Status = "Expiring Soon"
            };

            _context.Policies.Add(policy);
            _context.SaveChanges();

            await _service.RenewPolicyAsync(
                "POL700",
                new RenewPolicyDto());

            var updated = await _service.GetPolicyByNumberAsync("POL700");

            Assert.That(updated!.Renewals.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task RenewPolicy_ShouldCreateTransaction()
        {
            var policy = new Policy
            {
                PolicyNumber = "POL800",
                StartDate = DateTime.Today.AddYears(-1),
                EndDate = DateTime.Today.AddDays(5),
                Premium = 1000,
                Status = "Expiring Soon"
            };

            _context.Policies.Add(policy);
            _context.SaveChanges();

            await _service.RenewPolicyAsync(
                "POL800",
                new RenewPolicyDto());

            var updated = await _service.GetPolicyByNumberAsync("POL800");

            Assert.That(
                updated!.Transactions.Any(
                    t => t.TransactionType == "Renewed"),
                Is.True);
        }

        [Test]
        public void CancelPolicy_DateBeforeStartDate_ShouldThrow()
        {
            _context.Policies.Add(new Policy
            {
                PolicyNumber = "POL900",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(6),
                Status = "Active"
            });

            _context.SaveChanges();

            var dto = new CancelPolicyDto
            {
                CancellationDate = DateTime.Today.AddDays(-10),
                Reason = "Customer Request"
            };

            Assert.ThrowsAsync<ArgumentException>(
                async () => await _service.CancelPolicyAsync(
                    "POL900",
                    dto));
        }

        [Test]
        public void CancelPolicy_ReasonRequired_ShouldThrow()
        {
            _context.Policies.Add(new Policy
            {
                PolicyNumber = "POL901",
                StartDate = DateTime.Today.AddDays(-10),
                EndDate = DateTime.Today.AddMonths(6),
                Status = "Active"
            });

            _context.SaveChanges();

            var dto = new CancelPolicyDto
            {
                CancellationDate = DateTime.Today,
                Reason = ""
            };

            Assert.ThrowsAsync<ArgumentException>(
                async () => await _service.CancelPolicyAsync(
                    "POL901",
                    dto));
        }

        [Test]
        public void CancelPolicy_AlreadyCancelled_ShouldThrow()
        {
            _context.Policies.Add(new Policy
            {
                PolicyNumber = "POL902",
                StartDate = DateTime.Today.AddMonths(-1),
                EndDate = DateTime.Today.AddMonths(6),
                Status = "Cancelled"
            });

            _context.SaveChanges();

            var dto = new CancelPolicyDto
            {
                CancellationDate = DateTime.Today,
                Reason = "Duplicate Policy"
            };

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _service.CancelPolicyAsync(
                    "POL902",
                    dto));
        }
    }
}