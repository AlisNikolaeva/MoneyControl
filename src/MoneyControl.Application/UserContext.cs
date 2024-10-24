namespace MoneyControl.Application;

public static class UserContext
{
    public static Guid UserId => _asyncLocalUserContext.Value!.UserId;
    private static AsyncLocal<UserDetails> _asyncLocalUserContext = new();
    public static void SetUserContext(Guid userId)
    {
        _asyncLocalUserContext.Value = new UserDetails { UserId = userId };
    }
}