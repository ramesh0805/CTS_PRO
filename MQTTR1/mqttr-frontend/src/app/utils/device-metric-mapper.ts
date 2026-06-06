/**
 * Utility for mapping devices to their default metrics based on device ID patterns
 */

export interface MetricOption {
  name: string;
  unit: string;
  color: string;
}

export class DeviceMetricMapper {

  // All available metrics in the system
  static readonly ALL_METRICS: MetricOption[] = [
    { name: 'temperature', unit: '°C', color: '#ff6384' },
    { name: 'humidity', unit: '%', color: '#36a2eb' },
    { name: 'pressure', unit: 'hPa', color: '#ffce56' },
    { name: 'voltage', unit: 'V', color: '#4bc0c0' },
    { name: 'current', unit: 'A', color: '#9966ff' },
    { name: 'power', unit: 'W', color: '#ff9f40' },
    { name: 'light', unit: 'lux', color: '#ffcd56' },
    { name: 'motion', unit: '', color: '#c9cbcf' },
    { name: 'co2', unit: 'ppm', color: '#ff6384' },
    { name: 'vibration', unit: 'g', color: '#36a2eb' }
  ];

  /**
   * Get default metric for a device based on its ID or name
   * @param deviceId The device identifier
   * @param deviceName The device name
   * @returns The default metric name
   */
  static getDefaultMetric(deviceId: string, deviceName?: string): string {
    const searchText = (deviceId + ' ' + (deviceName || '')).toLowerCase();

    // Pattern matching for common device types
    if (this.matchesPattern(searchText, ['temp', 'temperature', 'thermal'])) {
      return 'temperature';
    }
    if (this.matchesPattern(searchText, ['humid', 'moisture'])) {
      return 'humidity';
    }
    if (this.matchesPattern(searchText, ['press', 'pressure', 'barometer'])) {
      return 'pressure';
    }
    if (this.matchesPattern(searchText, ['volt', 'battery'])) {
      return 'voltage';
    }
    if (this.matchesPattern(searchText, ['current', 'amp', 'ampere'])) {
      return 'current';
    }
    if (this.matchesPattern(searchText, ['power', 'watt'])) {
      return 'power';
    }
    if (this.matchesPattern(searchText, ['light', 'lux', 'illuminance'])) {
      return 'light';
    }
    if (this.matchesPattern(searchText, ['motion', 'movement', 'pir'])) {
      return 'motion';
    }
    if (this.matchesPattern(searchText, ['co2', 'carbon'])) {
      return 'co2';
    }
    if (this.matchesPattern(searchText, ['vibrat', 'shake', 'accelero'])) {
      return 'vibration';
    }

    // Default fallback
    return 'temperature';
  }

  /**
   * Get all possible metrics for a device
   * @param deviceId The device identifier
   * @param deviceName The device name
   * @returns Array of possible metrics for this device
   */
  static getMetricsForDevice(deviceId: string, deviceName?: string): MetricOption[] {
    const searchText = (deviceId + ' ' + (deviceName || '')).toLowerCase();
    const metrics: MetricOption[] = [];

    // Temperature devices
    if (this.matchesPattern(searchText, ['temp', 'temperature', 'thermal', 'sensor'])) {
      metrics.push(
        this.getMetricOption('temperature'),
        this.getMetricOption('humidity'),
        this.getMetricOption('pressure')
      );
    }
    // Humidity devices
    else if (this.matchesPattern(searchText, ['humid', 'moisture'])) {
      metrics.push(
        this.getMetricOption('humidity'),
        this.getMetricOption('temperature')
      );
    }
    // Pressure devices
    else if (this.matchesPattern(searchText, ['press', 'pressure', 'barometer'])) {
      metrics.push(
        this.getMetricOption('pressure'),
        this.getMetricOption('temperature')
      );
    }
    // Power/Electrical devices
    else if (this.matchesPattern(searchText, ['power', 'electrical', 'volt', 'current'])) {
      metrics.push(
        this.getMetricOption('voltage'),
        this.getMetricOption('current'),
        this.getMetricOption('power')
      );
    }
    // Environmental sensors
    else if (this.matchesPattern(searchText, ['environment', 'air', 'quality'])) {
      metrics.push(
        this.getMetricOption('temperature'),
        this.getMetricOption('humidity'),
        this.getMetricOption('co2'),
        this.getMetricOption('pressure')
      );
    }
    // Default: add common metrics
    else {
      metrics.push(
        this.getMetricOption('temperature'),
        this.getMetricOption('humidity'),
        this.getMetricOption('pressure')
      );
    }

    return metrics;
  }

  /**
   * Get metric option by name
   */
  private static getMetricOption(name: string): MetricOption {
    return this.ALL_METRICS.find(m => m.name === name) || this.ALL_METRICS[0];
  }

  /**
   * Check if text matches any of the patterns
   */
  private static matchesPattern(text: string, patterns: string[]): boolean {
    return patterns.some(pattern => text.includes(pattern));
  }

  /**
   * Get metric unit by name
   */
  static getMetricUnit(metricName: string): string {
    const metric = this.ALL_METRICS.find(m => m.name === metricName);
    return metric?.unit || '';
  }

  /**
   * Get metric color by name
   */
  static getMetricColor(metricName: string): string {
    const metric = this.ALL_METRICS.find(m => m.name === metricName);
    return metric?.color || '#999999';
  }
}
