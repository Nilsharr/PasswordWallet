import { Component, OnInit, inject } from '@angular/core';
import {
  MAT_DIALOG_DATA,
  MatDialogRef,
  MatDialogTitle,
  MatDialogContent,
  MatDialogActions,
  MatDialogClose,
  MatDialog,
} from '@angular/material/dialog';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TextFieldModule } from '@angular/cdk/text-field';
import { Credential } from '../../models/credential';
import { PasswordValidators } from '../../validators/password.validator';
import { PasswordStrengthComponent } from '../../components/password-strength/password-strength.component';
import { PasswordGeneratorDialogComponent } from '../password-generator-dialog/password-generator-dialog.component';
import { getPasswordInputType } from '../../utils/utils';

@Component({
  selector: 'app-add-edit-credential-dialog',
  standalone: true,
  imports: [
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatDialogClose,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCheckboxModule,
    MatIconModule,
    MatTooltipModule,
    TextFieldModule,
    PasswordStrengthComponent,
  ],
  templateUrl: './add-edit-credential-dialog.component.html',
})
export class AddEditCredentialDialogComponent implements OnInit {
  private readonly _formBuilder = inject(FormBuilder);
  private readonly _dialog = inject(MatDialog);
  private readonly _dialogRef = inject(
    MatDialogRef<AddEditCredentialDialogComponent>
  );

  public readonly data: Credential | undefined = inject(MAT_DIALOG_DATA);
  public addEditCredentialForm!: FormGroup;

  ngOnInit(): void {
    this.addEditCredentialForm = this._formBuilder.group(
      {
        username: [this.data?.username ?? '', [Validators.maxLength(60)]],
        password: [this.data?.password ?? ''],
        confirmPassword: [this.data?.password ?? ''],
        webAddress: [this.data?.webAddress ?? '', [Validators.maxLength(256)]],
        description: [
          this.data?.description ?? '',
          [Validators.maxLength(512)],
        ],
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

  public onSubmit(): void {
    if (this.addEditCredentialForm.invalid) {
      this.addEditCredentialForm.markAllAsTouched();
      return;
    }
    this._dialogRef.close({
      username: this.username?.value,
      password: this.password?.value,
      webAddress: this.webAddress?.value,
      description: this.description?.value,
    });
  }

  public showGeneratePasswordDialog(): void {
    const dialogRef = this._dialog.open(PasswordGeneratorDialogComponent, {
      hasBackdrop: false,
    });
    dialogRef.afterClosed().subscribe((result: { password: string } | null) => {
      if (result) {
        this.password?.setValue(result.password);
        this.confirmPassword?.setValue(result.password);
      }
    });
  }

  public getPasswordInputType = (): string =>
    getPasswordInputType(this.showPassword?.value);

  public get username() {
    return this.addEditCredentialForm.get('username');
  }
  public get password() {
    return this.addEditCredentialForm.get('password');
  }
  public get confirmPassword() {
    return this.addEditCredentialForm.get('confirmPassword');
  }
  public get webAddress() {
    return this.addEditCredentialForm.get('webAddress');
  }
  public get description() {
    return this.addEditCredentialForm.get('description');
  }
  public get showPassword() {
    return this.addEditCredentialForm.get('showPassword');
  }
}
