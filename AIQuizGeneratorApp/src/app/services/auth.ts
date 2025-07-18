import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, tap } from 'rxjs';
import { TOKEN_KEY } from './constants';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class Auth {
  private baseUrl = 'https://localhost:7180/api'; // change if needed
  isLoggedIn$ = new BehaviorSubject<boolean>(false);

  constructor(private http: HttpClient,private router: Router) {}

  signup(data: any) {
    return this.http.post(`${this.baseUrl}/signup`, data);
  }

  login(data: any) {
    return this.http.post<{ token: string }>(`${this.baseUrl}/login`, data).pipe(
      tap((res) => {
        localStorage.setItem(TOKEN_KEY, res.token);
        this.isLoggedIn$.next(true);

        this.router.navigate(['/']);
      })
    );
  }

  logout() {
    localStorage.removeItem(TOKEN_KEY);
    this.isLoggedIn$.next(false);
  }

  getToken() {
    return localStorage.getItem(TOKEN_KEY);
  }
}
