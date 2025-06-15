using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.DTO.UserDto.Response;
using ZEN.Domain.Interfaces;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.UserUC.Queries
{
    public class GetProfileQuery : IQuery<ResUserDto>
    {

    }
    public class GetProfileQueryHandler(
        AppDbContext dbContext,
        IUserIdentifierProvider provider,
        IRedisCache redisCache
    ) : IQueryHandler<GetProfileQuery, ResUserDto>
    {
        public async Task<CTBaseResult<ResUserDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            //check in Redis first
            var cacheKey = $"profile:{provider.UserId}";
            try
            {
                var cacheData = await redisCache.GetAsync(cacheKey);
                if (cacheData != null)
                {
                    var porfolioFromRedis = JsonSerializer.Deserialize<ResUserDto>(cacheData);
                    if (porfolioFromRedis != null)
                    {
                        return new CTBaseResult<ResUserDto>(porfolioFromRedis);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis read failed: {ex.Message}");
            }

            var currentUser = await dbContext.Users
                        .AsNoTracking()
                        .Where(x => x.Id == provider.UserId)
                        .Select(x => new ResUserDto
                        {
                            user_id = x.Id,
                            fullname = x.fullname,
                            university_name = x.university_name,
                            address = x.address,
                            phone_number = x.phone_number,
                            github = x.github,
                            dob = x.dob,
                            avatar = x.avatar,
                            GPA = x.GPA,
                            email = x.Email,
                            workExpOfYear = x.expOfYear,
                            linkedin_url = x.linkedin_url,
                            mindset = x.mindset,
                            position_career = x.position_career,
                            background = x.background,
                            facebook_url = x.facebook_url
                        })
                        .FirstOrDefaultAsync(cancellationToken);

            // await redisCache.SetAsync(cacheKey, JsonSerializer.Serialize(currentUser), TimeSpan.FromMinutes(10));
            try
            {
                if (currentUser != null)
                {
                    await redisCache.SetAsync(cacheKey, JsonSerializer.Serialize(currentUser), TimeSpan.FromMinutes(10));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis write failed: {ex.Message}");
            }

            if (currentUser is null) throw new NotFoundException("Current user not found!");
            return new CTBaseResult<ResUserDto>(currentUser);
        }
    }
}