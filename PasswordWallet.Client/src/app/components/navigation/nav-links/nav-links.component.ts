import { Component, DestroyRef, Input, OnInit, inject } from '@angular/core';
import { UserService } from '../../../services/user.service';
import { CommonModule, NgClass } from '@angular/common';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { Observable } from 'rxjs';
import { ThemeService } from '../../../services/theme.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavItem } from '../../../models/nav-item';

@Component({
  selector: 'app-nav-links',
  standalone: true,
  imports: [
    NgClass,
    MatMenuModule,
    MatDividerModule,
    MatButtonModule,
    MatIconModule,
    MatSlideToggleModule,
    MatTooltipModule,
    RouterLink,
    RouterLinkActive,
    CommonModule,
  ],
  templateUrl: './nav-links.component.html',
})
export class NavLinksComponent implements OnInit {
  private readonly _destroyRef = inject(DestroyRef);
  private readonly _userService = inject(UserService);
  private readonly _themeService = inject(ThemeService);

  @Input() public isInsideSideNav: boolean = false;
  public isLoggedIn$: Observable<boolean> = this._userService.isLoggedIn$;
  public isDarkMode: boolean = false;

  ngOnInit(): void {
    this._themeService.darkTheme$
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe((isDarkMode: boolean) => (this.isDarkMode = isDarkMode));
  }

  public logout(): void {
    this._userService.logout();
  }

  public toggleTheme($event: MouseEvent): void {
    $event.stopPropagation();
    this._themeService.toggleTheme();
  }

  public get navItems(): NavItem[] {
    return [
      {
        name: 'Credentials',
        route: '/credentials',
        icon: 'lock',
      },
      {
        name: 'Settings',
        route: '/settings',
        icon: 'settings',
      },
    ];
  }
}
