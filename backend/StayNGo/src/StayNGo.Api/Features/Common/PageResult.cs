namespace StayNGo.Api.Features.Common;

public record PageResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int Total);