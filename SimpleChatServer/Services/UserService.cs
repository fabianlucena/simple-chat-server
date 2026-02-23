using SimpleChatServer.Entities;

namespace SimpleChatServer.Services
{
    public class UserService
    {
        static private readonly List<User> Users =
        [
            new User
            {
                Id = 1,
                Username = "adam",
                Password = "1234",
            },
        ];

        public User? GetByUsername(string username)
            => Users.FirstOrDefault(u => u.Username == username);
    }
}
