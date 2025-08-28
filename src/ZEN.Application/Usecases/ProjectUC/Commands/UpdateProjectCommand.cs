using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using DocumentFormat.OpenXml.Office2021.Excel.Pivot;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.ProjectDto.Request;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Definition;
using ZEN.Domain.Entities.Identities;
using ZEN.Domain.Interfaces;
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
        AppDbContext dbContext,
        IRedisCache redisCache,
        ISavePhotoToCloud savePhotoToCloud
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
            var cacheKey = $"pu:{provider.UserId}:p:1:s:10";
            await redisCache.RemoveByPrefixAsync(cacheKey);

            var urlImgInDB = currentProject.img_url;
            if (request.Arg.img_url != null || request.Arg?.img_url?.Length > 0)
            {
                using var stream = request.Arg!.img_url!.OpenReadStream();
                var url = await savePhotoToCloud.UploadPhotoAsync(stream, request.Arg.img_url.FileName);
                urlImgInDB = url;
            }

            if (request.Arg?.img_url != null)
            {
                if (currentProject.img_url != null)
                {
                    await savePhotoToCloud.DeletePhotoAsync(currentProject.img_url);
                }
            }

            var projectTech = await dbContext.Teches
                        .AsNoTracking()
                        .Where(x => x.project_id == request.Project_Id)
                        .ToListAsync(cancellationToken);
            foreach (var tech in projectTech)
            {
                dbContext.Teches.Remove(tech);
            }
            if (request.Arg?.tech != null)
                currentProject.CreateNewTech(request.Arg.tech, currentProject.Id);

            if (request.Arg == null)
            {
                throw new ArgumentNullException(nameof(request.Arg));
            }
            currentProject.Update(request.Arg, urlImgInDB);

            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new CTBaseResult<OkResponse>(new OkResponse($"Update project {currentProject.Id} successfully!"));
            }
            return CTBaseResult.ErrorServer(CTErrors.FAIL_TO_SAVE);
        }
    }
}