import { Component } from '@angular/core';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterModule } from '@angular/router';
import { NavItem } from '../../models/nav-item';

@Component({
  selector: 'app-user-settings',
  standalone: true,
  imports: [MatListModule, MatIconModule, MatTooltipModule, RouterModule],
  templateUrl: './user-settings.component.html',
})
export class UserSettingsComponent {
  public get navItems(): NavItem[] {
    return [
      { name: 'Profile', route: 'profile', icon: 'person' },
      {
        name: 'Security',
        route: 'security',
        icon: 'security',
      },
      {
        name: 'Login history',
        route: 'history',
        icon: 'timeline',
      },
    ];
  }
}
