namespace MoneyControl.Application;

public static class Context
{
    public static UserContext UserContext => _asyncLocalUserContext.Value!;
    private static AsyncLocal<UserContext> _asyncLocalUserContext = new();
    public static void SetUserContext(Guid userId)
    {
        _asyncLocalUserContext.Value = new UserContext { UserId = userId };
    }
}