using Microsoft.EntityFrameworkCore;

namespace Accounts.MinimalWebAPI
{
    public interface IAccountsDbContext
    {
        /// <summary>
        /// Коллекция всех сущностей в контексте
        /// </summary>
        DbSet<Account> Accounts { get; set; }
        /// <summary>
        /// Сохранить изменение контекста в БД
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
