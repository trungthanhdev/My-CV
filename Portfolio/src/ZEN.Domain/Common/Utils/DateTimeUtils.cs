
using System.Runtime.InteropServices;

namespace ZEN.Domain.Common.Utils;


public static class DateTimeUtils
{
    private static bool isWindow = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    public static TimeZoneInfo GetTZInfo() => 
        isWindow ? 
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time") : 
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
    public static DateTimeOffset ToLocal(this DateTimeOffset dtOffset) {
        return dtOffset.ToOffset(GetTZInfo().GetUtcOffset(dtOffset));
    }
}