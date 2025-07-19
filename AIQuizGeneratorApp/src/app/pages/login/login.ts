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
  loading = false;
  loginSuccess = false;
  loginError = false; // add this

  constructor(private fb: FormBuilder, private auth: Auth, private router: Router, private toastr: ToastrService) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  onSubmit() {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched(); // show validation errors
      return;
    }

    this.loading = true;
    this.loginError = false;

    this.auth.login(this.loginForm.value).subscribe({
      next: () => {
        this.loginSuccess = true;
        this.loading = false;
        setTimeout(() => this.router.navigate(['/home']), 2000);
      },
      error: () => {
        this.loading = false;
        this.loginError = true;
        this.toastr.error('Invalid credentials');
      }
    });
  }
}

