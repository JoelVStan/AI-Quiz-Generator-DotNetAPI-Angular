using System.Text.Json.Serialization;

namespace AIQuizGeneratorApi.Models
{
    public class OllamaChatResponse
    {
        [JsonPropertyName("message")]
        public OllamaMessage Message { get; set; }
    }

    public class OllamaMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }


}
