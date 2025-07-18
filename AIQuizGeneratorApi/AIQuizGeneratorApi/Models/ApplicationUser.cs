using Microsoft.AspNetCore.Identity;

namespace AIQuizGeneratorApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public int QuizCount { get; set; } = 0;
    }
}
