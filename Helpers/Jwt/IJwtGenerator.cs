namespace LearnHttpContext.Helpers.Jwt;

public interface IJwtGenerator
{
    Task<string>Generate(string userId, string email, string[] roleIds);
}