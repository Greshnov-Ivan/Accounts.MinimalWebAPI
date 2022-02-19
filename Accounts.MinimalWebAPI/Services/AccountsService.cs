using Accounts.MinimalWebAPI.Domain;
using Accounts.MinimalWebAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Accounts.MinimalWebAPI.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly IAccountsDbContext _db;
        private readonly string _connectionString;

        public AccountsService(IAccountsDbContext accountsDbContext,IConfiguration config) => (_db, _connectionString) = (accountsDbContext, config.GetConnectionString("Npgsql"));
        
        public async Task<List<Account>> Get(CancellationToken cancellationToken) => await _db.Accounts.ToListAsync(cancellationToken);

        public async Task<Account> Get(Guid id, CancellationToken cancellationToken)
        {
            var account = await _db.Accounts.FindAsync(new object[] { id }, cancellationToken);

            return account is null 
                ? throw new Exception("Not found")
                : account;
        }

        public async Task<Account> GetByUser(string userId, CancellationToken cancellationToken)
        {
            var account = await _db.Accounts.FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);

            return account is null
                ? throw new Exception("Not found")
                : account;
        }

        public async Task<Guid> Create(string userId, CancellationToken cancellationToken)
        {
            var account = new Account()
            {
                UserId = userId,
                AccountType = (int)AccountTypes.Free
            };

            _db.Accounts.Add(account);
            await _db.SaveChangesAsync(cancellationToken);

            return account.Id;
        }

        public async Task Update(Account updateAccount, CancellationToken cancellationToken)
        {
            var account = await _db.Accounts.FindAsync(new object[] { updateAccount.Id }, cancellationToken);

            if (account is null)
                throw new Exception("Not found");
            else if (account.UserId != updateAccount.UserId)
                throw new Exception("Can't change user");

            account.AccountType = updateAccount.AccountType;

            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(string userId, CancellationToken cancellationToken)
        {
            var account = await _db.Accounts.FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);

            if (account is null)
                throw new Exception("Not found");

            _db.Accounts.Remove(account);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAll(CancellationToken cancellationToken)
        {
            string query = "TRUNCATE \"Accounts\"";
            using NpgsqlConnection myConnect = new NpgsqlConnection(_connectionString);
            {
                myConnect.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myConnect))
                {
                    await myCommand.ExecuteNonQueryAsync(cancellationToken);
                    myConnect.Close();
                }
            }

            //var allData = await _db.Accounts.ToListAsync(cancellationToken);
            //_db.Accounts.RemoveRange(allData);

            //await _db.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Types accounts
        /// Просто для быстрого понимания числовых значений
        /// </summary>
        enum AccountTypes
        {
            Free,
            BasePaid,
            Gold
        }
    }
}
