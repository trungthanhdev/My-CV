using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Definition;
using ZEN.Domain.Entities.Identities;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.MyTaskUC.Commands
{
    public class DeleteMyTaskCommand(string mt_id) : ICommand<OkResponse>
    {
        public string Mt_id = mt_id;
    }
    public class DeleteMyTaskCommandHadler(
        IUnitOfWork unitOfWork,
        IUserIdentifierProvider provider,
        AppDbContext dbContext,
        IRepository<MyTask> myTaskRepo
    ) : ICommandHandler<DeleteMyTaskCommand, OkResponse>
    {
        public async Task<CTBaseResult<OkResponse>> Handle(DeleteMyTaskCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == provider.UserId, cancellationToken);
            if (currentUser is null) throw new NotFoundException("Current user not found!");

            var currentMyTask = await dbContext.MyTasks
                    .Where(x => x.Id == request.Mt_id)
                    .FirstOrDefaultAsync(cancellationToken);
            if (currentMyTask is null) throw new NotFoundException("My task not found!");

            myTaskRepo.Remove(currentMyTask);
            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new CTBaseResult<OkResponse>(new OkResponse($"User {currentUser.fullname} deletes his/her task successfully!"));
            }
            return CTBaseResult.ErrorServer(CTErrors.FAIL_TO_SAVE);
        }
    }
}