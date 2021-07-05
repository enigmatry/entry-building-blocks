import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CheckMarkPipe } from './check-mark.pipe';

@NgModule({
  declarations: [CheckMarkPipe],
  imports: [
    CommonModule
  ],
  exports: [CheckMarkPipe]
})
export class EnigmatryPipesModule { }
