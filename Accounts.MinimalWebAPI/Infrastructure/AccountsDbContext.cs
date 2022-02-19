using Accounts.MinimalWebAPI.Domain;
using Microsoft.EntityFrameworkCore;

namespace Accounts.MinimalWebAPI.Infrastructure
{
    public class AccountsDbContext : DbContext, IAccountsDbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public AccountsDbContext(DbContextOptions<AccountsDbContext> options)
            : base(options) { }
    }
}
