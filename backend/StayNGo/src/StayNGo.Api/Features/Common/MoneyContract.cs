using StayNGo.Domain.ValueObjects;

namespace StayNGo.Api.Features.Common;

public record MoneyContract(long AmountCents, string Currency)
{
    public static MoneyContract? From(Money? m) => m is null ? null : new MoneyContract(m.AmountCents, m.Currency);
    public Money ToDomain() => new Money(AmountCents, Currency);
}