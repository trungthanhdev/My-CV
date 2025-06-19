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
using ZEN.Application.Usecases.CertificateUC.Commands;
using ZEN.Contract.CertificateDto.Request;
using ZEN.Controller.Extensions;

namespace ZEN.Controller.Endpoints.V1
{
    public class CertificateEndpoint
    {
        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints, ApiVersionSet version)
        {
            var co = endpoints
                    .MapGroup($"{EndpointExntensions.BASE_ROUTE}/certificate")
                    .WithDisplayName("Certificates")
                    .WithApiVersionSet(version)
                    .HasApiVersion(1);


            co.MapPost("/", AddCertificate).RequireAuthorization().DisableAntiforgery();
            return endpoints;
        }

        private async Task<IResult> AddCertificate(
            [FromServices] IMediator mediator,
            [FromBody] ReqCertificateDto arg
            )
        {
            try
            {
                return (await mediator.Send(new AddCertificateCommand(arg))).ToOk(e => Results.Ok(e));
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
    }
}