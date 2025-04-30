namespace ThinkWorld.Services;

public interface IUserIdGenerator
{
    string ComputeUserId(string email);
}