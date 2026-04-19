using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace FinanceEdgeTrack.Application.Common.Pagination.Filters;

public class PaginationHeaderFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.Result is not ObjectResult objectResult)
            return;

        var value = objectResult.Value;

        if (value == null)
            return;

        var pagedList = ExtractPagedList(value);

        if (pagedList == null)
            return;

        var metadata = new PaginationMetadata
        {
            CurrentPage = pagedList.PageNumber,
            PageSize = pagedList.PageSize,
            TotalCount = pagedList.TotalCount,
            TotalPages = pagedList.TotalPages,
            HasNext = pagedList.HasNext,
            HasPrevious = pagedList.HasPrevious
        };

        context.HttpContext.Response.Headers.Append(
            $"X-Pagination: {metadata.CurrentPage}; {metadata.TotalPages}; {metadata.HasPrevious}; {metadata.HasNext}",
            JsonSerializer.Serialize(metadata));
    }

    private IPagedList? ExtractPagedList(object? value)
    {
        if (value is null)
            return null;

        // Caso 1: retorno direto PagedList<T>
        if (value is IPagedList pagedList)
        {
            return pagedList;
        }

        // Caso 2: ApiResponse<PagedList<T>>
        var type = value.GetType();

        var dataProperty = type.GetProperty("Data");

        if (dataProperty is null)
            return null;

        var dataValue = dataProperty.GetValue(value);
        
        if(dataValue is IPagedList innerPagedList)
            return innerPagedList;

        return null;
    }
}
