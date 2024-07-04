import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable, map } from 'rxjs';
import { Folder } from '../models/folder';
import { FolderResponse } from '../contracts/responses/folder-response';
import { FolderMapper } from '../mappers/folder.mapper';

@Injectable({ providedIn: 'root' })
export class FolderDataService {
  private readonly _httpClient = inject(HttpClient);
  private readonly _baseUrl: string = `${environment.apiUrl}/folders`;

  public getFolders(): Observable<Folder[]> {
    return this._httpClient
      .get<FolderResponse[]>(this._baseUrl)
      .pipe(map((data) => data.map(FolderMapper.toModel)));
  }

  public getFolder(id: string): Observable<Folder> {
    return this._httpClient
      .get<FolderResponse>(`${this._baseUrl}/${id}`)
      .pipe(map(FolderMapper.toModel));
  }

  public add(name: string): Observable<Folder> {
    return this._httpClient
      .post<FolderResponse>(this._baseUrl, { name })
      .pipe(map(FolderMapper.toModel));
  }

  public delete(id: string): Observable<unknown> {
    return this._httpClient.delete(`${this._baseUrl}/${id}`);
  }

  public rename(id: string, name: string): Observable<unknown> {
    return this._httpClient.patch(`${this._baseUrl}/${id}/name`, { name });
  }

  public changePosition(id: string, position: number): Observable<unknown> {
    return this._httpClient.patch(`${this._baseUrl}/${id}/position`, {
      position,
    });
  }
}
