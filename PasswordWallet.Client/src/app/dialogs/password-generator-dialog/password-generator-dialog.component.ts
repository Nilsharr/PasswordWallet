import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import {
  MatDialogRef,
  MatDialogTitle,
  MatDialogContent,
  MatDialogActions,
  MatDialogClose,
} from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CdkDrag, CdkDragHandle } from '@angular/cdk/drag-drop';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { PasswordGeneratorOptions } from '../../models/password-generator-options';
import { StorageConstants } from '../../constants/storage.constant';
import { CryptoService } from '../../services/crypto.service';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-password-generator-dialog',
  standalone: true,
  imports: [
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatDialogClose,
    CdkDrag,
    CdkDragHandle,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './password-generator-dialog.component.html',
})
export class PasswordGeneratorDialogComponent implements OnInit, OnDestroy {
  private static readonly _defaultPasswordLength: number = 16;
  private readonly _dialogRef = inject(
    MatDialogRef<PasswordGeneratorDialogComponent>
  );
  private readonly _formBuilder = inject(FormBuilder);
  private readonly _cryptoService = inject(CryptoService);
  private readonly _snackBar = inject(MatSnackBar);

  public passwordGenerationForm!: FormGroup;

  ngOnInit(): void {
    const savedGenerationOptions =
      this.getPasswordGeneratorOptionsFromStorage();
    this.passwordGenerationForm = this._formBuilder.group({
      passwordLength: [
        savedGenerationOptions?.length ??
          PasswordGeneratorDialogComponent._defaultPasswordLength,
        [Validators.min(4), Validators.max(1024)],
      ],
      includeLowercase: [savedGenerationOptions?.lowercase ?? true],
      includeUppercase: [savedGenerationOptions?.uppercase ?? true],
      includeDigits: [savedGenerationOptions?.digits ?? true],
      includeSpecialChars: [savedGenerationOptions?.special ?? true],
    });
  }

  ngOnDestroy(): void {
    this.setPasswordGeneratorOptionsInStorage();
  }

  public generatePassword(): void {
    if (this.passwordGenerationForm.valid) {
      if (
        !this.passwordGenerationOptions.lowercase &&
        !this.passwordGenerationOptions.uppercase &&
        !this.passwordGenerationOptions.digits &&
        !this.passwordGenerationOptions.special
      ) {
        this._snackBar.open(
          'There must be at least one checkbox checked to generate password',
          'X',
          { duration: 3000 }
        );
        return;
      }
      const password = this._cryptoService.randomPassword(
        this.passwordGenerationOptions
      );
      this._dialogRef.close({ password: password });
    }
  }

  private setPasswordGeneratorOptionsInStorage(): void {
    if (this.passwordLength?.value < 4 || this.passwordLength?.value > 1024) {
      this.passwordLength?.setValue(
        PasswordGeneratorDialogComponent._defaultPasswordLength
      );
    }
    localStorage.setItem(
      StorageConstants.PASSWORD_GENERATION_OPTIONS_KEY,
      JSON.stringify(this.passwordGenerationOptions)
    );
  }

  private getPasswordGeneratorOptionsFromStorage():
    | PasswordGeneratorOptions
    | undefined {
    const item = localStorage.getItem(
      StorageConstants.PASSWORD_GENERATION_OPTIONS_KEY
    );
    if (!item) {
      return undefined;
    }
    return JSON.parse(item);
  }

  public get passwordGenerationOptions(): PasswordGeneratorOptions {
    return {
      length: this.passwordLength?.value,
      lowercase: this.passwordGenerationForm.get('includeLowercase')?.value,
      uppercase: this.passwordGenerationForm.get('includeUppercase')?.value,
      digits: this.passwordGenerationForm.get('includeDigits')?.value,
      special: this.passwordGenerationForm.get('includeSpecialChars')?.value,
    };
  }

  public get passwordLength() {
    return this.passwordGenerationForm.get('passwordLength');
  }
}
