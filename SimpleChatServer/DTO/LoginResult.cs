namespace SimpleChatServer.DTO
{
    public record LoginResult(string Token, string Error = "");
}
