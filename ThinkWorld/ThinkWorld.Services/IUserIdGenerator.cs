namespace ThinkWorld.Services;

public interface IUserIdGenerator
{
    string ComputeUserIdAsync(string email);
}