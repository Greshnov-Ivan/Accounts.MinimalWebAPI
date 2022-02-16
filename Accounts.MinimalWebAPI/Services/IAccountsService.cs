namespace Accounts.MinimalWebAPI.Services
{
    public interface IAccountsService
    {
        public Task<List<Account>> Get(CancellationToken cancellationToken);
        public Task<Account> Get(Guid id, CancellationToken cancellationToken);
        public Task<Account> GetByUser(string userId, CancellationToken cancellationToken);
        public Task<Guid> Create(string userId, CancellationToken cancellationToken);
        public Task Update(Account updateAccount, CancellationToken cancellationToken);
        public Task Delete(string userId, CancellationToken cancellationToken);
        public Task DeleteAll(CancellationToken cancellationToken);
    }
}
