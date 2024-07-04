import { Injectable, inject } from '@angular/core';
import { Credential } from '../models/credential';
import { CredentialDataService } from './credential-data.service';
import { Observable, BehaviorSubject, tap, of, EMPTY } from 'rxjs';
import { moveItemInArray } from '@angular/cdk/drag-drop';
import { CredentialMapper } from '../mappers/credential.mapper';
import { PaginatedList } from '../models/paginated-list';

@Injectable({
  providedIn: 'root',
})
export class CredentialService {
  private readonly _credentialDataService = inject(CredentialDataService);
  private readonly _emptyCredentialPaginatedList: PaginatedList<Credential> = {
    pageNumber: 0,
    pageSize: 0,
    totalCount: 0,
    items: [],
  };
  private readonly _credentialsSubject: BehaviorSubject<
    PaginatedList<Credential>
  > = new BehaviorSubject<PaginatedList<Credential>>(
    this._emptyCredentialPaginatedList
  );

  public readonly credentials$: Observable<PaginatedList<Credential>> =
    this._credentialsSubject.asObservable();
  public pageSize: number = 10;

  public loadCredentials(
    folderId: string,
    pageNumber: number
  ): Observable<PaginatedList<Credential>> {
    return this._credentialDataService
      .getCredentials(folderId, pageNumber, this.pageSize)
      .pipe(
        tap((result: PaginatedList<Credential>) =>
          this._credentialsSubject.next(result)
        )
      );
  }

  public decryptCredential(id: number): Observable<string> {
    const credentialIndex: number = this.credentialsValue.items.findIndex(
      (x) => x.id === id
    );
    const password = this.credentialsValue.items[credentialIndex].password;
    if (password) {
      return of(password);
    }

    return this._credentialDataService.decryptPassword(id).pipe(
      tap((result: string) => {
        this.credentialsValue.items[credentialIndex].password = result;
        this._credentialsSubject.next(this.credentialsValue);
      })
    );
  }

  public addCredential(
    folderId: string,
    credential: Credential
  ): Observable<Credential> {
    return this._credentialDataService
      .add(folderId, CredentialMapper.toDto(credential))
      .pipe(
        tap((result: Credential) => {
          if (!this.isDataFull()) {
            this.credentialsValue.items.push(result);
          }
          this.credentialsValue.totalCount++;
          this._credentialsSubject.next(this.credentialsValue);
        })
      );
  }

  public editCredential(
    id: number,
    credential: Credential
  ): Observable<Credential> {
    return this._credentialDataService
      .update(id, CredentialMapper.toDto(credential))
      .pipe(
        tap((result: Credential) => {
          const credentialIndex: number = this.credentialsValue.items.findIndex(
            (x) => x.id === id
          );
          this.credentialsValue.items[credentialIndex] = result;
          this._credentialsSubject.next(this.credentialsValue);
        })
      );
  }

  public moveCredential(id: number, folderId: string): Observable<unknown> {
    return this._credentialDataService
      .moveToFolder(id, folderId)
      .pipe(tap(() => this.removeCredentialFromObservable(id)));
  }

  public changeCredentialPosition(
    previousIndex: number,
    currentIndex: number
  ): Observable<unknown> {
    if (previousIndex === currentIndex) {
      return EMPTY;
    }

    const id: number = this.credentialsValue.items[previousIndex].id;
    const newPosition: number =
      this.credentialsValue.items[currentIndex].position;

    return this._credentialDataService.changePosition(id, newPosition).pipe(
      tap(() => {
        if (currentIndex < previousIndex) {
          this.credentialsValue.items
            .filter((credential) => credential.position >= newPosition)
            .forEach((credential) => credential.position++);
        } else {
          this.credentialsValue.items
            .filter((credential) => credential.position <= newPosition)
            .forEach((credential) => credential.position--);
        }

        this.credentialsValue.items[previousIndex].position = newPosition;
        moveItemInArray(
          this.credentialsValue.items,
          previousIndex,
          currentIndex
        );
        this._credentialsSubject.next(this.credentialsValue);
      })
    );
  }

  public deleteCredential(id: number): Observable<unknown> {
    return this._credentialDataService
      .delete(id)
      .pipe(tap(() => this.removeCredentialFromObservable(id)));
  }

  public isDataEmpty(): boolean {
    return this.credentialsValue.items.length === 0;
  }

  public isDataFull(): boolean {
    return this.credentialsValue.items.length === this.pageSize;
  }

  private removeCredentialFromObservable(id: number): void {
    this.credentialsValue.items = this.credentialsValue.items.filter(
      (credential: Credential) => credential.id !== id
    );
    this.credentialsValue.totalCount--;
    this._credentialsSubject.next(this.credentialsValue);
  }

  private get credentialsValue(): PaginatedList<Credential> {
    return this._credentialsSubject.getValue();
  }
}
