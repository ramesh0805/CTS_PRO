import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface Notification {
  id: number;
  type: 'success' | 'error' | 'info' | 'warning';
  message: string;
  duration?: number;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private notificationsSubject = new BehaviorSubject<Notification[]>([]);
  public notifications$: Observable<Notification[]> = this.notificationsSubject.asObservable();
  private nextId = 1;

  constructor() {}

  success(message: string, duration: number = 5000): void {
    this.show('success', message, duration);
  }

  error(message: string, duration: number = 5000): void {
    this.show('error', message, duration);
  }

  info(message: string, duration: number = 5000): void {
    this.show('info', message, duration);
  }

  warning(message: string, duration: number = 5000): void {
    this.show('warning', message, duration);
  }

  private show(type: 'success' | 'error' | 'info' | 'warning', message: string, duration: number): void {
    const notification: Notification = {
      id: this.nextId++,
      type,
      message,
      duration
    };

    const currentNotifications = this.notificationsSubject.value;
    this.notificationsSubject.next([...currentNotifications, notification]);

    // Auto remove after duration
    if (duration > 0) {
      setTimeout(() => {
        this.remove(notification.id);
      }, duration);
    }
  }

  remove(id: number): void {
    const currentNotifications = this.notificationsSubject.value;
    this.notificationsSubject.next(currentNotifications.filter(n => n.id !== id));
  }

  clear(): void {
    this.notificationsSubject.next([]);
  }
}
