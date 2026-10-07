using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using GlobeLife.Api.Domain;

namespace GlobeLife.Api.Application;

public sealed record LoginCommand(string Username, string Password);
public sealed record AuthenticatedUser(string Username, string DisplayName, string Role);
public sealed record LoginResult(string AccessToken, AuthenticatedUser User);

public interface ICredentialRepository { AuthenticatedUser? Verify(LoginCommand command); }
public sealed class DemoCredentialRepository : ICredentialRepository
{
    private static readonly IReadOnlyDictionary<string, (string DisplayName, string Role, string PasswordHash)> DemoCredentials =
        new Dictionary<string, (string, string, string)>(StringComparer.OrdinalIgnoreCase)
        {
            ["alex.customer"] = ("Alex Morgan", "Customer", PasswordHasher.Hash("CarePortal!2026")),
            ["maya.support"] = ("Maya Support", "Support", PasswordHasher.Hash("SupportDesk!2026"))
        };

    public AuthenticatedUser? Verify(LoginCommand command)
    {
        if (!DemoCredentials.TryGetValue(command.Username, out var credential) ||
            !PasswordHasher.Verify(command.Password, credential.PasswordHash)) return null;
        return new AuthenticatedUser(command.Username, credential.DisplayName, credential.Role);
    }
}

public sealed class PasswordHasher
{
    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var derived = Rfc2898DeriveBytes.Pbkdf2(password, salt, 120_000, HashAlgorithmName.SHA256, 32);
        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(derived)}";
    }

    public static bool Verify(string password, string storedHash)
    {
        var parts = storedHash.Split(':');
        if (parts.Length != 2) return false;
        var salt = Convert.FromBase64String(parts[0]);
        var expected = Convert.FromBase64String(parts[1]);
        var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, 120_000, HashAlgorithmName.SHA256, 32);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}

public sealed class JwtAccessTokenIssuer(IConfiguration configuration)
{
    public LoginResult Issue(AuthenticatedUser user)
    {
        var secret = configuration["Jwt:SigningKey"] ?? throw new InvalidOperationException("Jwt:SigningKey is required.");
        var claims = new[] { new Claim(ClaimTypes.Name, user.Username), new Claim(ClaimTypes.Role, user.Role), new Claim("display_name", user.DisplayName) };
        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddHours(2), signingCredentials: credentials);
        return new LoginResult(new JwtSecurityTokenHandler().WriteToken(token), user);
    }
}
