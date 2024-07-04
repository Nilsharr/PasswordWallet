export interface PaginatedList<T> {
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  items: T[];
}
