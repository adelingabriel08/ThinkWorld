using System.Text;

namespace ThinkWorld.Services;

public class UserIdGenerator : IUserIdGenerator
{
    public string ComputeUserIdAsync(string email)
    {
        var bytes = Encoding.UTF8.GetBytes(email.ToLower().Trim());
        using var hasher = System.Security.Cryptography.SHA256.Create();
        
        StringBuilder sb = new StringBuilder();
        foreach (byte b in hasher.ComputeHash(bytes))
            sb.Append(b.ToString("X2"));

        return sb.ToString();
    }
}