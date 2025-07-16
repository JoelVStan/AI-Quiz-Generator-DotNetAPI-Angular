import { Component } from '@angular/core';
import { QuizQuestion } from '../../models/quiz-question.model';
import { Quiz } from '../../services/quiz';
import { QuizRequest } from '../../models/quiz-request.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-home',
  imports: [CommonModule,FormsModule],
  templateUrl: './home.html',
  styleUrls: ['./home.css']
})
export class Home {
  topic: string = '';
  numberOfQuestions = 5;
  type = 'mcq';
  questions: QuizQuestion[] = [];
  selectedAnswers: string[] = [];
  loading = false;
  error = '';

  constructor(private quizService: Quiz) {}

  generateQuiz() {
    this.loading = true;
    this.error = '';
    const request: QuizRequest = {
      topic: this.topic,
      numberOfQuestions: this.numberOfQuestions,
      type: this.type
    };

    this.quizService.generateQuiz(request).subscribe({
      next: (res) => {
        this.questions = res;
        this.selectedAnswers = new Array(this.questions.length).fill('');
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;

        const rawError = err?.error?.toString() || '';

        if (rawError.includes('ServiceUnavailable') || rawError.includes('503')) {
          this.error = 'AI server is currently busy. Please try again shortly.';
        } else {
          this.error = 'Failed to generate quiz. Please try again.';
        }

        console.error('Error generating quiz:', err);
      }
    });
  }

  // generateQuiz() {
  //   this.loading = true;
  //   this.error = '';

  //   // ⛔ Skip actual API call
  //   setTimeout(() => {
  //     this.questions = [
  //       {
  //         question: 'Which HTML tag is used to create a hyperlink?',
  //         options: ['<link>', '<a>', '<href>', '<hyperlink>'],
  //         correctAnswer: '<a>'
  //       },
  //       {
  //         question: 'What does HTML stand for?',
  //         options: [
  //           'HyperText Markup Language',
  //           'Home Tool Markup Language',
  //           'Hyper Transfer Markup Language',
  //           'Hyperlink and Text Markup Language'
  //         ],
  //         correctAnswer: 'HyperText Markup Language'
  //       }
  //     ];
  //     this.loading = false;
  //   }, 1000);
  // }


  resetForm() {
    this.topic = '';
    this.numberOfQuestions = 5;
    this.type = 'mcq';
    this.questions = [];
    this.error = '';
    this.score = 0;
    this.submitted = false;
    window.location.reload();
  }

  checkAnswer(q: QuizQuestion) {
    q.isAnswered = true;
    q.isCorrect = q.selectedAnswer === q.correctAnswer;
  }
  score = 0;
  submitted = false;
  resultMessage: string = '';


  submitQuiz() {
    this.score = 0;

    this.questions.forEach((q, index) => {
      const selected = this.selectedAnswers[index];
      q.selectedAnswer = selected;
      q.isCorrect = selected === q.correctAnswer;

      if (q.isCorrect) {
        this.score++;
      } else {
        // 🔥 Only generate explanation for incorrect answers
        this.generateExplanation(q);
      }
    });

    const percentage = (this.score / this.questions.length) * 100;

    if (this.score === this.questions.length) {
      this.resultMessage = "🎉 Perfect Score! You're a genius!";
    } else if (percentage >= 80) {
      this.resultMessage = "💪 Great job! You really know your stuff!";
    } else if (percentage > 40) {
      this.resultMessage = "🙂 Good effort! There's room to improve.";
    } else if (percentage > 0) {
      this.resultMessage = "😅 Keep practicing, you'll get better!";
    } else {
      this.resultMessage = "🤔 Seems like you need to study the topic.";
    }

    this.submitted = true;
  }


  refreshPage() {
    window.location.reload();
  }

  currentYear = new Date().getFullYear();

  darkMode = false;

  ngOnInit() {
    const saved = localStorage.getItem('darkMode');
    if (saved === 'true') {
      this.darkMode = true;
      this.toggleDarkMode();
    }
  }

  toggleDarkMode() {
    const parentEl = document.querySelector('.parent');
    if (!parentEl) return;

    if (this.darkMode) {
      parentEl.classList.add('dark-mode');
      localStorage.setItem('darkMode', 'true');
    } else {
      parentEl.classList.remove('dark-mode');
      localStorage.setItem('darkMode', 'false');
    }
  }

  generateExplanation(question: QuizQuestion) {
  const prompt = `Explain in 2-3 short sentences why "${question.correctAnswer}" is the correct answer for the question: "${question.question}". Keep it concise and simple.`;


  this.quizService.getExplanation(prompt).subscribe({
    next: (res) => {
      question.explanation = res.explanation; // ✅ this works now
    },
    error: (err) => {
      question.explanation = 'Explanation not available at the moment.';
    }
  });

}



}