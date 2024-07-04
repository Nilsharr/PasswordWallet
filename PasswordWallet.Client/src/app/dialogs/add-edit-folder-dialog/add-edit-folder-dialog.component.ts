import { Component, OnInit, inject } from '@angular/core';
import {
  MatDialogRef,
  MatDialogTitle,
  MatDialogContent,
  MatDialogActions,
  MatDialogClose,
  MAT_DIALOG_DATA,
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
import { MatIconModule } from '@angular/material/icon';
import { FocusAndSelectDirective } from '../../directives/focus-and-select.directive';

@Component({
  selector: 'app-add-edit-folder-dialog',
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
    MatIconModule,
    FocusAndSelectDirective,
  ],
  templateUrl: './add-edit-folder-dialog.component.html',
})
export class AddEditFolderDialogComponent implements OnInit {
  private readonly _dialogRef = inject(
    MatDialogRef<AddEditFolderDialogComponent>
  );
  private readonly _formBuilder = inject(FormBuilder);

  public readonly data: { name: string } | undefined = inject(MAT_DIALOG_DATA);
  public addEditFolderForm!: FormGroup;

  ngOnInit(): void {
    this.addEditFolderForm = this._formBuilder.group({
      name: [
        this.data?.name ?? '',
        [Validators.required, Validators.maxLength(64)],
      ],
    });
  }

  public onSubmit(): void {
    if (this.addEditFolderForm.invalid) {
      this.addEditFolderForm.markAllAsTouched();
      return;
    }
    this._dialogRef.close({ name: this.name?.value ?? '' });
  }

  public get name() {
    return this.addEditFolderForm.get('name');
  }
}
