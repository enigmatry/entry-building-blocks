
export { Sort as SortEvent, SortDirection } from '@angular/material/sort';
export { PageEvent } from '@angular/material/paginator';

export interface PagedData<T> {
    items?: T[];
    pageSize?: number;
    pageNumber?: number;
    totalCount?: number;
    totalPages?: number;
    hasPreviousPage?: boolean;
    hasNextPage?: boolean;
}
