using Microsoft.EntityFrameworkCore;
using PolicyAdmin.API.Models;

namespace PolicyAdmin.API.Data
{
    public class PolicyDbContext : DbContext
    {
        public PolicyDbContext(DbContextOptions<PolicyDbContext> options) : base(options)
        {
        }

        public DbSet<Quote> Quotes => Set<Quote>();
        public DbSet<Policy> Policies => Set<Policy>();
        public DbSet<PolicyCoverage> PolicyCoverages => Set<PolicyCoverage>();
        public DbSet<PolicyTransaction> PolicyTransactions => Set<PolicyTransaction>();
        public DbSet<PolicyRenewal> PolicyRenewals => Set<PolicyRenewal>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Policy>(entity =>
            {
                entity.HasKey(p => p.PolicyId);
                entity.HasIndex(p => p.PolicyNumber).IsUnique();
                entity.Property(p => p.Premium).HasPrecision(18, 2);

                entity.HasMany(p => p.PolicyCoverages)
                      .WithOne(c => c.Policy)
                      .HasForeignKey(c => c.PolicyId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(p => p.PolicyTransactions)
                      .WithOne(t => t.Policy)
                      .HasForeignKey(t => t.PolicyId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(p => p.PolicyRenewals)
                      .WithOne(r => r.Policy)
                      .HasForeignKey(r => r.PolicyId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PolicyCoverage>(entity =>
            {
                entity.HasKey(c => c.PolicyCoverageId);
                entity.Property(c => c.AnnualLimit).HasPrecision(18, 2);
                entity.Property(c => c.Deductible).HasPrecision(18, 2);
                entity.Property(c => c.ReimbursementPct).HasPrecision(5, 2);
            });

            modelBuilder.Entity<PolicyRenewal>(entity =>
            {
                entity.HasKey(r => r.RenewalId);
                entity.Property(r => r.PreviousPremium).HasPrecision(18, 2);
                entity.Property(r => r.NewPremium).HasPrecision(18, 2);
            });

            modelBuilder.Entity<PolicyTransaction>(entity =>
            {
                entity.HasKey(t => t.TransactionId);
            });

            modelBuilder.Entity<Quote>(entity =>
            {
                entity.HasKey(q => q.QuoteId);
                entity.Property(q => q.AnnualPremium).HasPrecision(18, 2);
                entity.Property(q => q.AnnualLimit).HasPrecision(18, 2);
                entity.Property(q => q.Deductible).HasPrecision(18, 2);
                entity.Property(q => q.ReimbursementPct).HasPrecision(5, 2);
            });
        }
    }
}
