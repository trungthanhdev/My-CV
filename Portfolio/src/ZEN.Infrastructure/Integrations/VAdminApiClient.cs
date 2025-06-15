using System.Text.Json;
using CTCore.DynamicQuery.Core.Primitives;
using ZEN.Domain.Common.Utils;
using ZEN.Domain.Services;
using ZEN.Domain.Services.Dtos;

namespace ZEN.Infrastructure.Integrations;

public class VAdminApiClient : IVAdminApiClient
{
    public async Task<OkResponse<string>> GetPackageAssignToClientAsync(string payload, string keySeed)
    {
        var dehelper = new CryptoHelper(keySeed);

        var depayload1 = dehelper.DecryptAes(payload);

        var raw = JsonSerializer.Deserialize<RawData>(depayload1)!;

        var account3 = new PackageToClientResponse//(account2.orgId, "BASIC", account2.mac, 20, 2, DateTimeOffset.Now.AddMonths(1).ToUnixTimeMilliseconds())
        {
            OrgId = raw.orgId,
            PackageName = "BASIC",
            Mac = raw.mac,
            SoNguoi = 20,
            SoMCC = 2,
            Exp = DateTimeOffset.Now.AddMonths(1).ToUnixTimeMilliseconds()
        };

        var payloadResult = dehelper.EncryptAes(JsonSerializer.Serialize(account3));

        var sign = dehelper.EncodeECDSASignature(payloadResult, "private.pem");

        await Task.CompletedTask;

        return $"{payloadResult}.{Convert.ToBase64String(sign)}";
    }

    private record RawData(string mac, string orgId, string comName)
    {
        public string ToPayload()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
