import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Device, DeviceDto } from '../models/device.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DeviceService {
  private apiUrl = `${environment.apiUrl}/devices`;

  constructor(private http: HttpClient) {}

  getAllDevices(): Observable<Device[]> {
    return this.http.get<Device[]>(this.apiUrl);
  }

  getDeviceById(id: number): Observable<Device> {
    return this.http.get<Device>(`${this.apiUrl}/${id}`);
  }

  getDeviceByDeviceId(deviceId: string): Observable<Device> {
    return this.http.get<Device>(`${this.apiUrl}/byDeviceId/${deviceId}`);
  }

  createDevice(device: DeviceDto): Observable<Device> {
    return this.http.post<Device>(this.apiUrl, device);
  }

  updateDevice(id: number, device: DeviceDto): Observable<Device> {
    return this.http.put<Device>(`${this.apiUrl}/${id}`, device);
  }

  deleteDevice(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  searchDevices(searchTerm: string): Observable<Device[]> {
    return this.http.get<Device[]>(`${this.apiUrl}/search?q=${encodeURIComponent(searchTerm)}`);
  }
}
