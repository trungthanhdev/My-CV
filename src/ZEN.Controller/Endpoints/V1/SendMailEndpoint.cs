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
using ZEN.Application.Usecases.SendMailUC.Commands;
using ZEN.Controller.Extensions;
using ZEN.Domain.Interfaces;

namespace ZEN.Controller.Endpoints.V1
{
    public class SendMailEndpoint : IEndpoint
    {
        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints, ApiVersionSet version)
        {
            var co = endpoints
                    .MapGroup($"{EndpointExntensions.BASE_ROUTE}/email")
                    .WithDisplayName("Emails")
                    .WithApiVersionSet(version)
                    .HasApiVersion(1);


            co.MapPost("/", SendMail).RequireAuthorization().DisableAntiforgery();
            return endpoints;
        }
        private async Task<IResult> SendMail(
            [FromServices] IMediator mediator,
            [FromBody] HrContactDto arg
        )
        {
            try
            {
                return (await mediator.Send(new SendMailCommand(arg))).ToOk(e => Results.Ok(e));
            }
            catch (ArgumentNullException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
            catch (NotFoundException ex)
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