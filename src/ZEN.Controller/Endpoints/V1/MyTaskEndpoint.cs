using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning.Builder;
using CTCore.DynamicQuery.Common.Exceptions;
using DocumentFormat.OpenXml.Drawing.Charts;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ZEN.Application.Usecases.MyTaskUC.Commands;
using ZEN.Contract.MyTaskDto.Request;
using ZEN.Controller.Extensions;

namespace ZEN.Controller.Endpoints.V1
{
    public class MyTaskEndpoint
    {
        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints, ApiVersionSet version)
        {
            var co = endpoints
               .MapGroup($"{EndpointExntensions.BASE_ROUTE}/mytask")
               .WithDisplayName("MyTasks")
               .WithApiVersionSet(version)
               .HasApiVersion(1);

            co.MapPost("/{we_id}", AddMyTask).RequireAuthorization();
            co.MapPatch("update/{we_id}", UpdateMyTask).RequireAuthorization();
            co.MapDelete("/{mt_id}", DeleteMyTask).RequireAuthorization();
            return endpoints;
        }

        private async Task<IResult> AddMyTask(
            [FromServices] IMediator mediator,
            [FromRoute] string we_id,
            [FromBody] ReqMyTaskDto arg
        )
        {
            try
            {
                return (await mediator.Send(new AddMyTaskCommand(we_id, arg))).ToOk(e => Results.Ok(e));
            }
            catch (NotFoundException ex)
            {
                return Results.Problem(ex.Message, statusCode: 404);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Problem(ex.Message, statusCode: 401);
            }
            catch (BadHttpRequestException ex)
            {
                return Results.Problem(ex.Message, statusCode: 401);
            }
        }
        private async Task<IResult> UpdateMyTask(
            [FromServices] IMediator mediator,
            [FromRoute] string we_id,
            [FromBody] ReqMyTaskDto arg
        )
        {
            try
            {
                return (await mediator.Send(new UpdateMyTaskCommand(we_id, arg))).ToOk(e => Results.Ok(e));
            }
            catch (NotFoundException ex)
            {
                return Results.Problem(ex.Message, statusCode: 404);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Problem(ex.Message, statusCode: 401);
            }
            catch (BadHttpRequestException ex)
            {
                return Results.Problem(ex.Message, statusCode: 401);
            }
        }

        private async Task<IResult> DeleteMyTask(
            [FromServices] IMediator mediator,
            [FromRoute] string mt_id
        )
        {
            try
            {
                return (await mediator.Send(new DeleteMyTaskCommand(mt_id))).ToOk(e => Results.Ok(e));
            }
            catch (NotFoundException ex)
            {
                return Results.Problem(ex.Message, statusCode: 404);
            }
        }
    }
}