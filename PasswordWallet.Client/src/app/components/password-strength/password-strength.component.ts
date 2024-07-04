import { NgClass } from '@angular/common';
import { Component, DestroyRef, Input, OnInit, inject } from '@angular/core';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Observable, debounceTime, distinctUntilChanged } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { zxcvbn } from '@zxcvbn-ts/core';

@Component({
  selector: 'app-password-strength',
  standalone: true,
  imports: [NgClass, MatTooltipModule],
  templateUrl: './password-strength.component.html',
})
export class PasswordStrengthComponent implements OnInit {
  private readonly _destroyRef = inject(DestroyRef);

  @Input({ required: true }) public password$: Observable<string> | undefined;
  @Input() public initialValue: string = '';
  public passwordStrength: number = 0;

  ngOnInit(): void {
    if (this.initialValue !== '') {
      this.passwordStrength = this.calculateStrength(this.initialValue);
    }
    if (this.password$) {
      this.password$
        .pipe(
          debounceTime(250),
          distinctUntilChanged(),
          takeUntilDestroyed(this._destroyRef)
        )
        .subscribe((password: string) => {
          this.passwordStrength = this.calculateStrength(password);
        });
    }
  }

  public getPasswordStrengthTooltip(): string {
    switch (this.passwordStrength) {
      case 0:
        return 'Very weak password';
      case 1:
        return 'Weak password';
      case 2:
        return 'Moderate password';
      case 3:
        return 'Strong password';
      case 4:
        return 'Very strong password';
      default:
        return '';
    }
  }

  private calculateStrength(password: string): number {
    return zxcvbn(password).score;
  }
}
