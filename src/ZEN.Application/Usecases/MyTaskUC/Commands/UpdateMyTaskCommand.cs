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
using ZEN.Contract.MyTaskDto.Request;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Entities.Identities;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.MyTaskUC.Commands
{
    public class UpdateMyTaskCommand(string we_id, ReqMyTaskDto arg) : ICommand<OkResponse>
    {
        public string We_id = we_id;
        public ReqMyTaskDto Arg = arg;
    }
    public class UpdateMyTaskCommandHandler(
        IUnitOfWork unitOfWork,
        IUserIdentifierProvider provider,
        AppDbContext dbContext
    // ILogger<UpdateMyTaskCommand> _logger
    ) : ICommandHandler<UpdateMyTaskCommand, OkResponse>
    {
        public async Task<CTBaseResult<OkResponse>> Handle(UpdateMyTaskCommand request, CancellationToken cancellationToken)
        {
            // _logger.LogWarning($"user_id: {provider.UserId}");
            // _logger.LogWarning($"Arg: mt_id: {request.Arg.myTask_id}, des: {request.Arg.task_description}");
            // _logger.LogWarning($"We_Id: {request.We_id}");

            var currentUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == provider.UserId, cancellationToken);
            if (currentUser is null) throw new NotFoundException("Current user not found!");

            var currentUserWE = await dbContext.WorkExperiences
                    .Include(x => x.MyTasks)
                    .Where(x => x.user_id == currentUser.Id && x.Id == request.We_id)
                    .FirstOrDefaultAsync(cancellationToken);
            if (currentUserWE is null) throw new NotFoundException($"User {currentUser.fullname} does not have this experience!");
            // _logger.LogWarning($"we_project_name: {currentUserWE.project_id}");

            if (request.Arg.myTask_id == null || request.Arg.task_description == null) throw new BadHttpRequestException("Task_id and task_description are required!");
            currentUserWE.UpdateMyTask(request.Arg.myTask_id, request.Arg.task_description);

            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new CTBaseResult<OkResponse>(new OkResponse($"User {currentUser.fullname} updates his/her task successfully!"));
            }
            return CTBaseResult.ErrorServer("Nothing changes!");
        }
    }
}