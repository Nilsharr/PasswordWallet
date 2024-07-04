import {
  AfterViewInit,
  Component,
  OnInit,
  ViewChild,
  inject,
} from '@angular/core';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';
import { Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterLink } from '@angular/router';
import { MatLoadingButtonDirective } from '../../directives/mat-loading-button.directive';
import { UserService } from '../../services/user.service';
import {
  Observable,
  catchError,
  delay,
  finalize,
  map,
  of,
  switchMap,
} from 'rxjs';
import { MasterPasswordFormComponent } from '../master-password-form/master-password-form.component';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MasterPasswordFormComponent,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatLoadingButtonDirective,
    RouterLink,
  ],
  templateUrl: './register.component.html',
})
export class RegisterComponent implements OnInit, AfterViewInit {
  private readonly _formBuilder = inject(FormBuilder);
  private readonly _userService = inject(UserService);
  private readonly _router = inject(Router);
  @ViewChild(MasterPasswordFormComponent)
  private readonly _masterPasswordFormComponent!: MasterPasswordFormComponent;

  public registerForm!: FormGroup;
  public isLoading: boolean = false;

  ngOnInit(): void {
    this.registerForm = this._formBuilder.group({
      username: [
        '',
        [Validators.required, Validators.maxLength(30)],
        [this.validateUsernameIsAvailable.bind(this)],
      ],
    });
  }

  ngAfterViewInit(): void {
    this.registerForm.addControl('passwordForm', this.passwordForm);
  }

  public onSubmit(): void {
    if (this.registerForm.invalid || this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      return;
    }
    this.isLoading = true;

    this._userService
      .register({
        username: this.username?.value,
        password: this.passwordForm.value.password,
        confirmPassword: this.passwordForm.value.confirmPassword,
      })
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe(() => this._router.navigate(['']));
  }

  private validateUsernameIsAvailable(
    control: AbstractControl
  ): Observable<ValidationErrors | null> {
    return of(control.value).pipe(
      delay(750),
      switchMap((username) =>
        this._userService.checkUsernameAvailability(username).pipe(
          map((isAvailable: boolean) => {
            return isAvailable ? null : { usernameTaken: true };
          }),
          catchError(() => of(null))
        )
      )
    );
  }

  public get username() {
    return this.registerForm.get('username');
  }
  public get passwordForm() {
    return this._masterPasswordFormComponent.masterPasswordForm;
  }
}
