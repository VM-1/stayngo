using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StayNGo.Domain.ValueObjects;

namespace StayNGo.Infrastructure.Persistence.Converters;

public sealed class IanaTimeZoneConverter()
    : ValueConverter<IanaTimeZone, string>(tz => tz.Value, s => new IanaTimeZone(s));