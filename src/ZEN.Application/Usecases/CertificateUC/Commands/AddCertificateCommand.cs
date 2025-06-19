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
using Microsoft.Extensions.Logging;
using ZEN.Contract.CertificateDto.Request;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Definition;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.CertificateUC.Commands
{
    public class AddCertificateCommand(ReqCertificateDto arg) : ICommand<OkResponse>
    {
        public ReqCertificateDto Arg = arg;
    }
    public class AddCertificateCommandHandler(
        IUnitOfWork unitOfWork,
        AppDbContext dbContext,
        IUserIdentifierProvider provider
    // ILogger<AddCertificateCommand> _logger
    ) : ICommandHandler<AddCertificateCommand, OkResponse>
    {
        public async Task<CTBaseResult<OkResponse>> Handle(AddCertificateCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await dbContext.Users
                        .Where(x => x.Id == provider.UserId)
                        .FirstOrDefaultAsync(cancellationToken);
            if (currentUser is null) throw new NotFoundException("Current user not found!");
            if (request.Arg.certificate_name is null) throw new BadHttpRequestException("Certificate name is required!");
            currentUser.AddCertificate(request.Arg.certificate_name);
            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new CTBaseResult<OkResponse>(new OkResponse($"User {currentUser.fullname} adds his/her certificate successfully!"));
            }
            return CTBaseResult.ErrorServer(CTErrors.FAIL_TO_SAVE);
        }
    }
}