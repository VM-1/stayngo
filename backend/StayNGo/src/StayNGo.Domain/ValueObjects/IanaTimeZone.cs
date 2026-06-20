namespace StayNGo.Domain.ValueObjects;

public sealed record IanaTimeZone
{
    public string Value { get; init; } = null!;

    // EF Core uses this via reflection. Do not call from application code.
    private IanaTimeZone()
    {
    }

    public IanaTimeZone(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("Time zone required.", nameof(value));
        }

        if (!TimeZoneInfo.TryFindSystemTimeZoneById(value, out _))
        {
            throw new ArgumentException($"'{value}' is not a valid IANA time zone.", nameof(value));
        }

        Value = value;
    }

    public static IanaTimeZone? From(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : new IanaTimeZone(value);

    public TimeZoneInfo ToTimeZoneInfo() => TimeZoneInfo.FindSystemTimeZoneById(Value);
    public override string ToString() => Value;
}