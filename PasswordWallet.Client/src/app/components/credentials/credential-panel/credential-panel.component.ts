import {
  AfterViewInit,
  Component,
  Input,
  OnChanges,
  SimpleChanges,
  ViewChild,
  inject,
} from '@angular/core';
import { Credential } from '../../../models/credential';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { CdkDrag, CdkDropList, CdkDragDrop } from '@angular/cdk/drag-drop';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import {
  MatPaginator,
  MatPaginatorModule,
  PageEvent,
} from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { Clipboard } from '@angular/cdk/clipboard';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ConfirmDialogComponent } from '../../../dialogs/confirm-dialog/confirm-dialog.component';
import { AddEditCredentialDialogComponent } from '../../../dialogs/add-edit-credential-dialog/add-edit-credential-dialog.component';
import { Folder } from '../../../models/folder';
import { Observable, filter, finalize, map, switchMap } from 'rxjs';
import { FolderService } from '../../../services/folder.service';
import { CredentialService } from '../../../services/credential.service';
import { CommonModule } from '@angular/common';
import { PaginatedList } from '../../../models/paginated-list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-credential-panel',
  standalone: true,
  imports: [
    MatTableModule,
    CdkDrag,
    CdkDropList,
    MatMenuModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    MatSortModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,
    CommonModule,
  ],
  templateUrl: './credential-panel.component.html',
})
export class CredentialPanelComponent implements OnChanges, AfterViewInit {
  private readonly _folderService = inject(FolderService);
  private readonly _credentialService = inject(CredentialService);
  private readonly _dialog = inject(MatDialog);
  private readonly _clipboard = inject(Clipboard);
  private readonly _snackBar = inject(MatSnackBar);
  private _dataSource = new MatTableDataSource<Credential>();
  @ViewChild(MatSort) private readonly _sort!: MatSort;

  @Input({ required: true }) public currentFolder!: Folder;
  @ViewChild(MatPaginator) public readonly paginator!: MatPaginator;
  public isLoading: boolean = false;
  public currentPage: number = 0;
  public pageSize: number = this._credentialService.pageSize;
  public totalCount: number = 0;
  public isSorting: boolean = false;
  public dragDisabled: boolean = true;
  public readonly displayedColumns: string[] = [
    'dragColumn',
    'username',
    'password',
    'webAddress',
    'description',
    'iconColumn',
  ];

  public readonly folders$: Observable<Folder[]> =
    this._folderService.allFolders$;
  public readonly dataSource$: Observable<MatTableDataSource<Credential>> =
    this._credentialService.credentials$.pipe(
      map((credentials: PaginatedList<Credential>) => {
        this.currentPage = credentials.pageNumber - 1;
        this.totalCount = credentials.totalCount;
        this._dataSource.data = credentials.items;
        return this._dataSource;
      })
    );

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['currentFolder']) {
      this.loadCredentials(1);
    }
  }

  ngAfterViewInit(): void {
    this._dataSource.sort = this._sort;
  }

  public dropCredential(event: CdkDragDrop<Credential[]>): void {
    if (event.previousIndex !== event.currentIndex) {
      this._credentialService
        .changeCredentialPosition(event.previousIndex, event.currentIndex)
        .subscribe();
    }
  }

  public copyToClipboard(text: string): void {
    if (text) {
      this._clipboard.copy(text);
      this._snackBar.open('Text copied to clipboard', 'X', { duration: 2000 });
    }
  }

  public copyPasswordToClickboard(credential: Credential): void {
    this._credentialService.decryptCredential(credential.id).subscribe(() => {
      if (credential.password) {
        this.copyToClipboard(credential.password);
      } else {
        this._snackBar.open('No password to copy', 'X', { duration: 2000 });
      }
    });
  }

  public openSite(siteUrl: string): void {
    if (!siteUrl.startsWith('http://') && !siteUrl.startsWith('https://')) {
      siteUrl = 'https://' + siteUrl;
    }
    window.open(siteUrl);
  }

  public editCredential(credential: Credential): void {
    this._credentialService
      .decryptCredential(credential.id)
      .pipe(
        switchMap(() => {
          const dialogRef = this._dialog.open(
            AddEditCredentialDialogComponent,
            {
              data: credential,
              disableClose: true,
            }
          );
          return dialogRef.afterClosed();
        }),
        filter((result: Credential | null) => !!result),
        switchMap((result: Credential | null) =>
          this._credentialService.editCredential(credential.id, result!)
        )
      )
      .subscribe();
  }

  public deleteCredential(id: number): void {
    const dialogRef = this._dialog.open(ConfirmDialogComponent);
    dialogRef
      .afterClosed()
      .pipe(
        filter((result: boolean) => !!result),
        switchMap(() => this._credentialService.deleteCredential(id))
      )
      .subscribe(() => {
        if (this._credentialService.isDataEmpty()) {
          this.paginator.previousPage();
        }
      });
  }

  public moveCredential(credentialId: number, folderId: string): void {
    this._credentialService
      .moveCredential(credentialId, folderId)
      .subscribe(() => {
        if (this._credentialService.isDataEmpty()) {
          this.paginator.previousPage();
        }
      });
  }

  public onPageChange(pageEvent: PageEvent): void {
    this._credentialService.pageSize = pageEvent.pageSize;
    this.pageSize = pageEvent.pageSize;
    this.loadCredentials(pageEvent.pageIndex + 1);
  }

  private loadCredentials(pageNumber: number): void {
    const timeoutId = setTimeout(() => {
      this.isLoading = true;
    }, 50);

    this._credentialService
      .loadCredentials(this.currentFolder.id, pageNumber)
      .pipe(
        finalize(() => {
          clearTimeout(timeoutId);
          this.isLoading = false;
        })
      )
      .subscribe();
  }
}
