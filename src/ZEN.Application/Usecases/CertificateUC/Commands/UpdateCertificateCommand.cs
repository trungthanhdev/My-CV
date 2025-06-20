using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.CertificateDto.Request;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Definition;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.CertificateUC.Commands
{
    public class UpdateCertificateCommand(string certificate_id, ReqCertificateDto arg) : ICommand<OkResponse>
    {
        public string Certificate_id = certificate_id;
        public ReqCertificateDto Arg = arg;
    }
    public class UpdateCertificateCommandHandler(
        IUnitOfWork unitOfWork,
        AppDbContext dbContext,
        IUserIdentifierProvider provider
    ) : ICommandHandler<UpdateCertificateCommand, OkResponse>
    {
        public async Task<CTBaseResult<OkResponse>> Handle(UpdateCertificateCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == provider.UserId, cancellationToken);

            if (currentUser == null) throw new NotFoundException("User not found!");

            var currentCertificate = await dbContext.Certificates
                .FirstOrDefaultAsync(x => x.Id == request.Certificate_id && x.user_id == currentUser.Id, cancellationToken);

            if (currentCertificate == null) throw new NotFoundException($"Certificate of {currentUser.fullname} not found!");
            if (request.Arg.certificate_name == null) throw new BadHttpRequestException("Certificate data cannot be null!");

            currentUser.UpdateCertificate(request.Certificate_id, request.Arg.certificate_name);
            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new CTBaseResult<OkResponse>(new OkResponse("Certificate updated successfully!"));
            }
            return CTBaseResult.ErrorServer("Nothing changes!");
        }
    }
}