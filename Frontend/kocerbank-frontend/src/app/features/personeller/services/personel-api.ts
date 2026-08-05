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

@Injectable({
  providedIn: 'root'
})
export class PersonelApi {

  private readonly apiUrl =
    'http://localhost:5107/api/Personel';

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

    return this.http.get<Personel>(
      `${this.apiUrl}/GetirById/${id}`
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
      `${this.apiUrl}/Güncelle/${id}`,
      personel
    );
  }

  dashboardOzet(): Observable<PersonelDashboard> {

    return this.http.post<PersonelDashboard>(
      `${this.apiUrl}/DashboardOzet`,
      null
    );
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
