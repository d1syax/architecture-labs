using MediatR;
using MyBank.Application.ReadModels;

namespace MyBank.Application.Accounts.Queries;

public class xGetMyAccountsQueryHandler : IRequestHandler<GetMyAccountsQuery, List<AccountReadModel>>
{
    private readonly IAccountReadRepository _readRepository;

    public GetMyAccountsQueryHandler(IAccountReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<List<AccountReadModel>> Handle(GetMyAccountsQuery query, CancellationToken cancellationToken)
    {
        return await _readRepository.GetByUserIdAsync(query.UserId);
    }
}