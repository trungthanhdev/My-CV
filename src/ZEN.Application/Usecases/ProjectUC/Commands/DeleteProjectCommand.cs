using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Definition;
using ZEN.Domain.Entities.Identities;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.ProjectUC.Commands
{
    public class DeleteProjectCommand(string project_id) : ICommand<OkResponse>
    {
        public string Project_Id = project_id;
    }
    public class DeleteProjectCommandHandler(
        IUnitOfWork unitOfWork,
        IUserIdentifierProvider provider,
        AppDbContext dbContext,
        IRepository<Project> projectRepo
    ) : ICommandHandler<DeleteProjectCommand, OkResponse>
    {
        public async Task<CTBaseResult<OkResponse>> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var currentProject = await projectRepo.GetByIdAsync(request.Project_Id, cancellationToken);
            if (currentProject == null) throw new NotFoundException("Project not found!");

            var userProject = await dbContext.UserProjects
                        .Where(x => x.user_id == provider.UserId && x.project_id == request.Project_Id)
                        .FirstOrDefaultAsync(cancellationToken);
            if (userProject == null) throw new UnauthorizedAccessException("You have no permission!");

            projectRepo.Remove(currentProject);
            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new CTBaseResult<OkResponse>(new OkResponse($"Delete project {currentProject.Id} successfully!"));
            }
            return CTBaseResult.ErrorServer(CTErrors.FAIL_TO_SAVE);
        }
    }
}