namespace StayNGo.Domain.ValueObjects;

public sealed record Money
{
    public long AmountCents { get; init; }
    public string Currency { get; init; } = null!;

    // EF Core uses this via reflection. Do not call from application code.
    private Money() { }
    
    public Money(long amountCents, string currency )
    {
        if (string.IsNullOrEmpty(currency))
        {
            throw new ArgumentException("Currency required.", nameof(currency));
        }

        if (currency.Length != 3)
        {
            throw new ArgumentException("Currency must be a 3-letter ISO 4217 code.", nameof(currency));
        }

        if (amountCents < 0)
        {
            throw new ArgumentException("Amount cannot be negative.", nameof(amountCents));
        }

        AmountCents = amountCents;
        Currency = currency.ToUpperInvariant();
    }

    public static Money Zero(string currency) => new(0,currency);

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(AmountCents + other.AmountCents,Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(AmountCents - other.AmountCents,Currency);
    }

    public Money Multiply(int factor) => new(AmountCents * factor,Currency);

    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator -(Money left, Money right) => left.Subtract(right);
    public static Money operator *(Money left, int factor) => left.Multiply(factor);

    private void EnsureSameCurrency(Money other)
    {
        if (other.Currency != Currency)
        {
            throw new InvalidOperationException($"Cannot operate on {Currency} and {other.Currency}.");
        }
    }
}