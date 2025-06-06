using System.Net.NetworkInformation;
using ZEN.Domain.Services;

namespace ZEN.Infrastructure.InSystemProvider;

public class HardwareSpecService : IHardwareSpec
{
    public string GetMacAddress()
    {
        foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (nic.OperationalStatus == OperationalStatus.Up && nic.Name == "en0")
            {
                return nic.GetPhysicalAddress().ToString();
            }
        }

        return "nil";
    }
}