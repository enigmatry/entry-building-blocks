import { InjectionToken, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';

import { EnigmatryGridComponent } from './grid.component';
import { EnigmatryGridCellComponent } from './cell.component';
import { EnigmatryGridContextMenuComponent } from './context-menu.component';
import { EnigmatryPipesModule } from '../pipes';

export const DEFAULT_DATE_FORMAT: InjectionToken<string> = new InjectionToken<string>('');

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatCheckboxModule,
    MatIconModule,
    MatMenuModule,
    EnigmatryPipesModule
  ],
  declarations: [
    EnigmatryGridComponent,
    EnigmatryGridCellComponent,
    EnigmatryGridContextMenuComponent
  ],
  exports: [
    EnigmatryGridComponent
  ],
  providers: [{ provide: DEFAULT_DATE_FORMAT, useValue: 'mediumDate' }]
})
export class EnigmatryGridModule { }
