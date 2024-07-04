import {
  AfterViewInit,
  Component,
  OnInit,
  ViewChild,
  inject,
} from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { UserService } from '../../../services/user.service';
import { MatLoadingButtonDirective } from '../../../directives/mat-loading-button.directive';
import { MasterPasswordFormComponent } from '../../master-password-form/master-password-form.component';
import { finalize } from 'rxjs';
import { getPasswordInputType } from '../../../utils/utils';
import { MatDividerModule } from '@angular/material/divider';

@Component({
  selector: 'app-security',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MasterPasswordFormComponent,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatLoadingButtonDirective,
  ],
  templateUrl: './security.component.html',
})
export class SecurityComponent implements OnInit, AfterViewInit {
  private readonly _formBuilder = inject(FormBuilder);
  private readonly _userService = inject(UserService);
  @ViewChild(MasterPasswordFormComponent)
  private readonly _masterPasswordFormComponent!: MasterPasswordFormComponent;

  public changePasswordForm!: FormGroup;
  public isLoading: boolean = false;

  ngOnInit(): void {
    this.changePasswordForm = this._formBuilder.group({
      currentPassword: ['', Validators.required],
    });
  }

  ngAfterViewInit(): void {
    this.changePasswordForm.addControl('passwordForm', this.passwordForm);
  }

  public onSubmit(): void {
    if (this.changePasswordForm.invalid || this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      return;
    }
    this.isLoading = true;

    this._userService
      .changePassword({
        currentPassword: this.currentPassword?.value,
        password: this.passwordForm.value.password,
        confirmPassword: this.passwordForm.value.confirmPassword,
      })
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: () => this._userService.logout(),
        error: (error) => {
          if (error.status === 401) {
            this.currentPassword?.setValue('');
            this.currentPassword?.setErrors({ invalidCurrentPassword: true });
          }
        },
      });
  }

  public getPasswordInputType = (): string =>
    getPasswordInputType(this.passwordForm?.value.showPassword);

  public get currentPassword() {
    return this.changePasswordForm.get('currentPassword');
  }
  public get passwordForm() {
    return this._masterPasswordFormComponent?.masterPasswordForm;
  }
}
