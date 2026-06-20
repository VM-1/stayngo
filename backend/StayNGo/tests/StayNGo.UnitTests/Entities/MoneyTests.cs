using FluentAssertions;
using StayNGo.Domain.ValueObjects;

namespace StayNGo.UnitTests.Entities;

public class MoneyTests
{
    [Fact]
    public void Ctor_WithValidAmountAndCurrency_CreatesInstance()
    {
        var m = new Money(12345, "EUR");
        
        m.Currency.Should().Be("EUR");
        m.AmountCents.Should().Be(12345);
    }

    [Fact]
    public void Ctor_WithEmptyCurrency_Throws()
    {
        Action act = () => new Money(12345, "");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Ctor_WithInvalidCurrency_Throws()
    {
        Action act = () => new Money(12345, "CurrencyNameLongerThen3");
        act.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void Ctor_WithNegativeAmount_Throws()
    {
        Action act = () => new Money(-1, "EUR");
        act.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void Ctor_WithZeroAmount_Accepts()
    {
        var m = new Money(0, "EUR");
        m.AmountCents.Should().Be(0);
    }

    [Fact]
    public void Ctor_LowercaseCurrency_ConvertsToUppercase()
    {
        var m = new Money(12345, "eur");
        m.Currency.Should().Be("EUR");
    }

}