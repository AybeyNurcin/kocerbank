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

@Injectable({
  providedIn: 'root'
})
export class SubeApi {

  private readonly apiUrl =
    'http://localhost:5107/api/Sube';

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

  dashboardOzet(): Observable<SubeDashboard> {

    return this.http.get<SubeDashboard>(
      `${this.apiUrl}/DashboardOzet`
    );
  }
}
