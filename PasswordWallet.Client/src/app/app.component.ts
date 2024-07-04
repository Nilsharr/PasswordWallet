import {
  Component,
  DestroyRef,
  HostBinding,
  OnInit,
  inject,
} from '@angular/core';
import { NavBarComponent } from './components/navigation/nav-bar/nav-bar.component';
import { NavLinksComponent } from './components/navigation/nav-links/nav-links.component';
import { MatSidenavModule } from '@angular/material/sidenav';
import { RouterOutlet } from '@angular/router';
import { ThemeService } from './services/theme.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { OverlayContainer } from '@angular/cdk/overlay';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [NavBarComponent, NavLinksComponent, MatSidenavModule, RouterOutlet],
  templateUrl: './app.component.html',
})
export class AppComponent implements OnInit {
  private readonly DARK_THEME_CLASS_NAME = 'dark-theme';
  private readonly _destroyRef = inject(DestroyRef);
  private readonly _themeService = inject(ThemeService);
  private readonly _overlayContainer = inject(OverlayContainer);

  @HostBinding('class')
  public theme: string = '';

  ngOnInit(): void {
    this._themeService.darkTheme$
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe((isDarkTheme: boolean) => {
        this.theme = isDarkTheme ? this.DARK_THEME_CLASS_NAME : '';
        this.applyThemeToOverlayContainers(isDarkTheme);
      });
  }

  private applyThemeToOverlayContainers(isDarkTheme: boolean): void {
    const overlayContainerClasses =
      this._overlayContainer.getContainerElement().classList;
    if (isDarkTheme) {
      overlayContainerClasses.add(this.DARK_THEME_CLASS_NAME);
    } else {
      overlayContainerClasses.remove(this.DARK_THEME_CLASS_NAME);
    }
  }

  public get title(): string {
    return 'Password Wallet';
  }
}
