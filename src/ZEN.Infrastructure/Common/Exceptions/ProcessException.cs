namespace ZEN.Infrastructure.Common.Exceptions;

public sealed class ProcessException(string? message) : Exception(message)
{
}