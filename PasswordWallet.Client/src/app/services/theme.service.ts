import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { StorageConstants } from '../constants/storage.constant';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private readonly LIGHT_THEME = 'light';
  private readonly DARK_THEME = 'dark';

  private readonly _darkThemeSubject: BehaviorSubject<boolean> =
    new BehaviorSubject<boolean>(this.getCurrentTheme());

  public readonly darkTheme$: Observable<boolean> =
    this._darkThemeSubject.asObservable();

  public toggleTheme(): void {
    const isDarkTheme = !this._darkThemeSubject.getValue();
    this.saveTheme(isDarkTheme);
    this._darkThemeSubject.next(isDarkTheme);
  }

  private getCurrentTheme(): boolean {
    const savedTheme: string | null = localStorage.getItem(
      StorageConstants.THEME_KEY
    );
    if (savedTheme) {
      return savedTheme === this.DARK_THEME;
    }
    const prefersDarkMode =
      window.matchMedia &&
      window.matchMedia('(prefers-color-scheme: dark)').matches;
    this.saveTheme(prefersDarkMode);
    return prefersDarkMode;
  }

  private saveTheme(isDarkTheme: boolean): void {
    localStorage.setItem(
      StorageConstants.THEME_KEY,
      isDarkTheme ? this.DARK_THEME : this.LIGHT_THEME
    );
  }
}
