import {
  Component,
  DestroyRef,
  OnInit,
  ViewChild,
  inject,
} from '@angular/core';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FolderStructureComponent } from './folder-structure/folder-structure.component';
import { CredentialPanelComponent } from './credential-panel/credential-panel.component';
import { MatDialog } from '@angular/material/dialog';
import { AddEditCredentialDialogComponent } from '../../dialogs/add-edit-credential-dialog/add-edit-credential-dialog.component';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Folder } from '../../models/folder';
import { Credential } from '../../models/credential';
import { FolderService } from '../../services/folder.service';
import { CredentialService } from '../../services/credential.service';
import { filter, switchMap } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-credentials',
  standalone: true,
  imports: [
    MatSidenavModule,
    FolderStructureComponent,
    CredentialPanelComponent,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    MatDividerModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './credentials.component.html',
})
export class CredentialsComponent implements OnInit {
  private readonly _folderService = inject(FolderService);
  private readonly _credentialService = inject(CredentialService);
  private readonly _dialog = inject(MatDialog);
  private readonly _destroyRef = inject(DestroyRef);
  @ViewChild(CredentialPanelComponent)
  private readonly _credentialPanelComponent!: CredentialPanelComponent;

  public isLoading: boolean = false;
  public firstLoading: boolean = true;
  public isShowingSidenav: boolean = true;
  public currentFolder: Folder | null = null;

  ngOnInit(): void {
    const timeoutId = setTimeout(() => {
      this.isLoading = true;
    }, 50);

    this._folderService.currentFolder$
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe((result: Folder | null) => {
        if (this.firstLoading) {
          clearTimeout(timeoutId);
          this.isLoading = false;
          this.firstLoading = false;
        }
        this.currentFolder = result;
      });
  }

  public toggleSidenav() {
    this.isShowingSidenav = !this.isShowingSidenav;
  }

  public addCredential(): void {
    const dialogRef = this._dialog.open(AddEditCredentialDialogComponent, {
      disableClose: true,
    });

    dialogRef
      .afterClosed()
      .pipe(
        filter((result: Credential | undefined) => !!result),
        switchMap((result: Credential | undefined) =>
          this._credentialService.addCredential(this.currentFolder!.id, result!)
        )
      )
      .subscribe(() => {
        if (this._credentialService.isDataFull()) {
          // need to update paginator length because its not updated yet by binding
          this._credentialPanelComponent.paginator.length++;
          this._credentialPanelComponent.paginator.lastPage();
        }
      });
  }
}
