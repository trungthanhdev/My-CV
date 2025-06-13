using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.ProjectDto.Request;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Definition;
using ZEN.Domain.Entities.Identities;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.ProjectUC.Commands
{
    public class UpdateProjectCommand(string project_id, ReqUpdateProjectDto arg) : ICommand<OkResponse>
    {
        public string Project_Id = project_id;
        public ReqUpdateProjectDto Arg = arg;
    }
    public class UpdateProjectCommandHandler(
        IRepository<Project> projectRepo,
        IUnitOfWork unitOfWork,
        IUserIdentifierProvider provider,
        AppDbContext dbContext
    ) : ICommandHandler<UpdateProjectCommand, OkResponse>
    {
        public async Task<CTBaseResult<OkResponse>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var hasPermission = await dbContext.UserProjects
                .AnyAsync(x => x.project_id == request.Project_Id
                            && x.user_id == provider.UserId, cancellationToken);

            if (!hasPermission)
            {
                throw new UnauthorizedAccessException("You have no permission!");
            }

            var currentProject = await projectRepo.GetByIdAsync(request.Project_Id, cancellationToken);
            if (currentProject == null)
            {
                throw new NotFoundException("Project not found!");
            }

            var projectTech = await dbContext.Teches
                        .AsNoTracking()
                        .Where(x => x.project_id == request.Project_Id)
                        .ToListAsync(cancellationToken);
            foreach (var tech in projectTech)
            {
                dbContext.Teches.Remove(tech);
            }
            currentProject.CreateNewTech(request.Arg.tech!, currentProject.Id);
            currentProject.Update(request.Arg);

            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new CTBaseResult<OkResponse>(new OkResponse($"Update project {currentProject.Id} successfully!"));
            }
            return CTBaseResult.ErrorServer(CTErrors.FAIL_TO_SAVE);
        }
    }
}