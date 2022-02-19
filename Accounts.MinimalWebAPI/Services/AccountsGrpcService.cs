using Grpc.Core;

namespace Accounts.MinimalWebAPI.Services
{
    internal sealed class AccountsGrpcService : AccountsGrpc.AccountsGrpcBase
    {
        private readonly IAccountsService _accountsService;
        public AccountsGrpcService(IAccountsService accountsService) => (_accountsService) = (accountsService);
        public override async Task GetListAccounts(GetAccountsRequest request, IServerStreamWriter<AccountResponse> responseStream, ServerCallContext context)
        {
            var cts = new CancellationTokenSource();
            var accounts = await _accountsService.Get(cts.Token);

            foreach (var response in accounts.Select(a => new AccountResponse { Id = a.Id.ToString(), UserId = a.UserId, AccountType = a.AccountType }))
            {
                await responseStream.WriteAsync(response);
            }
        }
    }
}
