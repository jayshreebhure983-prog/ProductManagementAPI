using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProductManagement.Domain.Exceptions;

namespace ProductManagementAPI.Filters;

public class ValidationExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not FluentValidation.ValidationException validationException)
            return;

        var errors = validationException.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(x => x.ErrorMessage).ToArray());

        context.Result = new BadRequestObjectResult(new
        {
            Success = false,
            Message = "Validation failed.",
            Errors = errors
        });

        context.ExceptionHandled = true;
    }
}
