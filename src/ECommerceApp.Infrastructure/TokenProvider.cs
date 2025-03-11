using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ECommerceApp.Infrastructure;

public sealed class TokenProvider
{
    private readonly IConfiguration configuration;

    public TokenProvider(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public string Create(IdentityUser user, IEnumerable<string> userRoles)
    {
        string secretKey = configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT Secret is not configured.");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Create base claims
        var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

        // Add role claims (using ClaimTypes.Role)
        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Read expiration from configuration (defaulting to 60 minutes if invalid)
        int expirationInMinutes = int.Parse(configuration["Jwt:ExpirationInMinutes"] ?? "60");
        if (expirationInMinutes <= 0)
        {
            expirationInMinutes = 60;
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
            SigningCredentials = credentials,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"]
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }
}
