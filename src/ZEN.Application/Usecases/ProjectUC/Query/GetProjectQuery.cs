using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Core.Mediators.Abstraction;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZEN.Contract.ProjectDto.Request;
using ZEN.Contract.ProjectDto.Response;
using ZEN.Contract.ResponsePagination;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Entities.Identities;
using ZEN.Domain.Interfaces;
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
        IUserIdentifierProvider provider,
        IRedisCache redisCache,
        ILogger<GetProjectQuery> _logger
    ) : IQueryHandler<GetProjectQuery, PageResultResponse<ResProjectDto>>
    {
        public async Task<CTBaseResult<PageResultResponse<ResProjectDto>>> Handle(GetProjectQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"pu:{provider.UserId}:p:{request.Page_Index}:s:{request.Page_Size}";
            try
            {
                var data = await redisCache.GetAsync(cacheKey);
                if (data != null)
                {
                    var parseData = JsonSerializer.Deserialize<PageResultResponse<ResProjectDto>>(data);
                    if (parseData != null)
                    {
                        return new CTBaseResult<PageResultResponse<ResProjectDto>>(parseData);
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogWarning($"Project in Redis of user {provider.UserId} is empty!, with error: {ex.Message}");
            }

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

            _logger.LogInformation($"total_item: {total_item} of {provider.UserId}");
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
                        }).ToList(),
                        url_contract = x.url_contract,
                        url_excel = x.url_excel
                    })
                    .ToListAsync(cancellationToken);
            var result = new PageResultResponse<ResProjectDto>
            {
                total_item = total_item,
                data = dataList
            };

            if (dataList.Any())
            {
                await redisCache.SetAsync(cacheKey, JsonSerializer.Serialize(result), TimeSpan.FromMinutes(5));
            }
            else
            {
                _logger.LogInformation($"User {provider.UserId} does not have any project yet!");
            }

            return new CTBaseResult<PageResultResponse<ResProjectDto>>(result);
        }
    }
}