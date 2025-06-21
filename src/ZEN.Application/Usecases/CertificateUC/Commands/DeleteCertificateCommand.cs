using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Definition;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.CertificateUC.Commands
{
    public class DeleteCertificateCommand(string certificate_id) : ICommand<OkResponse>
    {
        public string Certificate_id = certificate_id;
    }
    public class DeleteCertificateCommandHandler(
        IUnitOfWork unitOfWork,
        IUserIdentifierProvider provider,
        AppDbContext dbContext
    // ILogger<DeleteCertificateCommand> _logger
    ) : ICommandHandler<DeleteCertificateCommand, OkResponse>
    {
        public async Task<CTBaseResult<OkResponse>> Handle(DeleteCertificateCommand request, CancellationToken cancellationToken)
        {
            // _logger.LogInformation($"Certificate_id: {request.Certificate_id}");
            var currentUser = await dbContext.Users
                       .Where(x => x.Id == provider.UserId)
                       .Include(x => x.Certificates)
                       .FirstOrDefaultAsync(cancellationToken);
            if (currentUser is null) throw new NotFoundException("Current user not found!");

            var deletedCertificate = currentUser.DeleteCertificate(request.Certificate_id);
            dbContext.Certificates.Remove(deletedCertificate);

            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new CTBaseResult<OkResponse>(new OkResponse($"User {currentUser.fullname} deletes his/her certificate successfully!"));
            }
            return CTBaseResult.ErrorServer(CTErrors.FAIL_TO_SAVE);
        }
    }
}