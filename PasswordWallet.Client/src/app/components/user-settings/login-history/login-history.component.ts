import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { UserService } from '../../../services/user.service';
import { LoginHistory } from '../../../models/login-history';
import { PaginatedList } from '../../../models/paginated-list';
import { MatSortModule, Sort, SortDirection } from '@angular/material/sort';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { finalize } from 'rxjs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-login-history',
  standalone: true,
  imports: [
    MatFormFieldModule,
    MatSelectModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,
    CommonModule,
  ],
  templateUrl: './login-history.component.html',
})
export class LoginHistoryComponent implements OnInit {
  private readonly _userService = inject(UserService);
  private _corrrect?: boolean;

  public isLoading: boolean = false;
  public sortDir: SortDirection = 'desc';
  public currentPage: number = 0;
  public pageSize: number = 25;
  public totalCount: number = 0;
  public loginHistories: LoginHistory[] = [];
  public displayedColumns: string[] = [
    'position',
    'date',
    'correct',
    'ipAddress',
  ];

  ngOnInit(): void {
    this.loadLoginHistory(1);
  }

  public onLoginStatusChange(newValue: string): void {
    this._corrrect = this.loginStatusToBool(newValue);
    this.loadLoginHistory(1);
  }

  public onSortChange(sort: Sort): void {
    this.sortDir = sort.direction;
    this.loadLoginHistory(1);
  }

  public onPageChange(pageEvent: PageEvent): void {
    this.pageSize = pageEvent.pageSize;
    this.loadLoginHistory(pageEvent.pageIndex + 1);
  }

  private loadLoginHistory(pageNumber: number): void {
    const timeoutId = setTimeout(() => {
      this.isLoading = true;
    }, 50);

    this._userService
      .getLoginHistory(pageNumber, this.pageSize, this.sortDir, this._corrrect)
      .pipe(
        finalize(() => {
          clearTimeout(timeoutId);
          this.isLoading = false;
        })
      )
      .subscribe((loginHistories: PaginatedList<LoginHistory>) => {
        this.currentPage = loginHistories.pageNumber - 1;
        this.totalCount = loginHistories.totalCount;
        this.loginHistories = loginHistories.items;
      });
  }

  private loginStatusToBool(value: string): boolean | undefined {
    switch (value) {
      case 'correct':
        return true;
      case 'incorrect':
        return false;
      default:
        return undefined;
    }
  }
}
