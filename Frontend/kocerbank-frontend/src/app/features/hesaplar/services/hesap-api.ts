import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Hesap } from '../models/hesap-model';
import { HesapCekYatir } from '../models/hesap-cek-yatir-model';
import { HesapDashboard } from '../models/hesap-dashboard-model';
import { DovizCinsi } from '../../../shared/enums/doviz-cinsi-enum';
import { DashboardFiltre } from '../../../shared/models/dashboard-filtre-model';

@Injectable({
  providedIn: 'root'
})
export class HesapApi {

  private readonly apiUrl = 'http://localhost:5107/api/Hesap';

  constructor(private http: HttpClient) {
  }

  ekle(dto: Partial<Hesap>): Observable<Hesap> {
    return this.http.post<Hesap>(
      `${this.apiUrl}/Ekle`,
      dto
    );
  }

  musteriyeGoreListele(musteriId: number): Observable<Hesap[]> {
    return this.http.post<Hesap[]>(
      `${this.apiUrl}/listele`,
      {
        musteriBilgileriId: musteriId
      }
    );
  }

  musterininTLHesaplariniGetir(musteriId: number): Observable<Hesap[]> {
    return this.http.post<Hesap[]>(
      `${this.apiUrl}/listele`,
      {
        musteriBilgileriId: musteriId,
        dovizCinsi: DovizCinsi.TL
      }
    );
  }

  ibanaGoreListele(iban: string): Observable<Hesap[]> {
    return this.http.post<Hesap[]>(
      `${this.apiUrl}/listele`,
      {
        iban: iban
      }
    );
  }

  getirById(hesapId: number): Observable<Hesap> {
    return this.http.post<Hesap>(
      `${this.apiUrl}/GetirById/${hesapId}`,
      null
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
    return this.http.post<HesapDashboard>(
      `${this.apiUrl}/DashboardOzet`,
      filtre ?? null
    );
  }
}
