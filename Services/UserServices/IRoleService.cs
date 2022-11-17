using LearnHttpContext.Models;

namespace LearnHttpContext.Services.UserServices;

public interface IRoleService
{
    Task<Role> Create(Role role);
    Task<IEnumerable<Role>> GetAll();
    Task<Role> GetById(string id);

    Task<bool> IsRoleExist(string name);

}