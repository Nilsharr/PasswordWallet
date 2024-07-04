/* eslint-disable @angular-eslint/directive-selector */
import {
  ComponentRef,
  Directive,
  Input,
  OnChanges,
  Renderer2,
  SimpleChanges,
  ViewContainerRef,
  inject,
} from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatProgressSpinner } from '@angular/material/progress-spinner';

@Directive({
  selector: `button[mat-button][loading],
  button[mat-raised-button][loading],
  button[mat-icon-button][loading],
  button[mat-fab][loading],
  button[mat-mini-fab][loading],
  button[mat-stroked-button][loading],
  button[mat-flat-button][loading]`,
  standalone: true,
})
export class MatLoadingButtonDirective implements OnChanges {
  private readonly matButton = inject(MatButton);
  private readonly viewContainerRef = inject(ViewContainerRef);
  private readonly renderer = inject(Renderer2);
  private spinner: ComponentRef<MatProgressSpinner> | null = null;

  @Input() public loading: boolean = false;
  @Input() public spinnerDiameter: number = 25;

  ngOnChanges(changes: SimpleChanges): void {
    if (!changes['loading']) {
      return;
    }

    if (changes['loading'].currentValue) {
      this.nativeElement.classList.add('mat-loading');
      this.matButton.disabled = true;
      this.createSpinner();
    } else if (!changes['loading'].firstChange) {
      this.nativeElement.classList.remove('mat-loading');
      this.matButton.disabled = false;
      this.destroySpinner();
    }
  }

  private createSpinner(): void {
    if (!this.spinner) {
      this.spinner = this.viewContainerRef.createComponent(MatProgressSpinner);
      this.spinner.instance.diameter = this.spinnerDiameter;
      this.spinner.instance.mode = 'indeterminate';
      this.renderer.appendChild(
        this.nativeElement,
        this.spinner.instance._elementRef.nativeElement
      );
    }
  }

  private destroySpinner(): void {
    if (this.spinner) {
      this.spinner.destroy();
      this.spinner = null;
    }
  }

  private get nativeElement(): HTMLElement {
    return this.matButton._elementRef.nativeElement;
  }
}
