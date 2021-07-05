import { Component, DEFAULT_CURRENCY_CODE, Inject, Input, ViewEncapsulation } from '@angular/core';

import { ColumnDef } from './grid.interface';
import { DEFAULT_DATE_FORMAT } from './grid.module';

@Component({
  selector: 'enigmatry-grid-cell',
  templateUrl: './cell.component.html',
  styleUrls: ['./cell.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class EnigmatryGridCellComponent<T> {
  @Input() rowData: T;
  @Input() colDef: ColumnDef;

  constructor(
    @Inject(DEFAULT_DATE_FORMAT) public defaultDateFormat: string,
    @Inject(DEFAULT_CURRENCY_CODE) public defaultCurrencyCode: string) {
  }

  get value(): any {
    return this.getCellValue(this.rowData, this.colDef);
  }

  private getCellValue(rowData: T, colDef: ColumnDef) {
    const keys = colDef.field ? colDef.field.split('.') : [];
    return keys.reduce((data, key) => data && (data as any)[key], rowData);
  }

}
