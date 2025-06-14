using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.SkillDto.Response;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Entities.Identities;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.SkillUC.Queries
{
    public class GetAllSkillQueries : IQuery<List<ResSkillDto>>
    {

    }

    public class GetAllSkillQueriesHandler(
        IUserIdentifierProvider provider,
        AppDbContext dbContext
    ) : IQueryHandler<GetAllSkillQueries, List<ResSkillDto>>
    {
        public async Task<CTBaseResult<List<ResSkillDto>>> Handle(GetAllSkillQueries request, CancellationToken cancellationToken)
        {
            var currentUser = await dbContext.Users
                    .AsNoTracking()
                    .Where(x => x.Id == provider.UserId)
                    .FirstOrDefaultAsync(cancellationToken);
            if (currentUser is null) throw new NotFoundException("Current user not found!");

            var allUserSkills = await dbContext.UserSkills
                    .AsNoTracking()
                    .Include(x => x.Skill)
                    .Where(x => x.user_id == currentUser.Id)
                    .Select(x => new ResSkillDto
                    {
                        skill_name = x.Skill.skill_name,
                        position = x.Skill.position
                    })
                    .ToListAsync(cancellationToken);
            return new CTBaseResult<List<ResSkillDto>>(allUserSkills);
        }
    }
}