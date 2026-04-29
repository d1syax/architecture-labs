using Microsoft.EntityFrameworkCore;
using MyBank.Infrastructure.Persistence.Entities;

namespace MyBank.Infrastructure.Persistence;

public class BankDbContext : DbContext
{
    public BankDbContext(DbContextOptions<BankDbContext> options) : base(options) { }

    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<AccountEntity> Accounts => Set<AccountEntity>();
    public DbSet<TransactionEntity> Transactions => Set<TransactionEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<AccountEntity>().HasIndex(a => a.AccountNumber).IsUnique();
        modelBuilder.Entity<AccountEntity>().Property(a => a.Balance).HasPrecision(18, 2);
        modelBuilder.Entity<TransactionEntity>().Property(t => t.Amount).HasPrecision(18, 2);
    }
}