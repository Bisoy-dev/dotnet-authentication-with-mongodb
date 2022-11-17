using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using LearnHttpContext.Services.UserServices;

namespace LearnHttpContext.Helpers.Jwt;

public class JwtGenerator : IJwtGenerator
{
    private readonly IConfiguration configuration;
    private readonly IRoleService roleService;

    public JwtGenerator(
        IConfiguration configuration,
        IRoleService roleService)
    {
        this.configuration = configuration;
        this.roleService = roleService;
    }
    public async Task<string> Generate(string userId, string email, string[] roleIds)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)),
            SecurityAlgorithms.HmacSha256
        );

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        if(roleIds.Any())
        {
            // claims.AddRange(roleIds.Select(r => new Claim(ClaimTypes.Role, r)));
            foreach(var roleId in roleIds)
            {
                var role = await roleService.GetById(roleId);
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }
        }

        var securityToken = new JwtSecurityToken(
            issuer : "HttpContext",
            expires : DateTime.Now.AddHours(1),
            claims : claims,
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}