import { Injectable } from '@angular/core';
import {
  HttpClient
} from '@angular/common/http';

import {
  Observable
} from 'rxjs';

import { Sube } from '../models/sube-model';
import { SubeFiltre } from '../models/sube-filtre-model';
import { SubeDashboard } from '../models/sube-dashboard-model';
import { DashboardFiltre } from '../../../shared/models/dashboard-filtre-model';
import { environment } from '../../../../environments/environment';
import { dashboardOzetGetir } from '../../../shared/utils/dashboard-api';

@Injectable({
  providedIn: 'root'
})
export class SubeApi {

  private readonly apiUrl =
    `${environment.apiBaseUrl}/Sube`;

  constructor(private http: HttpClient) {
  }

  listele(
    kriterler: SubeFiltre = {}
  ): Observable<Sube[]> {

    return this.http.post<Sube[]>(
      `${this.apiUrl}/Listele`,
      kriterler
    );
  }

  ekle(sube: {
    subeAdi: string;
    subeTelefonNo: string;
    subeAdres: string;
    subeDurumKodu: number;
  }): Observable<Sube> {

    return this.http.post<Sube>(
      `${this.apiUrl}/Ekle`,
      sube
    );
  }

  guncelle(
    id: number,
    sube: {
      subeAdi: string;
      subeTelefonNo: string;
      subeAdres: string;
      subeDurumKodu: number;
    }
  ): Observable<void> {

    return this.http.put<void>(
      `${this.apiUrl}/${id}`,
      sube
    );
  }

  dashboardOzet(filtre?: DashboardFiltre): Observable<SubeDashboard> {

    return dashboardOzetGetir(this.http, this.apiUrl, filtre);
  }
}
