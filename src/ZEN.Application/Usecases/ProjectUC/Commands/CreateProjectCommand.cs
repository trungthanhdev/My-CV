using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.AspNetCore.Http;
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
        // IRepository<UserProject> userProjectRepo,
        IUserIdentifierProvider provider
    ) : ICommandHandler<CreateProjectCommand, OkResponse>
    {
        public async Task<CTBaseResult<OkResponse>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Arg;
            var newProject = Project.Create(
                project_name: dto.project_name,
                description: dto.description,
                project_type: dto.project_type,
                is_reality: dto.is_Reality,
                url_project: dto.url_project,
                url_demo: dto.url_demo,
                url_github: dto.url_github,
                duration: dto.duration,
                from: dto.from,
                to: dto.to
            );
            projectRepo.Add(newProject);
            if (dto.tech is null) throw new BadHttpRequestException("Tech are required!");
            foreach (var tech in dto.tech)
            {
                newProject.AddTechToProject(tech.tech_name, newProject.Id);
            }
            newProject.AddUserProject(provider.UserId, newProject.Id);

            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 1)
            {
                return new CTBaseResult<OkResponse>(new OkResponse($"Create project {newProject.Id} successfully!"));
            }
            return CTBaseResult.ErrorServer(CTErrors.FAIL_TO_SAVE);
        }
    }
}