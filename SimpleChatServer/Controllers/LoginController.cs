using Microsoft.AspNetCore.Mvc;
using SimpleChatServer.DTO;
using SimpleChatServer.Services;

namespace SimpleChatServer.Controllers
{
    [ApiController]
    [Route("api/login")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var authService = new AuthService();
            var result = authService.Login(request);

            if (result.Error != "")
            {
                return BadRequest(new { message = result.Error });
            }

            return Ok(new { token = result.Token });
        }
    }
}
