
using System.Net;
using Microsoft.AspNetCore.Http;

namespace ZEN.Controller.Extensions;

public static class HttpContextExtensions
{
    public static string GetIpAddress(this HttpContext context)
    {
        // Check X-Forwarded-For header first
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            string forwardedIp = forwardedFor.FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim() ?? "";
            if (!string.IsNullOrEmpty(forwardedIp))
            {
                // Try to parse as IP address to validate
                if (IPAddress.TryParse(forwardedIp, out var _))
                {
                    return forwardedIp;
                }
            }
        }
        
        // Fall back to RemoteIpAddress
        var remoteIp = context.Connection.RemoteIpAddress;
        
        // Handle localhost scenarios
        if (remoteIp == null || IPAddress.IsLoopback(remoteIp))
        {
            return "127.0.0.1";
        }
        
        // Convert IPv6 to IPv4 if applicable
        if (remoteIp.IsIPv4MappedToIPv6)
        {
            return remoteIp.MapToIPv4().ToString();
        }
        
        return remoteIp.ToString();
    }
    
    // Get only IPv4 addresses
    public static string GetIpv4Address(this HttpContext context)
    {
        string ip = context.GetIpAddress();
        
        // Check if it's an IPv4 address
        if (IPAddress.TryParse(ip, out var ipAddress) && 
            (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork))
        {
            return ip;
        }
        
        // Try to convert IPv6 to IPv4 if possible
        if (IPAddress.TryParse(ip, out ipAddress) && 
            ipAddress.IsIPv4MappedToIPv6)
        {
            return ipAddress.MapToIPv4().ToString();
        }
        
        // Return null or default value if not IPv4
        return "127.0.0.1";
    }
}
