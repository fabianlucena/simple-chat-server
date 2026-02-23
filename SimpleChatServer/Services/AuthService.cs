using SimpleChatServer.DTO;
using SimpleChatServer.Entities;

namespace SimpleChatServer.Services
{
    public class AuthService
    {
        static Dictionary<string, User> Tokens = [];

        public LoginResult Login(LoginRequest request)
        {
            var userService = new UserService();
            var user = userService.GetByUsername(request.Username);

            if (user == null)
                return new LoginResult("", "Usuario desconocido");

            if (user.Password != request.Password)
                return new LoginResult("", "Error de credenciales");

            string token = Guid.NewGuid().ToString().Replace("-", "");
            Tokens[token] = user;

            return new LoginResult(token);
        }

        public User? GetUserByToken(string token)
        {
            if (Tokens.TryGetValue(token, out var user))
                return user;

            return null;
        }
    }
}
