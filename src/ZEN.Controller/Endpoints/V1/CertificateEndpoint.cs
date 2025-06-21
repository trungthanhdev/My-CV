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


            co.MapPost("/", AddCertificate).RequireAuthorization();
            co.MapPatch("/{certificate_id}", UpdateCertificate).RequireAuthorization();
            co.MapDelete("/{certificate_id}", DeleteCertificate).RequireAuthorization();
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
        private async Task<IResult> UpdateCertificate(
            [FromServices] IMediator mediator,
            [FromBody] ReqCertificateDto arg,
            [FromRoute] string certificate_id)
        {
            try
            {
                return (await mediator.Send(new UpdateCertificateCommand(certificate_id, arg))).ToOk(e => Results.Ok(e));
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
        private async Task<IResult> DeleteCertificate(
            [FromServices] IMediator mediator,
            [FromRoute] string certificate_id)
        {
            try
            {
                return (await mediator.Send(new DeleteCertificateCommand(certificate_id))).ToOk(e => Results.Ok(e));
            }
            catch (NotFoundException ex)
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