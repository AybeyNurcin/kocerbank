import { Injectable } from '@angular/core';
import {
  HttpClient
} from '@angular/common/http';

import {
  Observable
} from 'rxjs';

import { Personel } from '../models/personel-model';
import { PersonelFiltre } from '../models/personel-filtre-model';
import { PersonelDashboard } from '../models/personel-dashboard-model';
import { DashboardFiltre } from '../../../shared/models/dashboard-filtre-model';
import { environment } from '../../../../environments/environment';
import { dashboardOzetGetir } from '../../../shared/utils/dashboard-api';

@Injectable({
  providedIn: 'root'
})
export class PersonelApi {

  private readonly apiUrl =
    `${environment.apiBaseUrl}/Personel`;

  constructor(private http: HttpClient) {
  }

  listele(
    kriterler: PersonelFiltre = {}
  ): Observable<Personel[]> {

    return this.http.post<Personel[]>(
      `${this.apiUrl}/listele`,
      kriterler
    );
  }

  getirById(id: number): Observable<Personel> {

    return this.http.post<Personel>(
      `${this.apiUrl}/GetirById/${id}`,
      null
    );
  }

  ekle(personel: {
    ad: string;
    soyad: string;
    rol: string;
    sifre: string;
    tckn: string;
    telefonNo: string;
    adres: string;
    email: string;
    subeKodu: string;
    durumKodu: number;
  }): Observable<Personel> {

    return this.http.post<Personel>(
      `${this.apiUrl}/Ekle`,
      personel
    );
  }

  guncelle(
    id: number,
    personel: {
      ad: string;
      soyad: string;
      rol: string;
      sifre: string;
      tckn: string;
      telefonNo: string;
      adres: string;
      email: string;
      subeKodu: string;
      durumKodu: number;
    }
  ): Observable<void> {

    return this.http.put<void>(
      `${this.apiUrl}/Guncelle/${id}`,
      personel
    );
  }

  dashboardOzet(filtre?: DashboardFiltre): Observable<PersonelDashboard> {

    return dashboardOzetGetir(this.http, this.apiUrl, filtre);
  }

  sifreDegistir(
    id: number,
    sifreler: {
      eskiSifre: string;
      yeniSifre: string;
    }
  ): Observable<void> {

    return this.http.put<void>(
      `${this.apiUrl}/SifreDegistir/${id}`,
      sifreler
    );
  }
}
