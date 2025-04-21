using System.Text.Json.Serialization;

namespace ChatBotAPI.Models
{
    public class ChatMessage
    {
        [JsonPropertyName("Message")]
        public string Message { get; set; }

        [JsonPropertyName("guidId")]
        public string GuidId { get; set; }

    }
}
