export interface QuizQuestion {
  question: string;
  options: string[];
  correctAnswer: string;
  selectedAnswer?: string;
  isAnswered?: boolean;
  isCorrect?: boolean;
  explanation?: string; 
}
