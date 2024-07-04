import { Component, Input, OnInit, inject } from '@angular/core';
import { Folder } from '../../../models/folder';
import { CdkDragDrop, CdkDropList, CdkDrag } from '@angular/cdk/drag-drop';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialog } from '@angular/material/dialog';
import { AddEditFolderDialogComponent } from '../../../dialogs/add-edit-folder-dialog/add-edit-folder-dialog.component';
import { ConfirmDialogComponent } from '../../../dialogs/confirm-dialog/confirm-dialog.component';
import { CommonModule, NgClass } from '@angular/common';
import { FolderService } from '../../../services/folder.service';
import { Observable, filter, finalize, switchMap } from 'rxjs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TruncatePipe } from '../../../pipes/truncate.pipe';

@Component({
  selector: 'app-folder-structure',
  standalone: true,
  imports: [
    CdkDropList,
    CdkDrag,
    MatListModule,
    MatMenuModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatTooltipModule,
    MatProgressSpinnerModule,
    NgClass,
    CommonModule,
    TruncatePipe,
  ],
  templateUrl: './folder-structure.component.html',
})
export class FolderStructureComponent implements OnInit {
  private readonly _dialog = inject(MatDialog);
  private readonly _folderService = inject(FolderService);

  @Input({ required: true }) public currentFolder: Folder | null = null;
  public isLoading: boolean = false;
  public readonly folders$: Observable<Folder[]> =
    this._folderService.allFolders$;

  ngOnInit(): void {
    const timeoutId = setTimeout(() => {
      this.isLoading = true;
    }, 10);

    this._folderService
      .loadFolders()
      .pipe(
        switchMap(() => this._folderService.openLastOpenedFolderOrFirst()),
        finalize(() => {
          clearTimeout(timeoutId);
          this.isLoading = false;
        })
      )
      .subscribe();
  }

  public openFolder(id: string): void {
    this._folderService.openFolder(id).subscribe();
  }

  public addFolder(): void {
    const dialogRef = this._dialog.open(AddEditFolderDialogComponent);
    dialogRef
      .afterClosed()
      .pipe(
        filter((result: { name: string } | null) => !!result),
        switchMap((result: { name: string } | null) =>
          this._folderService.addFolder(result!.name)
        ),
        switchMap((result: Folder) => this._folderService.openFolder(result.id))
      )
      .subscribe();
  }

  public renameFolder(selectedFolder: Folder): void {
    const dialogRef = this._dialog.open(AddEditFolderDialogComponent, {
      data: { name: selectedFolder.name },
    });
    dialogRef
      .afterClosed()
      .pipe(
        filter(
          (result: { name: string } | null) =>
            !!result && result.name !== selectedFolder.name
        ),
        switchMap((result: { name: string } | null) =>
          this._folderService.renameFolder(selectedFolder.id, result!.name)
        )
      )
      .subscribe();
  }

  public deleteFolder(selectedFolderId: string): void {
    const currentFolderId: string | undefined = this.currentFolder?.id;
    const dialogRef = this._dialog.open(ConfirmDialogComponent);
    dialogRef
      .afterClosed()
      .pipe(
        filter((result: boolean) => !!result),
        switchMap(() => this._folderService.deleteFolder(selectedFolderId)),
        filter(() => selectedFolderId === currentFolderId),
        switchMap(() => this._folderService.openFolderWithFirstPosition())
      )
      .subscribe();
  }

  public dropFolder(event: CdkDragDrop<Folder[]>): void {
    this._folderService
      .changeFolderPosition(event.previousIndex, event.currentIndex)
      .subscribe();
  }
}
