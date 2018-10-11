using Microsoft.EntityFrameworkCore;
using MyLittleBank.Entities;

namespace MyLittleBank.Dal
{
    public class MyLittleBankDbContext : DbContext
    {
        public MyLittleBankDbContext()
        { }

        public MyLittleBankDbContext(DbContextOptions<MyLittleBankDbContext> options) : base(options)
        { }

        public DbSet<BankAccount> BankAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BankAccount>(entity =>
            {
                entity.HasData(
                    new BankAccount
                    {
                        Id = 1,
                        Balance = 102.45M,
                        Number = "PBO1221323",
                        IsLocked = false
                    },
                    new BankAccount
                    {
                        Id = 2,
                        Balance = 10002.98M,
                        Number = "PIC9984567",
                        IsLocked = false
                    },
                    new BankAccount
                    {
                        Id = 3,
                        Balance = 677.71M,
                        Number = "PAC0784412",
                        IsLocked = true
                    }
                );
            }
                );
            base.OnModelCreating(modelBuilder);
        }
    }
}
