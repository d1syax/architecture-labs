using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyBank.Infrastructure.Persistence;
using Xunit;

namespace MyBank.Tests.IntegrationTests;

public class AccountsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly string _dbName = Guid.NewGuid().ToString();

    public AccountsIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptors = services
                    .Where(d =>
                        d.ServiceType.FullName != null && (
                            d.ServiceType.FullName.Contains("DbContext") ||
                            d.ServiceType.FullName.Contains("Npgsql") ||
                            d.ServiceType.FullName.Contains("EntityFramework")))
                    .ToList();
                foreach (var d in descriptors) services.Remove(d);

                services.AddDbContext<BankDbContext>(options =>
                    options.UseInMemoryDatabase(_dbName)
                        .ConfigureWarnings(w => w.Ignore(
                            Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId
                                .TransactionIgnoredWarning)));
            });
        }).CreateClient();
    }

    private async Task<string> RegisterAndLoginAsync()
    {
        var email = $"test_{Guid.NewGuid()}@example.com";
        var response = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            email,
            password = "password123",
            fullName = "Test User"
        });
        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>();
        return body!["token"].GetString()!;
    }

    private async Task<int> CreateAccountAsync(string token, string currency = "USD")
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJsonAsync("/api/accounts", new { currency });
        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>();
        return body!["id"].GetInt32();
    }

    [Fact]
    public async Task GetMyAccounts_ReturnsEmptyList()
    {
        var token = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/accounts");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var accounts = await response.Content
            .ReadFromJsonAsync<List<Dictionary<string, object>>>();
        Assert.Empty(accounts!);
    }

    [Fact]
    public async Task GetMyAccounts_WithoutToken_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.GetAsync("/api/accounts");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMyAccounts_AfterCreating_ReturnsAccounts()
    {
        var token = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        await _client.PostAsJsonAsync("/api/accounts", new { currency = "USD" });
        await _client.PostAsJsonAsync("/api/accounts", new { currency = "EUR" });

        var response = await _client.GetAsync("/api/accounts");
        var accounts = await response.Content
            .ReadFromJsonAsync<List<Dictionary<string, object>>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, accounts!.Count);
    }

    [Fact]
    public async Task CreateAccount_ValidCurrency_Returns201()
    {
        var token = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync("/api/accounts", new { currency = "USD" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateAccount_UAH_Returns201()
    {
        var token = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync("/api/accounts", new { currency = "UAH" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateAccount_EUR_Returns201()
    {
        var token = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync("/api/accounts", new { currency = "EUR" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateAccount_InvalidCurrency_Returns400()
    {
        var token = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync("/api/accounts", new { currency = "XYZ" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateAccount_WithoutToken_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.PostAsJsonAsync("/api/accounts", new { currency = "USD" });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Deposit_ValidAmount_Returns200()
    {
        var token = await RegisterAndLoginAsync();
        var accountId = await CreateAccountAsync(token);

        var response = await _client.PostAsJsonAsync(
            $"/api/accounts/{accountId}/deposit", new { amount = 500 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Deposit_NegativeAmount_Returns400()
    {
        var token = await RegisterAndLoginAsync();
        var accountId = await CreateAccountAsync(token);

        var response = await _client.PostAsJsonAsync(
            $"/api/accounts/{accountId}/deposit", new { amount = -100 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Deposit_AccountNotFound_Returns404()
    {
        var token = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync(
            "/api/accounts/99999/deposit", new { amount = 100 });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Deposit_WithoutToken_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.PostAsJsonAsync(
            "/api/accounts/1/deposit", new { amount = 100 });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task Transfer_SameAccount_Returns400()
    {
        var token = await RegisterAndLoginAsync();
        var accountId = await CreateAccountAsync(token);

        var response = await _client.PostAsJsonAsync("/api/accounts/transfer", new
        {
            fromAccountId = accountId,
            toAccountId = accountId,
            amount = 100
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Transfer_InsufficientFunds_Returns400()
    {
        var token = await RegisterAndLoginAsync();
        var fromId = await CreateAccountAsync(token, "USD");

        var token2 = await RegisterAndLoginAsync();
        var toId = await CreateAccountAsync(token2, "USD");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync("/api/accounts/transfer", new
        {
            fromAccountId = fromId,
            toAccountId = toId,
            amount = 1000
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Transfer_WithoutToken_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.PostAsJsonAsync("/api/accounts/transfer", new
        {
            fromAccountId = 1,
            toAccountId = 2,
            amount = 100
        });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}