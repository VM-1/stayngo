using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NpgsqlTypes;
using StayNGo.Domain.ValueObjects;

namespace StayNGo.Infrastructure.Persistence.Converters;

public class DateRangeConverter() : ValueConverter<DateRange, NpgsqlRange<DateOnly>>(domain =>
        new NpgsqlRange<DateOnly>(
            domain.Start, true,
            domain.EndExclusive, false),
    npg => new DateRange(npg.LowerBound, npg.UpperBound));