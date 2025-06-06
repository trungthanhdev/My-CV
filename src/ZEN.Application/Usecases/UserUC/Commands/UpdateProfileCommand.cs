using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.AspAccountDto;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Definition;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.UserUC.Commands
{
    public class UpdateProfileCommand(ReqUserDto arg, string user_id) : ICommand<OkResponse>
    {
        public ReqUserDto Arg = arg;
        public string User_id = user_id;
    }
    public class UpdateProfileCommandHandler(
        IUnitOfWork unitOfWork,
        IUserIdentifierProvider provider,
        AppDbContext dbContext
    ) : ICommandHandler<UpdateProfileCommand, OkResponse>
    {
        public async Task<CTBaseResult<OkResponse>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == request.User_id, cancellationToken);

            if (currentUser == null)
                throw new NotFoundException("User not found");

            if (provider.UserId != request.User_id)
                throw new UnauthorizedAccessException("You have no permission!");

            currentUser.Update(request.Arg);
            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new CTBaseResult<OkResponse>(new OkResponse($"User {currentUser.Id} updated successfully!"));
            }
            return CTBaseResult.ErrorServer(CTErrors.FAIL_TO_SAVE);
        }
    }
}