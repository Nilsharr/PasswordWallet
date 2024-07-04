import { Injectable, inject } from '@angular/core';
import { FolderDataService } from './folder-data.service';
import { Folder } from '../models/folder';
import { BehaviorSubject, EMPTY, Observable, Subject, of, tap } from 'rxjs';
import { StorageConstants } from '../constants/storage.constant';
import { moveItemInArray } from '@angular/cdk/drag-drop';

@Injectable({
  providedIn: 'root',
})
export class FolderService {
  private readonly _folderDataService = inject(FolderDataService);
  private readonly _allFoldersSubject: BehaviorSubject<Folder[]> =
    new BehaviorSubject<Folder[]>([]);
  private readonly _currentFolderSubject: Subject<Folder | null> =
    new Subject<Folder | null>();

  public readonly allFolders$: Observable<Folder[]> =
    this._allFoldersSubject.asObservable();
  public readonly currentFolder$: Observable<Folder | null> =
    this._currentFolderSubject.asObservable();

  public loadFolders(): Observable<Folder[]> {
    return this._folderDataService
      .getFolders()
      .pipe(tap((result: Folder[]) => this._allFoldersSubject.next(result)));
  }

  public openFolder(id: string): Observable<Folder | null> {
    return this.setCurrentFolder(id);
  }

  public openLastOpenedFolderOrFirst(): Observable<Folder | null> {
    const lastOpenedFolderId: string | null = localStorage.getItem(
      StorageConstants.CURRENT_FOLDER_KEY
    );
    const lastOpenedFolder: Folder | undefined = this.allFoldersValue.find(
      (x) => x.id === lastOpenedFolderId
    );

    return lastOpenedFolder
      ? this.openFolder(lastOpenedFolder.id)
      : this.openFolderWithFirstPosition();
  }

  public openFolderWithFirstPosition(): Observable<Folder | null> {
    const folderWithLowestPosition: Folder | null =
      this.getFolderWithFirstPosition();

    return folderWithLowestPosition
      ? this.openFolder(folderWithLowestPosition.id)
      : this.clearCurrentFolder();
  }

  public addFolder(name: string): Observable<Folder> {
    return this._folderDataService.add(name).pipe(
      tap((result: Folder) => {
        this.allFoldersValue.push(result);
        this._allFoldersSubject.next(this.allFoldersValue);
      })
    );
  }

  public renameFolder(id: string, name: string): Observable<unknown> {
    return this._folderDataService.rename(id, name).pipe(
      tap(() => {
        const folderIndex: number = this.allFoldersValue.findIndex(
          (x) => x.id === id
        );
        this.allFoldersValue[folderIndex].name = name;
        this._allFoldersSubject.next(this.allFoldersValue);
      })
    );
  }

  public deleteFolder(id: string): Observable<unknown> {
    return this._folderDataService.delete(id).pipe(
      tap(() => {
        this._allFoldersSubject.next(
          this.allFoldersValue.filter((folder: Folder) => folder.id !== id)
        );
      })
    );
  }

  public changeFolderPosition(
    previousIndex: number,
    currentIndex: number
  ): Observable<unknown> {
    if (previousIndex === currentIndex) {
      return EMPTY;
    }

    const id: string = this.allFoldersValue[previousIndex].id;
    const newPosition: number = this.allFoldersValue[currentIndex].position;

    return this._folderDataService.changePosition(id, newPosition).pipe(
      tap(() => {
        if (currentIndex < previousIndex) {
          this.allFoldersValue
            .filter((folder) => folder.position >= newPosition)
            .forEach((folder) => folder.position++);
        } else {
          this.allFoldersValue
            .filter((folder) => folder.position <= newPosition)
            .forEach((folder) => folder.position--);
        }

        this.allFoldersValue[previousIndex].position = newPosition;
        moveItemInArray(this.allFoldersValue, previousIndex, currentIndex);
        this._allFoldersSubject.next(this.allFoldersValue);
      })
    );
  }

  private getFolderWithFirstPosition(): Folder | null {
    if (this.allFoldersValue.length === 0) {
      return null;
    }
    return this.allFoldersValue.reduce((prev, curr) =>
      prev.position < curr.position ? prev : curr
    );
  }

  private setCurrentFolder(id: string): Observable<Folder | null> {
    const folder: Folder | undefined = this.allFoldersValue.find(
      (x) => x.id === id
    );
    if (!folder) {
      return of(null);
    }
    localStorage.setItem(StorageConstants.CURRENT_FOLDER_KEY, folder.id);
    this._currentFolderSubject.next(folder);
    return of(folder);
  }

  private clearCurrentFolder(): Observable<null> {
    localStorage.removeItem(StorageConstants.CURRENT_FOLDER_KEY);
    this._currentFolderSubject.next(null);
    return of(null);
  }

  private get allFoldersValue(): Folder[] {
    return this._allFoldersSubject.getValue();
  }
}
