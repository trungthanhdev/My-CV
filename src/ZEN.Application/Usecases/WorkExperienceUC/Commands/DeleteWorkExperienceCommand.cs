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

namespace ZEN.Application.Usecases.WorkExperienceUC.Commands
{
    public class DeleteWorkExperienceCommand(string we_id) : ICommand<OkResponse>
    {
        public string We_id = we_id;
    }
    public class DeleteWorkExperienceCommandHandler(
        IUnitOfWork unitOfWork,
        IUserIdentifierProvider provider,
        AppDbContext dbContext,
        IRepository<WorkExperience> weRepo
    ) : ICommandHandler<DeleteWorkExperienceCommand, OkResponse>
    {
        public async Task<CTBaseResult<OkResponse>> Handle(DeleteWorkExperienceCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await dbContext.Users.Where(x => x.Id == provider.UserId).FirstOrDefaultAsync(cancellationToken);
            if (currentUser is null) throw new NotFoundException("User not found!");
            var currentExperience = await dbContext.WorkExperiences
                        .Where(x => x.Id == request.We_id)
                        .FirstOrDefaultAsync(cancellationToken);
            if (currentExperience is null) throw new NotFoundException("Current work experience not found!");
            if (currentExperience.user_id != currentUser.Id) throw new UnauthorizedAccessException("You have no permission!");

            weRepo.Remove(currentExperience);
            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new CTBaseResult<OkResponse>(new OkResponse($"User {currentUser.fullname} removes his/her work experience successfully!"));
            }
            return CTBaseResult.ErrorServer(CTErrors.FAIL_TO_SAVE);
        }
    }
}