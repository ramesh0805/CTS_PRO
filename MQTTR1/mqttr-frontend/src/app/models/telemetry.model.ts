export interface Telemetry {
  id: number;
  deviceId: string;
  metric: string;
  value: number;
  timestamp: string;
}

export interface TelemetryDto {
  deviceId: string;
  metric: string;
  value: number;
  timestamp?: string;
}

export interface PagedResult<T> {
  data: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

// Chart.js Dataset interface for time-series data
export interface ChartDataset {
  label: string;
  data: number[];
  borderColor: string;
  backgroundColor: string;
  borderWidth: number;
  fill: boolean;
  tension: number;
  pointRadius: number;
  pointBackgroundColor: string;
  pointBorderColor: string;
  pointBorderWidth: number;
}

// Multi-metric chart data (all metrics for a device)
export interface ChartDataMultiMetric {
  deviceId: string;
  startTime: string;
  endTime: string;
  labels: string[];
  datasets: ChartDataset[];
  dataPointCount: number;
}

// Single-metric chart data
export interface ChartDataSingleMetric {
  deviceId: string;
  metric: string;
  startTime: string;
  endTime: string;
  labels: string[];
  dataset: ChartDataset;
  dataPointCount: number;
}

// Union type for chart data
export type ChartData = ChartDataMultiMetric | ChartDataSingleMetric;

// Real-time data point for live updates
export interface RealtimeDataPoint {
  timestamp: string;
  value: number;
  metric: string;
}

// Live data update response
export interface LiveDataUpdate {
  deviceId: string;
  metric: string;
  value: number;
  timestamp: string;
  min: number;
  max: number;
  average: number;
}

