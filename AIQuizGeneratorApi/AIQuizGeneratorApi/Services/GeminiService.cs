using AIQuizGeneratorApi.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AIQuizGeneratorApi.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GeminiService(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            //_httpClient.DefaultRequestHeaders.Authorization =
            //    new AuthenticationHeaderValue("Bearer", _configuration["Gemini:ApiKey"]);
        }

        public async Task<string> GenerateQuizAsync(QuizRequest request)
        {
            var model = "models/gemini-1.5-flash-latest";
            var apiKey = _configuration["Gemini:ApiKey"];
            var url = $"https://generativelanguage.googleapis.com/v1beta/{model}:generateContent?key={apiKey}";

            string prompt = $@"
            Generate {request.NumberOfQuestions} {request.Type} quiz question(s) on the topic '{request.Topic}'.

            Respond only with a raw JSON array like this — **no markdown, no explanations**:

            [
              {{
                ""question"": ""What is HTML?"",
                ""options"": [""HyperText Markup Language"", ""Hyper Tool Machine Language"", ""Home Text Machine Language"", ""None of the above""],
                ""correctAnswer"": ""HyperText Markup Language""
              }}
            ]
            ";




            var requestBody = new
            {
                contents = new[]
                {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Gemini error {response.StatusCode}: {responseContent}");

            return responseContent;
        }

        private string BuildPrompt(QuizRequest request)
        {
            var typeText = request.Type == "yesno" ? "yes or no questions" : "multiple choice questions with 4 options";
            return $"Generate {request.NumberOfQuestions} {typeText} about the topic: {request.Topic}. " +
                   "Respond in JSON array format with each item containing 'question', 'options' (if MCQ), and 'answer'.";
        }
    }

}
