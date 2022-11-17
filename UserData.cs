using LearnHttpContext.Models;

namespace LearnHttpContext;

public class UserData
{
    private static List<User> Users = new();
    public static IEnumerable<User> GetUsers => Users;
    public static void Add(User user) => Users.Add(user);
}