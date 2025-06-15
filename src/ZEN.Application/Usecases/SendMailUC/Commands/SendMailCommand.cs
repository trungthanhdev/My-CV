using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Definition;
using ZEN.Domain.Interfaces;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.SendMailUC.Commands
{
    public class SendMailCommand(HrContactDto arg) : ICommand<OkResponse>
    {
        public HrContactDto Arg = arg;
    }
    public class SendMailCommandHandler(
        ISendMail sendMail,
        IUserIdentifierProvider provider,
        AppDbContext dbContext
    ) : ICommandHandler<SendMailCommand, OkResponse>
    {
        public async Task<CTBaseResult<OkResponse>> Handle(SendMailCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await dbContext.Users
                        .AsNoTracking()
                        .Where(x => x.Id == provider.UserId).
                        FirstOrDefaultAsync(cancellationToken);
            if (currentUser is null) throw new NotFoundException("Current user not found!");
            if (request.Arg.HrCompany == null
                || request.Arg.HrName == null
                || request.Arg.HrEmail == null
                || request.Arg.HrNotes == null
                || request.Arg.HrNotes == null)
                throw new ArgumentNullException("Please fill information before contact!");

            request.Arg.user_email = currentUser.Email;
            request.Arg.user_name = currentUser.UserName;

            bool success = await sendMail.SendHrContactEmailAsync(request.Arg);
            // System.Console.WriteLine($"trinh trang gui gmail {success}");
            if (success)
            {
                return new CTBaseResult<OkResponse>(new OkResponse("Send email successfully, I will response as soon as posible!"));
            }
            return CTBaseResult.ErrorServer("Can not send email right now!");
        }
    }
}