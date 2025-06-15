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
using ZEN.Domain.DTO.WorkExperienceDto.Request;
using ZEN.Domain.Entities.Identities;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.WorkExperienceUC.Commands
{
    public class UpdateWorkExperienceCommand(ReqWorkExperienceDto arg, string we_id) : ICommand<OkResponse>
    {
        public string We_Id = we_id;
        public ReqWorkExperienceDto Arg = arg;
    }
    public class UpdateWorkExperienceCommandHandler(
        IRepository<WorkExperience> weRepo,
        IUserIdentifierProvider provider,
        IUnitOfWork unitOfWork,
        AppDbContext dbContext
    ) : ICommandHandler<UpdateWorkExperienceCommand, OkResponse>
    {
        public async Task<CTBaseResult<OkResponse>> Handle(UpdateWorkExperienceCommand request, CancellationToken cancellationToken)
        {
            var currentWE = await weRepo.GetByIdAsync(request.We_Id, cancellationToken);
            if (currentWE is null) throw new NotFoundException("Work experience not found!");
            if (currentWE.user_id != provider.UserId) throw new UnauthorizedAccessException("You have no permission!");

            var currentUser = await dbContext.Users.Where(x => x.Id == provider.UserId).FirstOrDefaultAsync(cancellationToken);
            if (currentUser is null) throw new NotFoundException("User not found!");

            currentUser.UpdateWorkExperience(request.Arg, request.We_Id);
            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new CTBaseResult<OkResponse>(new OkResponse($"User {currentUser.fullname} updates work experience successfully!"));
            }
            return CTBaseResult.ErrorServer(CTErrors.FAIL_TO_SAVE);
        }
    }
}