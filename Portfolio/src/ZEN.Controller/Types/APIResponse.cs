using CTCore.DynamicQuery.Common.Definations;
using CTCore.DynamicQuery.Core.Primitives;
using ZEN.Domain.Common.Primitives;

namespace ZEN.Controller.Types;

public sealed class APIErrorResponse(string msg, int errorCode) : BaseResponse(msg, errorCode)
{
}

