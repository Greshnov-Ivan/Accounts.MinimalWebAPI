using Accounts.MinimalWebAPI;
using Accounts.MinimalWebAPI.Domain;
using Accounts.MinimalWebAPI.Infrastructure;
using Accounts.MinimalWebAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<IAccountsDbContext, AccountsDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Npgsql"));
});
builder.Services.Configure<KafkaConsumerConfig>(builder.Configuration.GetSection("KafkaConsumerConfigure"));
builder.Services.AddTransient<IAccountsService, AccountsService>();
builder.Services.AddHostedService<KafkaConsumerService>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "I'am Accounts API!")
    .ExcludeFromDescription();
app.MapGet("/api/accounts", async (IAccountsService accountService) => await accountService.Get(new CancellationToken()))
    .Produces<List<Account>>(StatusCodes.Status200OK)
    .WithName("GetAllAccounts")
    .WithTags("Getters");
app.MapGet("/api/accountsdef/{id}", async (IAccountsService accountService, Guid id) =>
{
    var user = await accountService.Get(id, new CancellationToken());
    return user is null ? Results.NotFound() : Results.Ok(user);
})
    .ExcludeFromDescription();
app.MapGet("/api/accounts/{userId}", async (IAccountsService accountService, string userId) =>
{
    var user = await accountService.GetByUser(userId, new CancellationToken());
    return user is null ? Results.NotFound() : Results.Ok(user);
})
    .Produces<Account>(StatusCodes.Status200OK)
    .WithName("GetAccountByUser")
    .WithTags("Getters");
app.MapPost("/api/accounts", async (IAccountsService accountService, string userId) =>
{
    var accauntId = await accountService.Create(userId, new CancellationToken());
    return Results.Created($"/api/accounts/", accauntId);
})
    .Accepts<Account>("application/json")
    .Produces<Account>(StatusCodes.Status201Created)
    .WithName("CreateAccount")
    .WithTags("Creators");
app.MapPut("/api/accounts/", async (IAccountsService accountService, Account updateAccount) =>
{
    await accountService.Update(updateAccount, new CancellationToken());
    return Results.Ok();
})
    .Accepts<Account>("application/json")
    .WithName("UpdateAccount")
    .WithTags("Updaters");
app.MapDelete("/api/accounts/{userId}", async (IAccountsService accountService, string userId) =>
{
    await accountService.Delete(userId, new CancellationToken());
    return Results.NoContent();
})
    .Produces(StatusCodes.Status204NoContent)
    .WithName("DeleteAccount")
    .WithTags("Deleters");
app.MapDelete("/api/accounts", async (IAccountsService accountService) =>
{
    await accountService.DeleteAll(new CancellationToken());
    return Results.NoContent();
})
    .Produces(StatusCodes.Status204NoContent)
    .WithName("DeleteAllAccount")
    .WithTags("Deleters");

app.UseHttpsRedirection();

app.Run();
