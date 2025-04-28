using System.Text;

namespace ThinkWorld.Services;

public class UserIdGenerator : IUserIdGenerator
{
    public string ComputeUserIdAsync(string email)
    {
        var bytes = Encoding.UTF8.GetBytes(email.ToLower().Trim());
        using var hasher = System.Security.Cryptography.SHA256.Create();
        
        return Encoding.UTF8.GetString(hasher.ComputeHash(bytes));
    }
}