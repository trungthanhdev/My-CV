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

namespace ZEN.Application.Usecases.SkillUC.Commands
{
    public class DeleteSpecificSkillCommand(string skill_id) : ICommand<OkResponse>
    {
        public string Skill_id = skill_id;
    }

    public class DeleteSpecificSkillCommandHandler(
        AppDbContext dbContext,
        IUserIdentifierProvider provider,
        IUnitOfWork unitOfWork,
        IRepository<Skill> skillRepo
    ) : ICommandHandler<DeleteSpecificSkillCommand, OkResponse>
    {
        public async Task<CTBaseResult<OkResponse>> Handle(DeleteSpecificSkillCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await dbContext.Users
                        .Where(x => x.Id == provider.UserId)
                        .FirstOrDefaultAsync(cancellationToken);
            if (currentUser is null) throw new NotFoundException("Current user not found!");

            var currentSkill = await dbContext.Skills
                        .Where(x => x.Id == request.Skill_id)
                        .FirstOrDefaultAsync(cancellationToken);
            if (currentSkill is null) throw new NotFoundException("Skill not found!");

            var currentUserSkill = await dbContext.UserSkills
                        .Where(x => x.user_id == currentUser.Id && x.skill_id == currentSkill.Id)
                        .FirstOrDefaultAsync(cancellationToken);
            if (currentUserSkill is null) throw new UnauthorizedAccessException("You have no permission!");

            skillRepo.Remove(currentSkill);
            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new CTBaseResult<OkResponse>(new OkResponse($"User {currentUser.fullname} deletes his/her skill successfully!"));
            }
            return CTBaseResult.ErrorServer(CTErrors.FAIL_TO_SAVE);
        }
    }
}