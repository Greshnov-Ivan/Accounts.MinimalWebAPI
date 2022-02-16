namespace Accounts.MinimalWebAPI
{
    public class Account
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Owner
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Account type
        /// </summary>
        public int AccountType { get; set; }
    }
}
