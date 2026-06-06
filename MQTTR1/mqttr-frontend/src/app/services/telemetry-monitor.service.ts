import { Injectable, OnDestroy } from '@angular/core';
import { TelemetryService } from './telemetry.service';
import { DeviceService } from './device.service';
import { AlertService } from './alert.service';
import { interval, Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';

interface MetricHistory {
  [deviceId: string]: {
    temperature?: number;
    humidity?: number;
    pressure?: number;
  };
}

@Injectable({
  providedIn: 'root'
})
export class TelemetryMonitorService implements OnDestroy {
  private monitoringSubscription?: Subscription;
  private metricHistory: MetricHistory = {};
  private readonly POLL_INTERVAL = 10000; // Check every 10 seconds for faster notification updates

  constructor(
    private telemetryService: TelemetryService,
    private deviceService: DeviceService,
    private alertService: AlertService
  ) {}

  /**
   * Start monitoring telemetry data for critical changes
   */
  startMonitoring(): void {
    if (this.monitoringSubscription) {
      return; // Already monitoring
    }

    console.log('[TelemetryMonitor] Starting monitoring...');

    // Initial check
    this.checkTelemetry();

    // Poll every 60 seconds
    this.monitoringSubscription = interval(this.POLL_INTERVAL)
      .pipe(
        switchMap(() => this.deviceService.getAllDevices())
      )
      .subscribe({
        next: (devices) => {
          devices.forEach(device => {
            this.checkDeviceTelemetry(device.deviceId, device.name);
          });
        },
        error: (err) => {
          console.error('[TelemetryMonitor] Error:', err);
        }
      });
  }

  /**
   * Stop monitoring
   */
  stopMonitoring(): void {
    if (this.monitoringSubscription) {
      console.log('[TelemetryMonitor] Stopping monitoring...');
      this.monitoringSubscription.unsubscribe();
      this.monitoringSubscription = undefined;
    }
  }

  /**
   * Check telemetry for all devices
   */
  private checkTelemetry(): void {
    this.deviceService.getAllDevices().subscribe({
      next: (devices) => {
        devices.forEach(device => {
          this.checkDeviceTelemetry(device.deviceId, device.name);
        });
      },
      error: (err) => {
        console.error('[TelemetryMonitor] Error loading devices:', err);
      }
    });
  }

  /**
   * Check telemetry for a specific device
   */
  private checkDeviceTelemetry(deviceId: string, deviceName: string): void {
    // Use hourly data endpoint and compare the latest 2 points for each metric
    this.telemetryService.getHourlyData(deviceId).subscribe({
      next: (result: any) => {
        if (!result?.metrics || !Array.isArray(result.metrics)) {
          return;
        }

        result.metrics.forEach((metricGroup: any) => {
          if (!metricGroup?.metric || !Array.isArray(metricGroup.data)) {
            return;
          }

          const sortedData = [...metricGroup.data].sort(
            (a: any, b: any) => new Date(a.hour).getTime() - new Date(b.hour).getTime()
          );

          if (sortedData.length < 2) {
            if (sortedData.length === 1) {
              this.storeMetric(deviceId, metricGroup.metric, Number(sortedData[0].averageValue));
            }
            return;
          }

          const latest = sortedData[sortedData.length - 1];
          const previous = sortedData[sortedData.length - 2];

          this.checkMetricChange(
            deviceId,
            deviceName,
            metricGroup.metric,
            Number(latest.averageValue),
            Number(previous.averageValue)
          );

          this.storeMetric(deviceId, metricGroup.metric, Number(latest.averageValue));
        });
      },
      error: (err) => {
        console.error(`[TelemetryMonitor] Error loading telemetry for ${deviceId}:`, err);
      }
    });
  }

  /**
   * Check if metric change is critical
   */
  private checkMetricChange(
    deviceId: string,
    deviceName: string,
    metric: string,
    currentValue: number,
    previousValue: number
  ): void {
    const metricType = metric.toLowerCase() as 'temperature' | 'humidity' | 'pressure';

    // Validate metric type
    if (!['temperature', 'humidity', 'pressure'].includes(metricType)) {
      return;
    }

    // Check against thresholds using AlertService
    this.alertService.checkMetric(
      metricType,
      deviceId,
      deviceName,
      currentValue,
      previousValue
    );
  }

  /**
   * Store metric value in history
   */
  private storeMetric(deviceId: string, metric: string, value: number): void {
    if (!this.metricHistory[deviceId]) {
      this.metricHistory[deviceId] = {};
    }

    const metricType = metric.toLowerCase() as 'temperature' | 'humidity' | 'pressure';
    this.metricHistory[deviceId][metricType] = value;
  }

  ngOnDestroy(): void {
    this.stopMonitoring();
  }
}
