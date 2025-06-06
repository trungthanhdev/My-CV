using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace ZEN.Application.Core.Behaviors;

public sealed class ValidationBehaviour<TTRequest, TTResponse>
    : IPipelineBehavior<TTRequest, TTResponse>
        where TTRequest : class, ICommand<TTResponse>
{
    private readonly IEnumerable<IValidator<TTRequest>> _validators;
    public ValidationBehaviour(IEnumerable<IValidator<TTRequest>> validators) => _validators = validators;

    public async Task<TTResponse> Handle(TTRequest request,
        RequestHandlerDelegate<TTResponse> next,
        CancellationToken cancellationToken)
    {
        // var validResult = await request.ValidateAsync(cancellationToken);
        // if (validResult.Errors.Count != 0)
        // {
        //     throw new ValidationException(validResult.Errors);
        // }
        // return await next();

        var context = new ValidationContext<TTRequest>(request);
        List<ValidationFailure> failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }
        return await next();
    }
}