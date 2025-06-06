namespace ZEN.Domain.Common.Abstractions;

/// <summary>
/// Represents the marker interface for expire entities.
/// </summary>
public interface IExpireEntity
{
    DateTimeOffset? ExpireAt { get; set; }

}