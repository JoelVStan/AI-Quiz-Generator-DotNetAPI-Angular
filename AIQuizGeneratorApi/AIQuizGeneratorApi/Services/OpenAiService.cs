using AIQuizGeneratorApi.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AIQuizGeneratorApi.Services
{
    public class OpenAiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public OpenAiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GenerateQuizAsync(QuizRequest request)
        {
            var apiKey = _configuration["OpenAI:ApiKey"];
            var model = _configuration["OpenAI:Model"];
            var numQuestions = Math.Min(request.NumberOfQuestions, 30);

            var prompt = BuildPrompt(request.Topic, numQuestions, request.Type);

            var requestBody = new
            {
                model = model,
                messages = new[]
                {
                new { role = "system", content = "You are an AI quiz generator." },
                new { role = "user", content = prompt }
            },
                temperature = 0.7
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            httpRequest.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"OpenAI error {response.StatusCode}: {responseBody}");
            }

            return responseBody;
        }

        private string BuildPrompt(string topic, int count, string type)
        {
            if (type.ToLower() == "yesno")
            {
                return $$"""
                Generate {{count}} Yes/No quiz questions on the topic "{{topic}}".
                Return as JSON format like this:
                [
                    {
                        "question": "Is JavaScript a compiled language?",
                        "options": ["Yes", "No"],
                        "answer": "No"
                    }
                ]
                """;
            }

            return $$"""
            Generate {{count}} multiple-choice quiz questions on the topic "{{topic}}".
            Each question should have 4 options and one correct answer.
            Return as JSON format like this:
            [
                {
                    "question": "What does HTML stand for?",
                    "options": ["Hyper Text Markup Language", "Home Tool Markup Language", "Hyperlinks and Text Markup Language", "Hyper Trainer Marking Language"],
                    "answer": "Hyper Text Markup Language"
                }
            ]
            """;
        }
    }
}
