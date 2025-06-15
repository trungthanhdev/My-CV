using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Definition;
using ZEN.Domain.Entities.Identities;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.SkillUC.Commands
{
    public class DeleteSkillCommand : ICommand<OkResponse>
    {

    }
    public class DeleteSkillCommandHandler(
        IUnitOfWork unitOfWork,
        AppDbContext dbContext,
        IUserIdentifierProvider provider
    ) : ICommandHandler<DeleteSkillCommand, OkResponse>
    {
        public async Task<CTBaseResult<OkResponse>> Handle(DeleteSkillCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await dbContext.Users
                        .Include(x => x.UserSkills)
                        .Where(x => x.Id == provider.UserId)
                        .FirstOrDefaultAsync(cancellationToken);

            if (currentUser is null) throw new NotFoundException("Current user not found!");

            var currentUserSkill = await dbContext.UserSkills
                        .Where(x => x.user_id == provider.UserId)
                        .ToListAsync(cancellationToken);


            var userSkillToDelete = currentUserSkill
                        .Where(x => x.user_id != null && x.skill_id != null)
                        .Select(x => x.skill_id!)
                        .ToList();
            if (!userSkillToDelete.Any()) throw new NotFoundException("You do have skill yet!");

            // currentUser.DeleteUserSkill(userSkillToDelete);

            // var userSkillsWithNulls = await dbContext.UserSkills
            //        .Where(us => us.user_id == null || us.skill_id == null)
            //        .ToListAsync(cancellationToken);

            // dbContext.UserSkills.RemoveRange(userSkillsWithNulls);


            var skillsToDelete = await dbContext.Skills
                        .Where(x => userSkillToDelete.Contains(x.Id))
                        .ToListAsync(cancellationToken);

            dbContext.Skills.RemoveRange(skillsToDelete);

            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new CTBaseResult<OkResponse>(new OkResponse($"User {currentUser.fullname} deletes his/her skill successfully!"));
            }
            return CTBaseResult.ErrorServer(CTErrors.FAIL_TO_SAVE);
        }
    }
}