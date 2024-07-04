import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable, map, tap } from 'rxjs';
import { TokenDataService } from './token-data.service';
import { User } from '../models/user';
import { LoginRequest } from '../contracts/requests/login-request';
import { AuthenticationResponse } from '../contracts/responses/authentication-response';
import { RegisterRequest } from '../contracts/requests/register-request';
import { ChangePasswordRequest } from '../contracts/requests/change-password-request';
import { PaginatedList } from '../models/paginated-list';
import { LoginHistoryResponse } from '../contracts/responses/login-history-response';
import { LoginHistory } from '../models/login-history';
import { LoginHistoryMapper } from '../mappers/login-history.mapper';
import { AuthenticationConstants } from '../constants/authentication.constant';
import { StorageConstants } from '../constants/storage.constant';

@Injectable({ providedIn: 'root' })
export class UserDataService {
  private readonly _httpClient = inject(HttpClient);
  private readonly _tokenDataService = inject(TokenDataService);
  private readonly _baseUrl: string = `${environment.apiUrl}/users`;
  private readonly _publicApiContext = {
    context: new HttpContext().set(AuthenticationConstants.IS_PUBLIC_API, true),
  };
  private readonly _omitUnauthorizedErrorHandlingContext = {
    context: new HttpContext().set(
      AuthenticationConstants.OMIT_DEFAULT_UNAUTHORIZED_ERROR_HANDLING,
      true
    ),
  };

  public register(
    request: RegisterRequest
  ): Observable<AuthenticationResponse> {
    return this._httpClient
      .post<AuthenticationResponse>(
        `${this._baseUrl}/register`,
        request,
        this._publicApiContext
      )
      .pipe(tap(this.setLoginData));
  }

  public login(request: LoginRequest): Observable<AuthenticationResponse> {
    return this._httpClient
      .post<AuthenticationResponse>(
        `${this._baseUrl}/login`,
        request,
        this._publicApiContext
      )
      .pipe(tap(this.setLoginData));
  }

  public changePassword(request: ChangePasswordRequest): Observable<unknown> {
    return this._httpClient.patch(
      `${this._baseUrl}/change-password`,
      request,
      this._omitUnauthorizedErrorHandlingContext
    );
  }

  public checkUsernameAvailability(username: string): Observable<boolean> {
    return this._httpClient
      .get<{ isAvailable: boolean }>(
        `${this._baseUrl}/availability/${username}`,
        this._publicApiContext
      )
      .pipe(map((response) => response.isAvailable));
  }

  public getLoginHistory(
    pageNumber: number = 1,
    pageSize: number = 25,
    sortDir: string = 'desc',
    correct?: boolean
  ): Observable<PaginatedList<LoginHistory>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize)
      .set('sortDir', sortDir);
    if (correct !== undefined) {
      params = params.set('correct', correct);
    }

    return this._httpClient
      .get<PaginatedList<LoginHistoryResponse>>(
        `${this._baseUrl}/login-history`,
        { params: params }
      )
      .pipe(map(LoginHistoryMapper.toModel));
  }

  private setLoginData = (loginData: AuthenticationResponse): void => {
    const user: User = {
      username: loginData.username,
      lastValidLoginDate: loginData.lastSuccessfulLogin
        ? new Date(loginData.lastSuccessfulLogin)
        : null,
      lastInvalidLoginDate: loginData.lastUnsuccessfulLogin
        ? new Date(loginData.lastUnsuccessfulLogin)
        : null,
    };
    sessionStorage.setItem(StorageConstants.USER_KEY, JSON.stringify(user));

    this._tokenDataService.setTokens(
      {
        token: loginData.accessToken,
        expiry: new Date(loginData.accessTokenExpiry),
      },
      {
        token: loginData.refreshToken,
        expiry: new Date(loginData.refreshTokenExpiry),
      }
    );
  };
}
