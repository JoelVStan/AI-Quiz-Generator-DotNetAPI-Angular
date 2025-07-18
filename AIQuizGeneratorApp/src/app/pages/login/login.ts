import { Component } from '@angular/core';
import { Validators, FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../services/auth';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  imports: [CommonModule,
    ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css',
  standalone: true
})
export class Login {
  loginForm: ReturnType<FormBuilder['group']>;

  constructor(private fb: FormBuilder, private auth: Auth, private router: Router, private toastr: ToastrService) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }
  loading = false;
  loginSuccess = false; 

  onSubmit() {
    if (this.loginForm.invalid) return;
    this.loading = true;
    this.auth.login(this.loginForm.value).subscribe({
      next: () => {
      this.loginSuccess = true;
      this.loading = false;
      // Navigate to quiz page after brief message
      setTimeout(() => this.router.navigate(['/home']), 2000);
      },
      error: () => {
        this.loading = false;
        this.toastr.error('Invalid credentials');
      }
    });
  }
}
