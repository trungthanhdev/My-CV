using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.ResponsePagination;
using ZEN.Contract.WEDTO.Response;
using ZEN.Domain.Common.Authenticate;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.WorkExperienceUC.Queries
{
    public class GetAllWorkExperieceQuery(int page_index, int page_size) : IQuery<PageResultResponse<ResWEDto>>
    {
        public int Page_Index = page_index;
        public int Page_Size = page_size;
    }
    public class GetAllWorkExperiecQueryHandler(
        AppDbContext dbContext,
        IUserIdentifierProvider provider
    ) : IQueryHandler<GetAllWorkExperieceQuery, PageResultResponse<ResWEDto>>
    {
        public async Task<CTBaseResult<PageResultResponse<ResWEDto>>> Handle(GetAllWorkExperieceQuery request, CancellationToken cancellationToken)
        {
            if (request.Page_Index < 1 || request.Page_Size < 1)
                throw new BadHttpRequestException("Page Size and Page Index must be equal or greater than 1");

            var currentUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == provider.UserId);
            if (currentUser is null) throw new NotFoundException("User not found!");

            var skip = (request.Page_Index - 1) * request.Page_Size;
            int total_item = await dbContext.WorkExperiences
                    .AsNoTracking()
                    .Where(x => x.user_id == provider.UserId)
                    .CountAsync(cancellationToken);

            var allWE = await dbContext.WorkExperiences
                    .AsNoTracking()
                    .Where(x => x.user_id == provider.UserId)
                    .Skip(skip)
                    .Take(request.Page_Size)
                    .Select(x => new ResWEDto
                    {
                        user_id = x.user_id,
                        company_name = x.company_name,
                        position = x.position,
                        duration = x.duration,
                        description = x.description,
                        project_id = x.project_id
                    })
                    .ToListAsync(cancellationToken);
            return new CTBaseResult<PageResultResponse<ResWEDto>>(new PageResultResponse<ResWEDto> { total_item = total_item, data = allWE });
        }
    }
}