import {
  Component,
  DestroyRef,
  Input,
  OnInit,
  forwardRef,
  inject,
} from '@angular/core';
import {
  ControlValueAccessor,
  FormBuilder,
  FormGroup,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ReactiveFormsModule,
  ValidationErrors,
  Validator,
  Validators,
} from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TooltipListPipe } from '../../pipes/tooltip-list.pipe';
import { PasswordValidators } from '../../validators/password.validator';
import { MasterPasswordFormValues } from '../../models/master-password-form-values';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { getPasswordInputType } from '../../utils/utils';

@Component({
  selector: 'app-master-password-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    MatIconModule,
    MatTooltipModule,
    TooltipListPipe,
  ],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => MasterPasswordFormComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      useExisting: forwardRef(() => MasterPasswordFormComponent),
      multi: true,
    },
  ],
  templateUrl: './master-password-form.component.html',
})
export class MasterPasswordFormComponent
  implements OnInit, ControlValueAccessor, Validator
{
  private readonly _formBuilder = inject(FormBuilder);
  private readonly _destroyRef = inject(DestroyRef);

  @Input() public passwordLabel: string = 'Master password';
  @Input() public confirmPasswordLabel: string = 'Confirm master password';
  public masterPasswordForm!: FormGroup;

  ngOnInit(): void {
    this.masterPasswordForm = this._formBuilder.group(
      {
        password: [
          '',
          [
            Validators.required,
            Validators.minLength(10),
            PasswordValidators.patternValidator('[a-z]+', {
              requiresLowercase: true,
            }),
            PasswordValidators.patternValidator('[A-Z]+', {
              requiresUppercase: true,
            }),
            PasswordValidators.patternValidator('[0-9]+', {
              requiresDigit: true,
            }),
            PasswordValidators.patternValidator('[^a-zA-Z0-9]+', {
              requiresSpecialChars: true,
            }),
          ],
        ],
        confirmPassword: ['', Validators.required],
        showPassword: [false],
      },
      {
        validators: PasswordValidators.matchValidator(
          'password',
          'confirmPassword',
          { passwordsNotMatching: true }
        ),
      }
    );
  }

  public registerOnChange(fn: (value: MasterPasswordFormValues) => void): void {
    this.masterPasswordForm.valueChanges
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe(fn);
  }

  public registerOnTouched(fn: () => void): void {
    this.masterPasswordForm.valueChanges
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe(fn);
  }

  public writeValue(value: MasterPasswordFormValues): void {
    this.masterPasswordForm.patchValue(value, { emitEvent: false });
  }

  public validate(): ValidationErrors | null {
    return this.masterPasswordForm.valid ? null : { invalidPasswordForm: true };
  }

  public getPasswordInputType = (): string =>
    getPasswordInputType(this.showPassword?.value);

  public get password() {
    return this.masterPasswordForm.get('password');
  }
  public get confirmPassword() {
    return this.masterPasswordForm.get('confirmPassword');
  }
  public get showPassword() {
    return this.masterPasswordForm.get('showPassword');
  }
}
