namespace AIQuizGeneratorApi.Models
{
    public class QuizRequest
    {
        public required string Topic { get; set; }
        public int NumberOfQuestions { get; set; }
        public required string Type { get; set; } // "mcq" or "yesno"
    }
}
