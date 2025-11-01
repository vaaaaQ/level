using Bybit.Net.Enums;

namespace level.Options;

public sealed class BybitOptions
{
    public const string SectionName = "Bybit";

    public string ApiKey { get; init; } = string.Empty;

    public string ApiSecret { get; init; } = string.Empty;

    public Category Category { get; init; } = Category.Linear;

    public string? OrderLinkPrefix { get; init; }
}
