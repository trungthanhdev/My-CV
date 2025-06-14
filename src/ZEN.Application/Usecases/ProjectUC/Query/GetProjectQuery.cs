using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Core.Mediators.Abstraction;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.ProjectDto.Request;
using ZEN.Contract.ProjectDto.Response;
using ZEN.Contract.ResponsePagination;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Entities.Identities;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.ProjectUC.Query
{
    public class GetProjectQuery(int page_index, int page_size) : IQuery<PageResultResponse<ResProjectDto>>
    {
        public int Page_Index = page_index;
        public int Page_Size = page_size;
    }
    public class GetProjectQueryHandler(
        AppDbContext dbContext,
        IUserIdentifierProvider provider
    ) : IQueryHandler<GetProjectQuery, PageResultResponse<ResProjectDto>>
    {
        public async Task<CTBaseResult<PageResultResponse<ResProjectDto>>> Handle(GetProjectQuery request, CancellationToken cancellationToken)
        {
            if (request.Page_Index < 1 || request.Page_Size < 1)
                throw new BadHttpRequestException("Page Size and Page Index must be equal or greater than 1");

            var skip = (request.Page_Index - 1) * request.Page_Size;
            var userProjectIds = await dbContext.UserProjects
                    .AsNoTracking()
                    .Where(x => x.user_id == provider.UserId)
                    .Select(x => x.project_id)
                    .ToListAsync(cancellationToken);

            int total_item = await dbContext.Projects
                            .AsNoTracking()
                            .Where(x => userProjectIds.Contains(x.Id))
                            .CountAsync(cancellationToken);

            System.Console.WriteLine($"total_item: {total_item}");
            var dataList = await dbContext.Projects
                    .AsNoTracking()
                    .Where(x => userProjectIds.Contains(x.Id))
                    .Include(x => x.Teches)
                    .OrderByDescending(p => p.from)
                    .Skip(skip)
                    .Take(request.Page_Size)
                    .Select(x => new ResProjectDto
                    {
                        project_id = x.Id,
                        project_name = x.project_name,
                        description = x.description,
                        img_url = x.img_url,
                        project_type = x.project_type,
                        is_Reality = x.is_Reality,
                        url_project = x.url_project,
                        url_demo = x.url_demo,
                        url_github = x.url_github,
                        duration = x.duration,
                        from = x.from,
                        to = x.to,
                        teches = x.Teches.Select(t => new TechDto
                        {
                            tech_name = t.tech_name!
                        }).ToList()
                    })
                    .ToListAsync(cancellationToken);
            return new CTBaseResult<PageResultResponse<ResProjectDto>>(new PageResultResponse<ResProjectDto>
            {
                total_item = total_item,
                data = dataList
            });
        }
    }
}