import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Hesap } from '../models/hesap-model';
import { HesapCekYatir } from '../models/hesap-cek-yatir-model';
import { HesapDashboard } from '../models/hesap-dashboard-model';
import { HesapFiltre } from '../models/hesap-filtre-model';
import { DashboardFiltre } from '../../../shared/models/dashboard-filtre-model';
import { environment } from '../../../../environments/environment';
import { dashboardOzetGetir } from '../../../shared/utils/dashboard-api';

@Injectable({
  providedIn: 'root'
})
export class HesapApi {

  private readonly apiUrl = `${environment.apiBaseUrl}/Hesap`;

  constructor(private http: HttpClient) {
  }

  ekle(dto: Partial<Hesap>): Observable<Hesap> {
    return this.http.post<Hesap>(
      `${this.apiUrl}/Ekle`,
      dto
    );
  }

  listele(kriterler: HesapFiltre): Observable<Hesap[]> {
    return this.http.post<Hesap[]>(
      `${this.apiUrl}/listele`,
      kriterler
    );
  }

  paraCekYatir(dto: HesapCekYatir): Observable<HesapCekYatir> {
    return this.http.post<HesapCekYatir>(
      `${this.apiUrl}/ParaCekYatir`,
      dto
    );
  }

  guncelle(hesapId: number, dto: Hesap): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/Guncelle/${hesapId}`,
      dto
    );
  }

  dashboardOzet(filtre?: DashboardFiltre): Observable<HesapDashboard> {
    return dashboardOzetGetir(this.http, this.apiUrl, filtre);
  }
}
