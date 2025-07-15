import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { QuizQuestion } from '../models/quiz-question.model';
import { QuizRequest } from '../models/quiz-request.model';

@Injectable({
  providedIn: 'root'
})
export class Quiz {
  private apiUrl = 'https://localhost:7180/api/quiz/generate';

  constructor(private http: HttpClient) {}

  generateQuiz(request: QuizRequest): Observable<QuizQuestion[]> {
    return this.http.post<QuizQuestion[]>(this.apiUrl, request);
  }
  getExplanation(prompt: string): Observable<string> {
  return this.http
    .post<{ explanation: string }>('https://localhost:7180/api/explanation', { prompt })
    .pipe(map(res => res.explanation));
}



}
