using MoneyControl.Application;

namespace MoneyControl.Server;

public class UserContextMiddleware
{
    private readonly RequestDelegate _next;

    public UserContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Context.SetUserContext(new Guid("560DCD61-E75C-4671-AB88-AC0057C3252B"));
        await _next.Invoke(context);
    }
}