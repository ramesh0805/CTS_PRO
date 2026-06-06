namespace MQTTR1.Utilities;

public static class PagingHelper
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public static (int Page, int PageSize) ValidatePagingParameters(int? page, int? pageSize)
    {
        var validPage = page.HasValue && page.Value > 0 ? page.Value : 1;
        var validPageSize = pageSize.HasValue && pageSize.Value > 0 
            ? Math.Min(pageSize.Value, MaxPageSize) 
            : DefaultPageSize;

        return (validPage, validPageSize);
    }
}
