using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.AspAccountDto;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Definition;
using ZEN.Domain.Interfaces;
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
        AppDbContext dbContext,
        ISavePhotoToCloud savePhotoToCloud,
        IRedisCache redisCache
    ) : ICommandHandler<UpdateProfileCommand, OkResponse>
    {
        public async Task<CTBaseResult<OkResponse>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == request.User_id, cancellationToken);

            if (currentUser == null)
                throw new NotFoundException("User not found");

            var urlImgInDB = currentUser.avatar ?? "";
            if (request.Arg.avatar != null || request.Arg?.avatar?.Length > 0)
            {
                using var stream = request.Arg!.avatar!.OpenReadStream();
                var url = await savePhotoToCloud.UploadPhotoAsync(stream, request.Arg.avatar.FileName);
                urlImgInDB = url;
            }

            if (provider.UserId != request.User_id)
                throw new UnauthorizedAccessException("You have no permission!");
            if (request.Arg == null)
                throw new ArgumentNullException(nameof(request.Arg));

            currentUser.Update(request.Arg, urlImgInDB);
            var currentUserAttribure = new
            {
                user_id = currentUser.Id,
                fullname = currentUser.fullname,
                university_name = currentUser.university_name,
                address = currentUser.address,
                phone_number = currentUser.phone_number,
                github = currentUser.github,
                dob = currentUser.dob,
                avatar = currentUser.avatar,
                GPA = currentUser.GPA,
                email = currentUser.Email,
                workExpOfYear = currentUser.expOfYear,
                linkedin_url = currentUser.linkedin_url,
                mindset = currentUser.mindset,
                position_career = currentUser.position_career,
                background = currentUser.background,
                facebook_url = currentUser.facebook_url
            };

            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                var cacheKey = $"profile:{currentUserAttribure.user_id}";
                if (cacheKey != null)
                {
                    await redisCache.RemoveAsync(cacheKey);
                    await redisCache.SetAsync(cacheKey, JsonSerializer.Serialize(currentUserAttribure), TimeSpan.FromMinutes(10));
                }
                else
                {
                    await redisCache.SetAsync(cacheKey!, JsonSerializer.Serialize(currentUserAttribure), TimeSpan.FromMinutes(10));
                }
                return new CTBaseResult<OkResponse>(new OkResponse($"User {currentUser.Id} updated successfully!"));
            }
            return CTBaseResult.ErrorServer("Nothing changes!");
        }
    }
}