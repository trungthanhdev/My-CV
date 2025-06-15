using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.SkillDto.Request;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Definition;
using ZEN.Domain.Entities.Identities;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.SkillUC.Commands
{
    public class CreateSkillCommand(ReqSkillDto arg) : ICommand<OkResponse>
    {
        public ReqSkillDto Arg = arg;
    }
    public class CreateSkillCommandHandler(
        IUnitOfWork unitOfWork,
        IUserIdentifierProvider provider,
        AppDbContext dbContext
    ) : ICommandHandler<CreateSkillCommand, OkResponse>
    {
        public async Task<CTBaseResult<OkResponse>> Handle(CreateSkillCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await dbContext.Users
                        .Where(x => x.Id == provider.UserId)
                        .FirstOrDefaultAsync(cancellationToken);
            if (currentUser is null) throw new NotFoundException($"User {provider.UserId} not found!");

            if (string.IsNullOrWhiteSpace(request.Arg.skill_name) || string.IsNullOrWhiteSpace(request.Arg.position))
                throw new BadHttpRequestException("Skill name and Position are required!");

            var newSkill = Skill.Create(request.Arg.skill_name, request.Arg.position);
            dbContext.Skills.Add(newSkill);

            currentUser.AddUserSkill(newSkill.Id);

            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new CTBaseResult<OkResponse>(new OkResponse($"User {currentUser.fullname} adds his/her skill successfully!"));
            }
            return CTBaseResult.ErrorServer(CTErrors.FAIL_TO_SAVE);
        }
    }
}