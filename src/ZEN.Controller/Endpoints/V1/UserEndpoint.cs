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
using ZEN.Application.Usecases.UserUC.Commands;
using ZEN.Contract.AspAccountDto;
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

            co.MapPatch("/{user_id}", UpdateProfile).RequireAuthorization();
            return endpoints;
        }

        private async Task<IResult> UpdateProfile(
            [FromServices] IMediator mediator,
            [FromRoute] string user_id,
            [FromBody] ReqUserDto arg)
        {
            try
            {
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
        }
    }
}