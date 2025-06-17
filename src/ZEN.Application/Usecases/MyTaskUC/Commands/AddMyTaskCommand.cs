using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.MyTaskDto.Request;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Definition;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.MyTaskUC.Commands
{
    public class AddMyTaskCommand(string we_id, ReqMyTaskDto arg) : ICommand<OkResponse>
    {
        public string We_id = we_id;
        public ReqMyTaskDto Arg = arg;
    }
    public class AddMyTaskCommandHandler(
        IUnitOfWork unitOfWork,
        AppDbContext dbContext,
        IUserIdentifierProvider provider
    ) : ICommandHandler<AddMyTaskCommand, OkResponse>
    {
        public async Task<CTBaseResult<OkResponse>> Handle(AddMyTaskCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await dbContext.Users
                        .AsNoTracking()
                        .Where(x => x.Id == provider.UserId)
                        .FirstOrDefaultAsync(cancellationToken);
            if (currentUser is null) throw new NotFoundException("Current user not found!");

            var currentExperience = await dbContext.WorkExperiences
                        .Where(x => x.Id == request.We_id)
                        .FirstOrDefaultAsync(cancellationToken);
            if (currentExperience is null) throw new NotFoundException("Current experience not found!");
            if (currentUser.Id != currentExperience.user_id) throw new UnauthorizedAccessException("You have no permission!");

            if (request.Arg.task_description is null) throw new BadHttpRequestException("Task description is required!");
            currentExperience.AddMyTask(request.Arg.task_description);

            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new CTBaseResult<OkResponse>(new OkResponse($"User {currentUser.fullname} adds his/her experience task successfully!"));
            }
            return CTBaseResult.ErrorServer(CTErrors.FAIL_TO_SAVE);
        }
    }
}