using System.Text.Json.Serialization;

namespace WebApplication1.Data
{
    public class aiAssist
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("message")]
        public ChatMessageContent Message { get; set; }

        [JsonPropertyName("done")]
        public bool Done { get; set; }
    }

    public class ChatMessageContent
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
