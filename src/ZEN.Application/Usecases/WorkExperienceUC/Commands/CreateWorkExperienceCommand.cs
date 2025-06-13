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
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.WorkExperienceUC.Commands
{
    public class CreateWorkExperienceCommand(ReqWorkExperienceDto arg) : ICommand<OkResponse>
    {
        public ReqWorkExperienceDto Arg = arg;
    }
    public class CreateWorkExperienceCommandHandle(
        AppDbContext dbContext,
        IUserIdentifierProvider provider,
        IUnitOfWork unitOfWork
    ) : ICommandHandler<CreateWorkExperienceCommand, OkResponse>
    {
        public async Task<CTBaseResult<OkResponse>> Handle(CreateWorkExperienceCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await dbContext.Users
                .Where(x => x.Id == provider.UserId)
                .FirstOrDefaultAsync(cancellationToken);
            if (currentUser is null) throw new NotFoundException("User not found!");

            currentUser.AddWorkExperience(request.Arg, provider.UserId);
            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new CTBaseResult<OkResponse>(new OkResponse($"User {currentUser.fullname} adds work experience successfully!"));
            }
            return CTBaseResult.ErrorServer(CTErrors.FAIL_TO_SAVE);
        }
    }
}