import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { CredentialResponse } from '../contracts/responses/credential-response';
import { Observable, map } from 'rxjs';
import { CredentialMapper } from '../mappers/credential.mapper';
import { PaginatedList } from '../models/paginated-list';
import { CredentialRequest } from '../contracts/requests/credential-request';
import { Credential } from '../models/credential';

@Injectable({ providedIn: 'root' })
export class CredentialDataService {
  private readonly _httpClient = inject(HttpClient);
  private readonly _baseUrl: string = `${environment.apiUrl}/credentials`;

  public getCredentials(
    folderId: string,
    pageNumber: number,
    pageSize: number
  ): Observable<PaginatedList<Credential>> {
    const options = {
      params: new HttpParams()
        .set('pageNumber', pageNumber)
        .set('pageSize', pageSize),
    };
    return this._httpClient
      .get<PaginatedList<CredentialResponse>>(
        `${this._baseUrl}/folders/${folderId}`,
        options
      )
      .pipe(map(CredentialMapper.toModel));
  }

  public getCredential(id: string): Observable<Credential> {
    return this._httpClient
      .get<CredentialResponse>(`${this._baseUrl}/${id}`)
      .pipe(map(CredentialMapper.dtoToModel));
  }

  public add(
    folderId: string,
    request: CredentialRequest
  ): Observable<Credential> {
    return this._httpClient
      .post<CredentialResponse>(`${this._baseUrl}/folders/${folderId}`, request)
      .pipe(map(CredentialMapper.dtoToModel));
  }

  public update(
    id: number,
    request: CredentialRequest
  ): Observable<Credential> {
    return this._httpClient
      .put<CredentialResponse>(`${this._baseUrl}/${id}`, request)
      .pipe(map(CredentialMapper.dtoToModel));
  }

  public decryptPassword(id: number): Observable<string> {
    return this._httpClient
      .get<{ password: string | null }>(`${this._baseUrl}/${id}/decrypt`)
      .pipe(map((response) => response.password ?? ''));
  }

  public moveToFolder(id: number, newFolderId: string): Observable<unknown> {
    return this._httpClient.patch(
      `${this._baseUrl}/${id}/folders/${newFolderId}`,
      {}
    );
  }

  public changePosition(id: number, position: number): Observable<unknown> {
    return this._httpClient.patch(`${this._baseUrl}/${id}/position`, {
      position,
    });
  }

  public delete(id: number): Observable<unknown> {
    return this._httpClient.delete(`${this._baseUrl}/${id}`);
  }
}
