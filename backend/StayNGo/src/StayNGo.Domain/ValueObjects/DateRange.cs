namespace StayNGo.Domain.ValueObjects;

public readonly record struct DateRange
{
    public DateOnly Start { get;  }
    public DateOnly EndExclusive { get;  }

    public DateRange(DateOnly start, DateOnly endExclusive)
    {
        if (start >= endExclusive)
        {
            throw new ArgumentException("Start date must be before end date");
        }
        Start = start;
        EndExclusive = endExclusive;
    }
}