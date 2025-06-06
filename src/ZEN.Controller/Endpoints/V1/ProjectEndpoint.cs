using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning.Builder;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ZEN.Application.Usecases.ProjectUC.Commands;
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

            co.MapPost("/create-project", CreateProject).RequireAuthorization();
            return endpoints;
        }

        private async Task<IResult> CreateProject(
            [FromServices] IMediator mediator,
            [FromBody] ReqCreateProjectDto arg
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
            catch
            {
                return Results.Problem("Internal Server Error", statusCode: 500);
            }
        }
    }
}