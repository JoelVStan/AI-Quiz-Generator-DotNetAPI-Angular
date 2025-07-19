import { Component } from '@angular/core';
import { Validators, FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../services/auth';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-signup',
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  templateUrl: './signup.html',
  styleUrl: './signup.css',
  standalone: true
})
export class Signup {
  signupForm: any;

  constructor(private fb: FormBuilder, private auth: Auth, private router: Router, private toastr: ToastrService) {
    this.signupForm = this.fb.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: [
      '',
      [
        Validators.required,
        Validators.minLength(6),
        Validators.pattern(/^(?=.*[^A-Za-z0-9])/)
      ]
    ]
  });
  }
  loading = false;
  signupSuccess = false;

  onSubmit() {
    if (this.signupForm.invalid) {
      this.signupForm.markAllAsTouched(); // ✅ show validation errors
      return;
    }

    this.loading = true;
    this.auth.signup(this.signupForm.value).subscribe({
      next: () => {
      this.signupSuccess = true;
      this.loading = false;
      // Optionally navigate after delay
      setTimeout(() => this.router.navigate(['/login']), 2000);
      },
      error: () => {
        this.loading = false;
        this.toastr.error('Signup failed');
      }
    });
  }
}