import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'tooltipList',
  standalone: true,
})
export class TooltipListPipe implements PipeTransform {
  public transform(lines: string[]): string {
    let list: string = '';
    lines.forEach((line) => {
      list += '• ' + line + '\n';
    });

    return list;
  }
}
