import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-metric-filter',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="form-group">
      <label for="filterMetric">Metric</label>
      <select
        id="filterMetric"
        [(ngModel)]="selectedMetric"
        (ngModelChange)="onMetricChange()"
        [disabled]="availableMetrics.length === 0">
        <option value="">{{ availableMetrics.length === 0 ? 'Select Sensor First' : 'Select Metric' }}</option>
        <option *ngFor="let metric of availableMetrics" [value]="metric">
          {{ metric | titlecase }}
        </option>
      </select>
    </div>
  `,
  styles: [`
    .form-group {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }
    label {
      font-weight: 700;
      color: #2a3568;
      font-size: 13px;
      letter-spacing: 0.2px;
      text-transform: uppercase;
    }
    select {
      padding: 10px 12px;
      border: 1px solid #d5dcf6;
      border-radius: 10px;
      font-size: 14px;
      background-color: white;
      color: #1f2a58;
      cursor: pointer;
      transition: all 0.25s ease;
    }
    select:hover { border-color: #8ea2eb; }
    select:focus {
      outline: none;
      border-color: #2563eb;
      box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.16);
    }
    select:disabled {
      background-color: #f5f7ff;
      color: #8b94b3;
      cursor: not-allowed;
      border-color: #e1e6f7;
    }
  `]
})
export class MetricFilterComponent implements OnChanges {
  @Input() availableMetrics: string[] = [];
  @Input() selectedMetric: string = '';
  @Output() metricChange = new EventEmitter<string>();

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['availableMetrics'] && this.selectedMetric) {
      if (!this.availableMetrics.includes(this.selectedMetric)) {
        this.selectedMetric = '';
        this.metricChange.emit('');
      }
    }
  }

  onMetricChange(): void {
    this.metricChange.emit(this.selectedMetric);
  }
}
