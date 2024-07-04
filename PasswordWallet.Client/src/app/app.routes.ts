import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { loggedInGuard } from './guards/logged-in.guard';
import { RegisterComponent } from './components/register/register.component';
import { LoginComponent } from './components/login/login.component';
import { CredentialsComponent } from './components/credentials/credentials.component';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { UserSettingsComponent } from './components/user-settings/user-settings.component';
import { ProfileComponent } from './components/user-settings/profile/profile.component';
import { SecurityComponent } from './components/user-settings/security/security.component';
import { LoginHistoryComponent } from './components/user-settings/login-history/login-history.component';

export const routes: Routes = [
  { path: '', redirectTo: 'credentials', pathMatch: 'full' },
  {
    path: 'register',
    component: RegisterComponent,
    title: 'Register',
    canActivate: [loggedInGuard],
  },
  {
    path: 'login',
    component: LoginComponent,
    title: 'Login',
    canActivate: [loggedInGuard],
  },
  {
    path: 'credentials',
    component: CredentialsComponent,
    title: 'Credentials',
    canActivate: [authGuard],
  },
  {
    path: 'settings',
    component: UserSettingsComponent,
    title: 'Account settings',
    canActivate: [authGuard],
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'profile',
      },
      {
        path: 'profile',
        component: ProfileComponent,
        title: 'Profile',
      },
      {
        path: 'security',
        component: SecurityComponent,
        title: 'Security',
      },
      {
        path: 'history',
        component: LoginHistoryComponent,
        title: 'Login history',
      },
    ],
  },
  { path: '**', component: PageNotFoundComponent, title: 'Page not found' },
];
