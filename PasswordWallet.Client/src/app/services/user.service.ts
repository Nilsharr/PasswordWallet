import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { TokenDataService } from './token-data.service';
import { User } from '../models/user';
import { StorageConstants } from '../constants/storage.constant';
import { parseToDate } from '../utils/utils';
import { Router } from '@angular/router';
import { AuthenticationResponse } from '../contracts/responses/authentication-response';
import { UserDataService } from './user-data.service';
import { RegisterRequest } from '../contracts/requests/register-request';
import { LoginRequest } from '../contracts/requests/login-request';
import { ChangePasswordRequest } from '../contracts/requests/change-password-request';
import { PaginatedList } from '../models/paginated-list';
import { LoginHistory } from '../models/login-history';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly _userDataService = inject(UserDataService);
  private readonly _tokenDataService = inject(TokenDataService);
  private readonly _router = inject(Router);
  private readonly _isLoggedInSubject: BehaviorSubject<boolean> =
    new BehaviorSubject<boolean>(!!this._tokenDataService.getJwtToken());

  public readonly isLoggedIn$: Observable<boolean> =
    this._isLoggedInSubject.asObservable();

  public register(
    request: RegisterRequest
  ): Observable<AuthenticationResponse> {
    return this._userDataService
      .register(request)
      .pipe(tap(() => this._isLoggedInSubject.next(true)));
  }

  public login(request: LoginRequest): Observable<AuthenticationResponse> {
    return this._userDataService
      .login(request)
      .pipe(tap(() => this._isLoggedInSubject.next(true)));
  }

  public logout(): void {
    if (this.isLoggedInValue) {
      this._tokenDataService.revokeToken().subscribe();
    }
    this._isLoggedInSubject.next(false);
    this._tokenDataService.clearTokens();
    sessionStorage.removeItem(StorageConstants.USER_KEY);
    this._router.navigate(['/login']);
  }

  public changePassword(request: ChangePasswordRequest): Observable<unknown> {
    return this._userDataService.changePassword(request);
  }

  public checkUsernameAvailability(username: string): Observable<boolean> {
    return this._userDataService.checkUsernameAvailability(username);
  }

  public getLoginHistory(
    pageNumber: number = 1,
    pageSize: number = 25,
    sortDir: string = 'desc',
    correct?: boolean
  ): Observable<PaginatedList<LoginHistory>> {
    return this._userDataService.getLoginHistory(
      pageNumber,
      pageSize,
      sortDir,
      correct
    );
  }

  public get isLoggedInValue(): boolean {
    return this._isLoggedInSubject.getValue();
  }

  public get user(): User | null {
    const item: string | null = sessionStorage.getItem(
      StorageConstants.USER_KEY
    );
    if (item) {
      try {
        const user: {
          username: string;
          lastValidLoginDate?: string;
          lastInvalidLoginDate?: string;
        } = JSON.parse(item);

        return {
          username: user.username,
          lastValidLoginDate: parseToDate(user.lastValidLoginDate),
          lastInvalidLoginDate: parseToDate(user.lastInvalidLoginDate),
        };
      } catch {
        return null;
      }
    }
    return null;
  }
}
