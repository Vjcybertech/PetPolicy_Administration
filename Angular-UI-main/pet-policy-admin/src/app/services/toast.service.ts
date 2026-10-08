import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface ToastMessage {
  id: number;
  type: 'success' | 'error' | 'info' | 'warning';
  text: string;
}

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private toasts$ = new BehaviorSubject<ToastMessage[]>([]);
  public toasts = this.toasts$.asObservable();
  private count = 0;

  show(text: string, type: 'success' | 'error' | 'info' | 'warning' = 'info'): void {
    const current = this.toasts$.value;
    const id = ++this.count;
    const toast: ToastMessage = { id, type, text };
    this.toasts$.next([...current, toast]);

    setTimeout(() => {
      this.remove(id);
    }, 4000);
  }

  success(text: string): void { this.show(text, 'success'); }
  error(text: string): void { this.show(text, 'error'); }
  info(text: string): void { this.show(text, 'info'); }
  warning(text: string): void { this.show(text, 'warning'); }

  remove(id: number): void {
    const current = this.toasts$.value.filter(t => t.id !== id);
    this.toasts$.next(current);
  }
}
