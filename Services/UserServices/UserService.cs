using LearnHttpContext.Models;

namespace LearnHttpContext.Services.UserServices;


public class UserService : IUserService
{
    private readonly IMongoCollection<User> _userCollection;
    public const string USER_COLLECTION = "user";
    public UserService(IMongoDatabase db)
    {
        _userCollection = db.GetCollection<User>(USER_COLLECTION);
    }
    public async Task<UserResult> Create(User user)
    {
        user.DateCreated = user.DateCreated == DateTime.MinValue ? DateTime.UtcNow : user.DateCreated;
        
        await _userCollection.InsertOneAsync(user);
        return new UserResult(user.UserId, user.Email, user.DateCreated, user.Roles.ToList());
    }

    public async Task<UserResult> GetUserById(string id)
    {
        var users = await _userCollection.FindAsync(Builders<User>.Filter.Eq(u => u.UserId, id));
        var user = await users.FirstOrDefaultAsync();
        return new UserResult(user.UserId, user.Email, user.DateCreated, user.Roles.ToList());
    }

    public async Task<bool> IsEmailUnique(string email)
    {
        var users = await _userCollection.FindAsync(Builders<User>.Filter.Eq(u => u.Email, email));
        return !await users.AnyAsync();
    }

    public async Task<User> FindByEmail(string email)
    {
        var users = await _userCollection.FindAsync(Builders<User>.Filter.Eq(u => u.Email, email));
        return await users.FirstOrDefaultAsync();
    }
}