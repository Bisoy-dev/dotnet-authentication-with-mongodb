using LearnHttpContext.Models;

namespace LearnHttpContext.Services.UserServices;

public class RoleService : IRoleService
{
    private readonly IMongoCollection<Role> _roleCollection;
    public const string ROLE_COLLECTION = "role";
    public RoleService(IMongoDatabase db)
    {
        _roleCollection = db.GetCollection<Role>(ROLE_COLLECTION);
    }
    public async Task<Role> Create(Role role)
    {
        await _roleCollection.InsertOneAsync(role);
        return role;
    }

    public async Task<IEnumerable<Role>> GetAll()
    {
        var roles = await (await _roleCollection.FindAsync(_ => true)).ToListAsync();
        return roles;
    }

    public async Task<Role> GetById(string id)
    {
        return await (await _roleCollection.FindAsync(Builders<Role>.Filter.Eq(r => r.RoleId, id)))
            .FirstOrDefaultAsync();
    }

    public async Task<bool> IsRoleExist(string name)
    {
        return await (await _roleCollection.FindAsync(Builders<Role>.Filter.Eq(r => r.Name, name))).AnyAsync();
    }
}