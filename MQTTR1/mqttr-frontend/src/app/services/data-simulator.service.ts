import { Injectable } from '@angular/core';
import { Observable, interval } from 'rxjs';
import { map } from 'rxjs/operators';
import { TelemetryDto } from '../models/telemetry.model';

@Injectable({
  providedIn: 'root'
})
export class DataSimulatorService {
  private simulationRunning = false;

  constructor() {}

  /**
   * Generate random telemetry data for a device
   */
  generateTelemetryData(deviceId: string, metric: string, baseValue: number = 20, variance: number = 5): TelemetryDto {
    const randomValue = baseValue + (Math.random() - 0.5) * variance * 2;

    return {
      deviceId: deviceId,
      metric: metric,
      value: parseFloat(randomValue.toFixed(2)),
      timestamp: new Date().toISOString()
    };
  }

  /**
   * Generate historical telemetry data for charts
   */
  generateHistoricalData(
    deviceId: string, 
    metric: string, 
    points: number = 50, 
    baseValue: number = 20, 
    variance: number = 5
  ): TelemetryDto[] {
    const data: TelemetryDto[] = [];
    const now = new Date();

    for (let i = points - 1; i >= 0; i--) {
      const timestamp = new Date(now.getTime() - i * 60000); // 1 minute intervals
      const randomValue = baseValue + (Math.random() - 0.5) * variance * 2;

      data.push({
        deviceId: deviceId,
        metric: metric,
        value: parseFloat(randomValue.toFixed(2)),
        timestamp: timestamp.toISOString()
      });
    }

    return data;
  }

  /**
   * Simulate multiple devices with different metrics
   */
  generateMultiDeviceData(devices: Array<{deviceId: string, name: string}>): Map<string, any[]> {
    const dataMap = new Map<string, any[]>();

    devices.forEach(device => {
      const metrics = [
        { name: 'temperature', baseValue: 22, variance: 5 },
        { name: 'humidity', baseValue: 60, variance: 10 },
        { name: 'pressure', baseValue: 1013, variance: 5 }
      ];

      metrics.forEach(metric => {
        const key = `${device.deviceId}-${metric.name}`;
        dataMap.set(key, this.generateHistoricalData(
          device.deviceId,
          metric.name,
          50,
          metric.baseValue,
          metric.variance
        ));
      });
    });

    return dataMap;
  }

  /**
   * Create a real-time data stream (Observable)
   */
  createDataStream(
    deviceId: string,
    metric: string,
    intervalMs: number = 5000,
    baseValue: number = 20,
    variance: number = 5
  ): Observable<TelemetryDto> {
    return interval(intervalMs).pipe(
      map(() => this.generateTelemetryData(deviceId, metric, baseValue, variance))
    );
  }

  /**
   * Generate sine wave pattern for demo
   */
  generateSineWaveData(
    deviceId: string,
    metric: string,
    points: number = 100,
    amplitude: number = 10,
    offset: number = 20
  ): TelemetryDto[] {
    const data: TelemetryDto[] = [];
    const now = new Date();

    for (let i = 0; i < points; i++) {
      const timestamp = new Date(now.getTime() - (points - i) * 60000);
      const value = offset + amplitude * Math.sin((i / points) * Math.PI * 4);

      data.push({
        deviceId: deviceId,
        metric: metric,
        value: parseFloat(value.toFixed(2)),
        timestamp: timestamp.toISOString()
      });
    }

    return data;
  }

  /**
   * Calculate statistics from telemetry data
   */
  calculateStats(data: TelemetryDto[]): {
    min: number;
    max: number;
    avg: number;
    latest: number;
    count: number;
  } {
    if (data.length === 0) {
      return { min: 0, max: 0, avg: 0, latest: 0, count: 0 };
    }

    const values = data.map(d => d.value);
    const min = Math.min(...values);
    const max = Math.max(...values);
    const avg = values.reduce((a, b) => a + b, 0) / values.length;
    const latest = values[values.length - 1];

    return {
      min: parseFloat(min.toFixed(2)),
      max: parseFloat(max.toFixed(2)),
      avg: parseFloat(avg.toFixed(2)),
      latest: parseFloat(latest.toFixed(2)),
      count: data.length
    };
  }
}
