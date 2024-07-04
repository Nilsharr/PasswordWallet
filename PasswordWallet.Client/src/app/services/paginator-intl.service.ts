import { Injectable } from '@angular/core';
import { MatPaginatorIntl } from '@angular/material/paginator';

@Injectable({
  providedIn: 'root',
})
export class PaginatorIntl extends MatPaginatorIntl {
  public override getRangeLabel = (
    page: number,
    pageSize: number,
    length: number
  ): string =>
    `Page ${length === 0 ? 0 : page + 1} of ${Math.ceil(length / pageSize)}`;
}
