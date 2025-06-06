// using Asp.Versioning.Builder;
// using CTCore.DynamicQuery;
// using MediatR;
// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Routing;
// using ZEN.Application.Usecases.PositionUC.Queries;
// using ZEN.Contract.PositionDto;
// using ZEN.Controller.Extensions;

// namespace ZEN.Controller.Endpoints.V1;


// public class PositionEndpoint : IEndpoint
// {
//     public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints, ApiVersionSet version)
//     {
//         var co = endpoints
//             .MapGroup($"{EndpointExntensions.BASE_ROUTE}/positions")
//             .WithDisplayName("Positions")
//             .WithApiVersionSet(version)
//             .HasApiVersion(1);
//         // .RequireAuthorization();

//         // co.MapPost("/", CreatePositionAsync);
//         // co.MapPatch("/{id}", UpdatePositionAsync);
//         // co.MapDelete("/{id}", DeletePositionAsync;
//         co.MapGet("/", AllPositionAsync);
//         // co.MapGet("/{id}", GetPositionAsync);

//         return endpoints;
//     }
//     private async Task<IResult> AllPositionAsync (
//         [FromServices] IMediator mediator,
//         BaseAPIPageRequest request
//     ) => (await mediator.Send(new GetAllPositionQuery(request.ToQueryContext())))
//             .ToOk(e => Results.Ok(e));

//     // private async Task<IResult> DeletePositionAsync(
//     //     [FromServices] IMediator mediator,
//     //     string id
//     // ) => (await mediator.Send(new DeletePositionCommand(id))).ToOk(e => Results.Ok(e));

//     // private async Task<IResult> UpdatePositionAsync(
//     //     [FromServices] IMediator mediator,
//     //     string id,
//     //     [FromBody] UpdatePositionArg arg
//     // ) => (await mediator.Send(new UpdatePositionCommand(id, arg))).ToOk(e => Results.Ok(e));

//     // private async Task<IResult> CreatePositionAsync(
//     //     [FromServices] IMediator mediator,
//     //     CreatePositionArg arg
//     // ) => (await mediator.Send(new CreatePositionCommand(arg))).ToOk(e => Results.Ok(e));

//     // private async Task<IResult> AllPositionAsync (
//     //     [FromServices] IMediator mediator,
//     //     BaseAPIPageRequest request
//     // ) => (await mediator.Send(new GetAllPositionQuery(request.ToQueryContext())))
//     //         .ToOk(e => Results.Ok(e));

//     // private async Task<IResult> GetPositionAsync (
//     //     [FromServices] IMediator mediator,
//     //     string id,
//     //     BaseAPIRequest request

//     // ) => (await mediator.Send(new GetPositionQuery(id, request.ToQueryContext())))
//     //         .ToOk(e => Results.Ok(e));
// }

