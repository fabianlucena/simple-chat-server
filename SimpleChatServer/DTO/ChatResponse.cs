namespace SimpleChatServer.DTO
{
    public class ChatResponse
    {
        public DateTime DateTime { get; set; } = DateTime.Now;
        public string Type { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
