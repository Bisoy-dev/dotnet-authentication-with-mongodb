using LearnHttpContext.Models;
namespace LearnHttpContext.Services.UserServices;

public interface IUserService
{
    Task<UserResult> GetUserById(string id);
    Task<UserResult> Create(User user);
    Task<bool> IsEmailUnique(string email);
    Task<User> FindByEmail(string email);
}

public record UserResult(string Id, string Email, DateTime DateCreated, List<string> Roles);