import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export class PasswordValidators {
  public static patternValidator(
    regexString: string,
    error: ValidationErrors
  ): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null;
      }
      const valid = new RegExp(regexString).test(control.value);
      return valid ? null : error;
    };
  }

  public static matchValidator(
    controlName: string,
    matchingControlName: string,
    error: ValidationErrors
  ): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const passwordControl = control.get(controlName);
      const confirmPasswordControl = control.get(matchingControlName);

      if (!passwordControl || !confirmPasswordControl) {
        return null;
      }

      if (
        confirmPasswordControl.errors &&
        !confirmPasswordControl.hasError(Object.keys(error)[0])
      ) {
        return null;
      }

      if (passwordControl.value !== confirmPasswordControl.value) {
        confirmPasswordControl.setErrors(error);
        return error;
      }
      confirmPasswordControl.setErrors(null);
      return null;
    };
  }
}
