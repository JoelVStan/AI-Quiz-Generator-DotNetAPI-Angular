using AIQuizGeneratorApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AIQuizGeneratorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateQuiz([FromBody] QuizRequest request)
        {
            var prompt = $"Generate {request.NumberOfQuestions} {request.Type.ToUpper()} quiz questions on {request.Topic}. " +
                         $"Each question should have 4 options and clearly mention the correct answer. " +
                         $"Return a JSON array like: " +
                         "[{{\"question\": \"...\", \"options\": [...], \"correctAnswer\": \"...\"}}]. If using a wrapper, use {{\"questions\": [...]}}.";

            var ollamaRequest = new
            {
                model = "gemma:2b",
                messages = new[]
                {
            new { role = "system", content = "You are a helpful assistant that returns clean JSON quiz questions only." },
            new { role = "user", content = prompt }
        },
                stream = false
            };

            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync("http://localhost:11434/api/chat", ollamaRequest);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Failed to generate quiz from Ollama.");

            var responseJson = await response.Content.ReadAsStringAsync();

            try
            {
                // Parse Ollama wrapper
                var wrapper = JsonSerializer.Deserialize<OllamaChatResponse>(responseJson);
                var rawContent = wrapper?.Message?.Content?.Trim();

                if (string.IsNullOrWhiteSpace(rawContent))
                    return StatusCode(500, "Empty AI response received.");

                // Remove code fences
                if (rawContent.StartsWith("```"))
                {
                    rawContent = rawContent
                        .Replace("```json", "")
                        .Replace("```", "")
                        .Trim();
                }

                // Try parsing directly as array
                try
                {
                    var directArray = JsonSerializer.Deserialize<List<QuizQuestion>>(rawContent);
                    return Ok(directArray);
                }
                catch { }

                // Try parsing as object with "questions" array
                try
                {
                    using var doc = JsonDocument.Parse(rawContent);
                    if (doc.RootElement.TryGetProperty("questions", out var questionsElement))
                    {
                        var questions = JsonSerializer.Deserialize<List<QuizQuestion>>(questionsElement.GetRawText());
                        return Ok(questions);
                    }
                }
                catch { }

                // Try parsing single object
                try
                {
                    var single = JsonSerializer.Deserialize<QuizQuestion>(rawContent);
                    return Ok(new List<QuizQuestion> { single });
                }
                catch { }

                return StatusCode(500, $"Unable to parse quiz questions.\nRaw: {rawContent}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error parsing AI response: {ex.Message}\nRaw: {responseJson}");
            }
        }



    }

}
