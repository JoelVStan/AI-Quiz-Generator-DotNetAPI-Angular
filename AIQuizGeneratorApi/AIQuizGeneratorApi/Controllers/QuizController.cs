using AIQuizGeneratorApi.Models;
using AIQuizGeneratorApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AIQuizGeneratorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly GeminiService _geminiService;

        public QuizController(GeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateQuiz([FromBody] QuizRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Topic) || request.NumberOfQuestions < 1)
                return BadRequest("Topic and number of questions are required.");

            try
            {
                var geminiResponse = await _geminiService.GenerateQuizAsync(request);

                using var doc = JsonDocument.Parse(geminiResponse);
                var content = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();
                Console.WriteLine("Gemini raw response:");
                Console.WriteLine(content);
                // Strip markdown formatting
                if (content.StartsWith("```"))
                {
                    content = content.Trim().Trim('`'); // Remove code fences
                    int firstBrace = content.IndexOf('[');
                    content = content.Substring(firstBrace);
                }



                try
                {
                    var questions = JsonSerializer.Deserialize<List<QuizQuestion>>(content);
                    return Ok(questions);
                }
                catch (JsonException jsonEx)
                {
                    return StatusCode(500, $"JSON parse error: {jsonEx.Message}\nRaw content: {content}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to generate quiz: {ex.Message}");
            }
        }
    }

}
