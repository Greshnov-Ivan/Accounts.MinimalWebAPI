using Microsoft.EntityFrameworkCore;

namespace Accounts.MinimalWebAPI
{
    public class AccountsDbContext : DbContext, IAccountsDbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public AccountsDbContext(DbContextOptions<AccountsDbContext> options)
            : base(options) { }
    }
}
