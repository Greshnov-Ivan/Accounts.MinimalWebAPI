using Accounts.MinimalWebAPI.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounts.MinimalWebAPI.Infrastructure
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        /// <summary>
        /// Конфигурация для типа сущности
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(account => account.Id);
            builder.HasIndex(account => account.Id).IsUnique();
            builder.Property(account => account.AccountType).HasMaxLength(32);
        }
    }
}
