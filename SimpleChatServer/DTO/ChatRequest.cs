namespace SimpleChatServer.DTO
{
    public class ChatRequest
    {
        public string Command { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
