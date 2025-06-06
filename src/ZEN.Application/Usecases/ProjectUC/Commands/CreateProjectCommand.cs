using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using ZEN.Contract.ProjectDto.Request;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Definition;
using ZEN.Domain.Entities.Identities;

namespace ZEN.Application.Usecases.ProjectUC.Commands
{
    public class CreateProjectCommand(ReqCreateProjectDto arg) : ICommand<OkResponse>
    {
        public ReqCreateProjectDto Arg = arg;
    }

    public class CreateProjectCommandHandler(
        IUnitOfWork unitOfWork,
        IRepository<Project> projectRepo,
        IRepository<UserProject> userProjectRepo,
        IUserIdentifierProvider provider
    ) : ICommandHandler<CreateProjectCommand, OkResponse>
    {
        public async Task<CTBaseResult<OkResponse>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Arg;
            var newProject = Project.Create(
                project_name: dto.project_name,
                description: dto.description,
                tech: dto.tech,
                project_type: dto.project_type,
                is_reality: dto.is_Reality,
                url_project: dto.url_project,
                url_demo: dto.url_demo,
                url_github: dto.url_github,
                duration: dto.duration
            );
            projectRepo.Add(newProject);
            var userId = provider.UserId;
            Console.WriteLine($"[DEBUG] UserId resolved: {userId}");
            var newUserProject = UserProject.Create(provider.UserId, newProject.Id);
            System.Console.WriteLine(provider.UserId);
            userProjectRepo.Add(newUserProject);

            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 1)
            {
                return new CTBaseResult<OkResponse>(new OkResponse($"Create project {newProject.Id} successfully!"));
            }
            return CTBaseResult.ErrorServer(CTErrors.FAIL_TO_SAVE);
        }
    }
}