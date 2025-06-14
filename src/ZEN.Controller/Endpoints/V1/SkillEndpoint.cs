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
using ZEN.Application.Usecases.SkillUC.Commands;
using ZEN.Application.Usecases.SkillUC.Queries;
using ZEN.Contract.SkillDto.Request;
using ZEN.Controller.Extensions;

namespace ZEN.Controller.Endpoints.V1
{
    public class SkillEndpoint : IEndpoint
    {
        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints, ApiVersionSet version)
        {
            var co = endpoints
               .MapGroup($"{EndpointExntensions.BASE_ROUTE}/skill")
               .WithDisplayName("Skill")
               .WithApiVersionSet(version)
               .HasApiVersion(1);

            co.MapPost("/add-skill", AddSkill).RequireAuthorization();
            co.MapGet("/", GetAllSkill).RequireAuthorization();
            co.MapPatch("/{skill_id}", UpdateSkill).RequireAuthorization();
            co.MapDelete("/", DeleteSkill).RequireAuthorization();

            return endpoints;
        }

        private async Task<IResult> AddSkill(
            [FromServices] IMediator mediator,
            [FromBody] ReqSkillDto arg
            )
        {
            try
            {
                return (await mediator.Send(new CreateSkillCommand(arg))).ToOk(e => Results.Ok(e));
            }
            catch (NotFoundException ex)
            {
                return Results.Problem(ex.Message, statusCode: 404);
            }
            catch (BadHttpRequestException ex)
            {
                return Results.Problem(ex.Message, statusCode: 404);
            }
        }
        private async Task<IResult> GetAllSkill(
            [FromServices] IMediator mediator
            )
        {
            try
            {
                return (await mediator.Send(new GetAllSkillQueries())).ToOk(e => Results.Ok(e));
            }
            catch (NotFoundException ex)
            {
                return Results.Problem(ex.Message, statusCode: 404);
            }
            catch (BadHttpRequestException ex)
            {
                return Results.Problem(ex.Message, statusCode: 404);
            }
        }
        private async Task<IResult> UpdateSkill(
            [FromServices] IMediator mediator,
            [FromRoute] string skill_id,
            [FromBody] ReqSkillDto arg
            )
        {
            try
            {
                return (await mediator.Send(new UpdateSkillCommand(arg, skill_id))).ToOk(e => Results.Ok(e));
            }
            catch (NotFoundException ex)
            {
                return Results.Problem(ex.Message, statusCode: 404);
            }
            catch (BadHttpRequestException ex)
            {
                return Results.Problem(ex.Message, statusCode: 404);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Problem(ex.Message, statusCode: 404);
            }
        }
        private async Task<IResult> DeleteSkill(
           [FromServices] IMediator mediator
           )
        {
            try
            {
                return (await mediator.Send(new DeleteSkillCommand())).ToOk(e => Results.Ok(e));
            }
            catch (NotFoundException ex)
            {
                return Results.Problem(ex.Message, statusCode: 404);
            }
            catch (BadHttpRequestException ex)
            {
                return Results.Problem(ex.Message, statusCode: 404);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Problem(ex.Message, statusCode: 404);
            }
        }
    }
}