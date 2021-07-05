import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'checkMark'
})
export class CheckMarkPipe implements PipeTransform {

  transform(value: boolean, heavy?: boolean): string {
    return value ? heavy ? '\u2714' : '\u2713' : '';
  }
}
