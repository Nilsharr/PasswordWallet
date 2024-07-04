import { Injectable, inject } from '@angular/core';
import { environment } from '../../environments/environment';
import { AuthenticationConstants } from '../constants/authentication.constant';
import { TokenInfo } from '../models/token-info';
import { TokenRequest } from '../contracts/requests/token-request';
import { TokenResponse } from '../contracts/responses/token-response';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable, map, tap } from 'rxjs';
import { TokenMapper } from '../mappers/token.mapper';
import { StorageConstants } from '../constants/storage.constant';
import { parseToDate } from '../utils/utils';

@Injectable({
  providedIn: 'root',
})
export class TokenDataService {
  private readonly _httpClient = inject(HttpClient);
  private readonly _baseUrl: string = `${environment.apiUrl}/users`;
  private readonly _publicApiContext = {
    context: new HttpContext().set(AuthenticationConstants.IS_PUBLIC_API, true),
  };

  public refreshToken(request: TokenRequest): Observable<TokenInfo> {
    return this._httpClient
      .post<TokenResponse>(
        `${this._baseUrl}/refresh`,
        request,
        this._publicApiContext
      )
      .pipe(map(TokenMapper.toModel), tap(this.setJwtToken));
  }

  public revokeToken(): Observable<unknown> {
    return this._httpClient.delete(`${this._baseUrl}/revoke`);
  }

  public setTokens(accessToken: TokenInfo, refreshToken: TokenInfo) {
    this.setJwtToken(accessToken);
    this.setRefreshToken(refreshToken);
  }

  public setJwtToken(tokenInfo: TokenInfo): void {
    sessionStorage.setItem(
      StorageConstants.JWT_TOKEN_KEY,
      JSON.stringify(tokenInfo)
    );
  }

  public setRefreshToken(tokenInfo: TokenInfo): void {
    sessionStorage.setItem(
      StorageConstants.REFRESH_TOKEN_KEY,
      JSON.stringify(tokenInfo)
    );
  }

  public clearTokens(): void {
    sessionStorage.removeItem(StorageConstants.JWT_TOKEN_KEY);
    sessionStorage.removeItem(StorageConstants.REFRESH_TOKEN_KEY);
  }

  public getJwtToken(): TokenInfo | null {
    return this.getToken(StorageConstants.JWT_TOKEN_KEY);
  }

  public getRefreshToken(): TokenInfo | null {
    return this.getToken(StorageConstants.REFRESH_TOKEN_KEY);
  }

  private getToken(key: string): TokenInfo | null {
    const item: string | null = sessionStorage.getItem(key);
    if (item) {
      try {
        const tokenInfo: { token: string; expiry: string } = JSON.parse(item);
        if (tokenInfo.token && tokenInfo.expiry) {
          const expiry = parseToDate(tokenInfo.expiry);
          return expiry ? { token: tokenInfo.token, expiry: expiry } : null;
        }
      } catch {
        return null;
      }
    }
    return null;
  }
}
