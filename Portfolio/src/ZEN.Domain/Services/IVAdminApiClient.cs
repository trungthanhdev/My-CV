
using CTCore.DynamicQuery.Core.Primitives;

namespace ZEN.Domain.Services;


public interface IVAdminApiClient
{
    Task<OkResponse<string>> GetPackageAssignToClientAsync(string payload, string keySeed);
}