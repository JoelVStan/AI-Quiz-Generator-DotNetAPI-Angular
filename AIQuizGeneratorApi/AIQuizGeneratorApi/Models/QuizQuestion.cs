using System.Text.Json.Serialization;

namespace AIQuizGeneratorApi.Models
{
    public class QuizQuestion
    {
        [JsonPropertyName("question")]
        public string Question { get; set; }

        [JsonPropertyName("options")]
        public List<string> Options { get; set; }

        [JsonPropertyName("correctAnswer")]
        public string Answer { get; set; }

    }
}
