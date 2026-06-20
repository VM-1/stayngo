namespace StayNGo.Api.Utils;

public static class MoneyConvertor
{
    public static long? FromMajorUnits(decimal? majorUnits) =>
        majorUnits.HasValue ? (long)Math.Round(majorUnits.Value * 100m, MidpointRounding.AwayFromZero) : null;
}