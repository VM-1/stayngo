using FluentAssertions;

namespace StayNGo.UnitTests;

public class SmokeTest
{
    [Fact]
    public void TestHarnessIsWired()
    {
        (1 + 1).Should().Be(2);
    }
}
