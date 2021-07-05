import { TemplateRef } from '@angular/core';

export interface ColumnDef {
  field: string;
  header?: string;
  hide?: boolean;
  pinned?: 'left' | 'right';
  width?: string;
  sortable?: boolean | string;
  sortProp?: ColumnSortProp;
  type?: ColumnType;
  typeParameter?: ColumnTypeParameter;
  cellTemplate?: TemplateRef<any> | null;
  class?: string;
}

export declare type ColumnType =
  | 'boolean'
  | 'number'
  | 'currency'
  | 'percent'
  | 'date'
  | 'link';

export interface ColumnTypeParameter {
  currencyCode?: string;
  display?: string | boolean;
  digitsInfo?: string;
  format?: string;
  locale?: string;
  timezone?: string;
}

export interface ColumnSortProp {
  id?: string;
  start?: 'asc' | 'desc';
  disableClear?: boolean;
}

export interface CellTemplate {
  [key: string]: TemplateRef<any>;
}

export interface RowSelectionFormatter {
  disabled?: (rowData: any) => boolean;
  hideCheckbox?: (rowData: any) => boolean;
}

export interface RowClassFormatter {
  [className: string]: (rowData: any) => boolean;
}

export interface ContextMenuItem {
  id: string;
  name: string;
  icon?: string;
  disabled?: boolean;
}

export interface RowContextMenuFormatter {
  items: (rowData: any) => ContextMenuItem[];
}
