import { Component, OnInit, inject } from '@angular/core';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatLoadingButtonDirective } from '../../directives/mat-loading-button.directive';
import { UserService } from '../../services/user.service';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { getPasswordInputType } from '../../utils/utils';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCheckboxModule,
    MatIconModule,
    MatLoadingButtonDirective,
    RouterLink,
  ],
  templateUrl: './login.component.html',
})
export class LoginComponent implements OnInit {
  private readonly _formBuilder = inject(FormBuilder);
  private readonly _userService = inject(UserService);
  private readonly _router = inject(Router);

  public loginForm!: FormGroup;
  public isLoading: boolean = false;

  ngOnInit(): void {
    this.loginForm = this._formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      showPassword: [false],
    });
  }

  public onSubmit(): void {
    if (this.loginForm.invalid) {
      return;
    }
    this.isLoading = true;

    this._userService
      .login({
        username: this.username?.value,
        password: this.password?.value,
      })
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: () => this._router.navigate(['']),
        error: (error) => {
          if (error.status === 401) {
            this.password?.setValue('');
            this.password?.setErrors({ invalidLogin: true });
          }
          if (error.status === 403) {
            this.password?.setValue('');
            this.password?.setErrors({ tooManyIncorrectAttempts: true });
          }
        },
      });
  }

  public getPasswordInputType = (): string =>
    getPasswordInputType(this.showPassword?.value);

  public get username() {
    return this.loginForm.get('username');
  }

  public get password() {
    return this.loginForm.get('password');
  }

  public get showPassword() {
    return this.loginForm.get('showPassword');
  }
}
