using AIQuizGeneratorApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace AIQuizGeneratorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExplanationController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ExplanationController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult<ExplanationResponse>> Post([FromBody] ExplanationRequest request)
        {
            var apiKey = _configuration["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return StatusCode(500, "Gemini API key is missing.");
            }

            var client = _httpClientFactory.CreateClient();
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent?key={apiKey}";

            var payload = new
            {
                contents = new[]
                {
                new {
                    parts = new[]
                    {
                        new { text = request.Prompt }
                    }
                }
            }
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, $"Failed to fetch explanation: {error}");
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            var json = await JsonSerializer.DeserializeAsync<JsonElement>(stream);

            var explanation = json
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return Ok(new ExplanationResponse { Explanation = explanation });
        }
        //[HttpPost]
        //public IActionResult GetExplanation([FromBody] PromptRequest request)
        //{
        //    if (string.IsNullOrWhiteSpace(request.Prompt))
        //        return BadRequest("Prompt is required.");

        //    // Replace this with your Gemini or AI explanation logic
        //    string explanation = $"Short explanation for: {request.Prompt}";

        //    return Ok(new { explanation });
        //}

        //public class PromptRequest
        //{
        //    public string Prompt { get; set; }
        //}
    }
}
