using MediatR;
using MyBank.Application.ReadModels;

namespace MyBank.Application.Accounts.Queries;

public record GetMyAccountsQuery(int UserId) : IRequest<List<AccountReadModel>>;