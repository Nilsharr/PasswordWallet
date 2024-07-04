import { Directive, ElementRef, OnInit, inject } from '@angular/core';

@Directive({
  selector: 'input[appFocusAndSelect]',
  standalone: true,
})
export class FocusAndSelectDirective implements OnInit {
  private readonly _elementRef = inject(ElementRef);

  ngOnInit(): void {
    const inputElement = this._elementRef.nativeElement as HTMLInputElement;
    inputElement.focus();
    inputElement.select();
  }
}
