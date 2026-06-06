import { Component, OnInit, HostListener, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AlertService, CriticalAlert } from '../../services/alert.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-navbar-alerts',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './navbar-alerts.component.html',
  styleUrls: ['./navbar-alerts.component.scss']
})
export class NavbarAlertsComponent implements OnInit {
  alerts$: Observable<CriticalAlert[]>;
  unreadCount$: Observable<number>;
  showDropdown = false;

  constructor(
    public alertService: AlertService,
    private elementRef: ElementRef
  ) {
    this.alerts$ = this.alertService.alerts$;
    this.unreadCount$ = this.alerts$.pipe(
      map(alerts => alerts.filter(a => !a.read).length)
    );
  }

  ngOnInit(): void {}

  toggleDropdown(): void {
    this.showDropdown = !this.showDropdown;
  }

  closeDropdown(): void {
    this.showDropdown = false;
  }

  markAsRead(alert: CriticalAlert, event: Event): void {
    event.stopPropagation();
    this.alertService.markAsRead(alert.id);
  }

  markAllAsRead(): void {
    this.alertService.markAllAsRead();
  }

  removeAlert(alert: CriticalAlert, event: Event): void {
    event.stopPropagation();
    this.alertService.removeAlert(alert.id);
  }

  clearAll(): void {
    this.alertService.clearAll();
    this.closeDropdown();
  }

  getTimeAgo(date: Date): string {
    const now = new Date();
    const seconds = Math.floor((now.getTime() - new Date(date).getTime()) / 1000);

    if (seconds < 60) return 'Just now';
    if (seconds < 3600) return `${Math.floor(seconds / 60)}m ago`;
    if (seconds < 86400) return `${Math.floor(seconds / 3600)}h ago`;
    return `${Math.floor(seconds / 86400)}d ago`;
  }

  // Close dropdown when clicking outside
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.closeDropdown();
    }
  }
}
