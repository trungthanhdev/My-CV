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
using ZEN.Application.Usecases.ProjectUC.Commands;
using ZEN.Application.Usecases.ProjectUC.Query;
using ZEN.Contract.ProjectDto.Request;
using ZEN.Controller.Extensions;

namespace ZEN.Controller.Endpoints.V1
{
    public class ProjectEndpoint
    {
        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints, ApiVersionSet version)
        {
            var co = endpoints
                .MapGroup($"{EndpointExntensions.BASE_ROUTE}/project")
                .WithDisplayName("Projects")
                .WithApiVersionSet(version)
                .HasApiVersion(1);

            co.MapGet("/", GetProject).RequireAuthorization();
            co.MapPost("/create-project", CreateProject).RequireAuthorization().DisableAntiforgery();
            co.MapPatch("/{project_id}", UpdateProject).RequireAuthorization().DisableAntiforgery();
            co.MapDelete("/{project_id}", DeleteProject).RequireAuthorization();
            return endpoints;
        }

        private async Task<IResult> CreateProject(
            [FromServices] IMediator mediator,
            [FromForm] ReqCreateProjectDto arg
        )
        {
            try
            {
                return (await mediator.Send(new CreateProjectCommand(arg))).ToOk(e => Results.Ok(e));
            }
            catch (ArgumentNullException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
            catch (BadHttpRequestException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
            catch
            {
                return Results.Problem("Internal Server Error", statusCode: 500);
            }
        }

        private async Task<IResult> UpdateProject(
            [FromServices] IMediator mediator,
            [FromRoute] string project_id,
            [FromForm] ReqUpdateProjectDto arg
        )
        {
            try
            {
                return (await mediator.Send(new UpdateProjectCommand(project_id, arg))).ToOk(e => Results.Ok(e));
            }
            catch (NotFoundException ex)
            {
                return Results.Problem(ex.Message, statusCode: 404);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Problem(ex.Message, statusCode: 401);
            }
            catch (ArgumentNullException ex)
            {
                return Results.Problem(ex.Message, statusCode: 401);
            }
        }

        private async Task<IResult> DeleteProject(
            [FromServices] IMediator mediator,
            [FromRoute] string project_id
        )
        {
            try
            {
                return (await mediator.Send(new DeleteProjectCommand(project_id))).ToOk(e => Results.Ok(e));
            }
            catch (NotFoundException ex)
            {
                return Results.Problem(ex.Message, statusCode: 404);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Problem(ex.Message, statusCode: 401);
            }
        }

        private async Task<IResult> GetProject(
            [FromServices] IMediator mediator,
            [FromQuery] int page_index,
            [FromQuery] int page_size
        )
        {
            try
            {
                return (await mediator.Send(new GetProjectQuery(page_index, page_size))).ToOk(e => Results.Ok(e));
            }
            catch (BadHttpRequestException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
        }
    }
}