using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.SkillDto.Request;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Definition;
using ZEN.Domain.Entities.Identities;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.SkillUC.Commands
{
    public class UpdateSkillCommand(ReqSkillDto arg, string skill_id) : ICommand<OkResponse>
    {
        public ReqSkillDto Arg = arg;
        public string Skill_id = skill_id;
    }
    public class CreateNewSkillCommandHandler(
        IUnitOfWork unitOfWork,
        AppDbContext dbContext,
        IUserIdentifierProvider provider
    ) : ICommandHandler<UpdateSkillCommand, OkResponse>
    {
        public async Task<CTBaseResult<OkResponse>> Handle(UpdateSkillCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await dbContext.Users
                        .Where(x => x.Id == provider.UserId)
                        .FirstOrDefaultAsync(cancellationToken);
            if (currentUser is null) throw new NotFoundException("Current user not found!");

            var currentSkill = await dbContext.Skills
                        .Where(x => x.Id == request.Skill_id)
                        .FirstOrDefaultAsync(cancellationToken);
            if (currentSkill is null) throw new NotFoundException("Current skill not found!");

            var currentUserSkill = await dbContext.UserSkills
                        .AsNoTracking()
                        .Where(x => x.skill_id == request.Skill_id)
                        .FirstOrDefaultAsync(cancellationToken);

            if (currentUserSkill!.user_id != provider.UserId) throw new UnauthorizedAccessException("You have no permission!");

            currentSkill.Update(request.Arg.skill_name ?? currentSkill.skill_name, request.Arg.position ?? currentSkill.position ?? "Default position!");

            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new CTBaseResult<OkResponse>(new OkResponse($"User {currentUser.fullname} updated his/her skill successfully!"));
            }
            return CTBaseResult.ErrorServer(CTErrors.FAIL_TO_SAVE);
        }
    }
}