
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace ZEN.Infrastructure.Core.Mapping
{
    public static class MapsterConfig
    {
        public static void RegisterMapsterConfiguration(this IServiceCollection services)
        {
            // TypeAdapterConfig<ListRecord, SessionRecord>.NewConfig()
            //     .Map(dest => dest.Value, src => src.DefaultValue);

            // TypeAdapterConfig<User, EmployeeOfDepartmentDTO>.NewConfig()
            //     .Map(dest => dest.PositionName, e => e.IdNavigation != null ? e.IdNavigation.Position!.Name : string.Empty)
            //     .Map(dest => dest.Name, e => e.IdNavigation == null ? string.Empty : $"{e.IdNavigation!.LastName} {e.IdNavigation!.FirstName} ");

            // TypeAdapterConfig<DeviceSessionDetail, >.NewConfig()
            //      .Map(dest => dest.Employees, src => src.Users.Adapt<List<EmployeeOfDepartmentDTO>>());

            // TypeAdapterConfig<Document, CreateDocCommandArgs>.NewConfig()
            //     .Ignore(dest => dest.SubFiles!);
            // TypeAdapterConfig<CreateSubFilesArgs, DocumentSubject>.NewConfig()
            //     .Map(dest => dest.DocumentId, src => MapContext.Current != null ? MapContext.Current.Parameters["DocumentId"] : 0);
        }
    }
}
