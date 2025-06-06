namespace ZEN.Domain.Common.Utils;

public static class StringUtils
{
    public static string CreateDeviceCode(int deviceCount)
    {
        // Increment the count by 1 to get the next device code
        int nextDeviceCode = deviceCount + 1;

        // Format the device code as an 8-digit string with leading zeros
        return nextDeviceCode.ToString("D8");
    }

    public static IEnumerable<string> CreateDeviceCode(int deviceCount, int deviceCountCreate)
    {
        foreach(var i in Enumerable.Range(0, deviceCountCreate))
        {
            // Increment the count by 1 to get the next device code
            int nextDeviceCode = deviceCount + i + 1;

            // Format the device code as an 8-digit string with leading zeros
            yield return nextDeviceCode.ToString("D8");
        }
    }
}