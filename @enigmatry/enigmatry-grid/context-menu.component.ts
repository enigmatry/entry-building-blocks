import { Component, EventEmitter, Input, OnInit, Output, ViewEncapsulation } from '@angular/core';
import { ContextMenuItem } from './grid.interface';

@Component({
  selector: 'enigmatry-grid-context-menu',
  templateUrl: './context-menu.component.html',
  styleUrls: ['./context-menu.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class EnigmatryGridContextMenuComponent implements OnInit {

  @Input() items: ContextMenuItem[] = [];
  @Output() selected = new EventEmitter<string>();

  constructor() { }

  ngOnInit(): void { }
}
