using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning.Builder;
using CTCore.DynamicQuery.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ZEN.Application.Usecases.WorkExperienceUC.Commands;
using ZEN.Application.Usecases.WorkExperienceUC.Queries;
using ZEN.Controller.Extensions;
using ZEN.Domain.DTO.WorkExperienceDto.Request;

namespace ZEN.Controller.Endpoints.V1
{
    public class WorkExpEndpoint : IEndpoint
    {
        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints, ApiVersionSet version)
        {
            var co = endpoints
               .MapGroup($"{EndpointExntensions.BASE_ROUTE}/workexp")
               .WithDisplayName("WorkExperience")
               .WithApiVersionSet(version)
               .HasApiVersion(1);


            co.MapPost("/", AddWorkExperience).RequireAuthorization();
            co.MapPatch("/{we_id}", UpdateWorkExperience).RequireAuthorization();
            co.MapGet("/", GetAllWorkExperience).RequireAuthorization();
            co.MapDelete("/{we_id}", DeleteWorkExperience).RequireAuthorization();
            return endpoints;
        }
        private async Task<IResult> AddWorkExperience(
            [FromServices] IMediator mediator,
            [FromBody] ReqWorkExperienceDto arg
        )
        {
            try
            {
                return (await mediator.Send(new CreateWorkExperienceCommand(arg))).ToOk(e => Results.Ok(e));
            }
            catch (NotFoundException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
            catch (BadHttpRequestException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
        }

        private async Task<IResult> UpdateWorkExperience(
            [FromServices] IMediator mediator,
            [FromBody] ReqWorkExperienceDto arg,
            [FromRoute] string we_id
        )
        {
            try
            {
                return (await mediator.Send(new UpdateWorkExperienceCommand(arg, we_id))).ToOk(e => Results.Ok(e));
            }
            catch (NotFoundException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
        }
        private async Task<IResult> GetAllWorkExperience(
            [FromServices] IMediator mediator,
            [FromQuery] int page_index,
            [FromQuery] int page_size
        )
        {
            try
            {
                return (await mediator.Send(new GetAllWorkExperieceQuery(page_index, page_size))).ToOk(e => Results.Ok(e));
            }
            catch (NotFoundException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
            catch (BadHttpRequestException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
        }
        private async Task<IResult> DeleteWorkExperience(
            [FromServices] IMediator mediator,
            [FromRoute] string we_id
        )
        {
            try
            {
                return (await mediator.Send(new DeleteWorkExperienceCommand(we_id))).ToOk(e => Results.Ok(e));
            }
            catch (NotFoundException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
        }
    }
}