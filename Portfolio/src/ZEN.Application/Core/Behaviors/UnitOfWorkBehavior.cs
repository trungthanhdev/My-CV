using CTCore.DynamicQuery.Core.Domain.Interfaces;
using MediatR;

namespace ZEN.Application.Core.Behaviors;

public class UnitOfWorkBehavior<TTRequest, TTResponse>(IEnumerable<IUnitOfWork> unitOfWorks)
        : IPipelineBehavior<TTRequest, TTResponse>
        where TTRequest : notnull
        where TTResponse : notnull
{
    public async Task<TTResponse> Handle(TTRequest request,
        RequestHandlerDelegate<TTResponse> next,
        CancellationToken cancellationToken)
    {
        // var espUnitOfWork = unitOfWorks.OfType<UnitOfWork<ESPDbContext>>().FirstOrDefault();
        // if (espUnitOfWork != null)
        // {
        //     if (IsCommand)
        //     {
        //         espUnitOfWork.DisableTransaction();
        //     }
        // }

        // IUnitOfWork UnitOfWork = unitOfWorks.OfType<UnitOfWork<AppDbContext>>().First();
        IUnitOfWork UnitOfWork = unitOfWorks.First();
        if (IsCommand)
            await UnitOfWork.BeginTransactionAsync(cancellationToken);

        var response = await next();

        if (!IsCommand)
            return response;

        try
        {
            await UnitOfWork.CommitAsync(cancellationToken);
            return response;
        }
        catch
        {
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static bool IsCommand => typeof(TTRequest).Name.EndsWith("Command");
}