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
using ZEN.Application.Usecases.UserUC.Commands;
using ZEN.Application.Usecases.UserUC.Queries;
using ZEN.Contract.AspAccountDto;
using ZEN.Contract.SkillDto.Request;
using ZEN.Controller.Extensions;

namespace ZEN.Controller.Endpoints.V1
{
    public class UserEndpoint
    {
        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints, ApiVersionSet version)
        {
            var co = endpoints
               .MapGroup($"{EndpointExntensions.BASE_ROUTE}/profile")
               .WithDisplayName("Profile")
               .WithApiVersionSet(version)
               .HasApiVersion(1);

            co.MapPatch("/{user_id}", UpdateProfile).RequireAuthorization().DisableAntiforgery();
            co.MapGet("/", GetProfile).RequireAuthorization();

            return endpoints;
        }

        private async Task<IResult> UpdateProfile(
            [FromServices] IMediator mediator,
            [FromRoute] string user_id,
            [FromForm] ReqUserDto arg)
        {
            try
            {
                double sizeInMB = 0.0;
                if (arg.avatar != null)
                {
                    sizeInMB = arg.avatar.Length / (1024.0 * 1024.0);
                    Console.WriteLine($"==> Avatar Size: {sizeInMB:F2} MB");
                }
                return (await mediator.Send(new UpdateProfileCommand(arg, user_id))).ToOk(e => Results.Ok(e));
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
        private async Task<IResult> GetProfile(
            [FromServices] IMediator mediator)
        {
            try
            {
                return (await mediator.Send(new GetProfileQuery())).ToOk(e => Results.Ok(e));
            }
            catch (NotFoundException ex)
            {
                return Results.Problem(ex.Message, statusCode: 404);
            }
        }
    }
}