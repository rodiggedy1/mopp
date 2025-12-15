import { Component, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';

@Component({
  template: '',
  styles: []
})
export class BaseComponent implements OnDestroy {

  public _unsubscribeAll: Subject<void> = new Subject<void>();
  
  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }
}
