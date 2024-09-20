using FluentValidation;

namespace MoneyControl.Server;

public class ValidationExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (ValidationException e)
        {
            var errors = e.Errors;
            var result = errors.GroupBy(x => x.PropertyName, y => y.ErrorMessage)
                .ToDictionary(x => x.Key, y => y);
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(result);
        }
    }
}