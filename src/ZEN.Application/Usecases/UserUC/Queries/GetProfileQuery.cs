using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.DTO.UserDto.Response;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.UserUC.Queries
{
    public class GetProfileQuery : IQuery<ResUserDto>
    {

    }
    public class GetProfileQueryHandler(
        AppDbContext dbContext,
        IUserIdentifierProvider provider
    ) : IQueryHandler<GetProfileQuery, ResUserDto>
    {
        public async Task<CTBaseResult<ResUserDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
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

            if (currentUser is null) throw new NotFoundException("Current user not found!");
            return new CTBaseResult<ResUserDto>(currentUser);
        }
    }
}