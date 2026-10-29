namespace SharedKernel;

public interface ICurrentUserService
{
    Guid UserId();
    string Phone { get; }
    string Role { get; }
    bool IsAdmin { get; }
    bool IsDriver { get; }
    bool IsSender { get; }
    IEnumerable<string> GetGroups();
}
