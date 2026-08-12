import {
  Injectable
} from '@angular/core';

import {
  HttpClient
} from '@angular/common/http';

import {
  Observable
} from 'rxjs';

import {
  Musteri
} from '../models/musteri-model';

import {
  MusteriFiltre
} from '../models/musteri-filtre-model';

import {
  MusteriKaydet
} from '../models/musteri-kaydet-model';

import {
  MusteriTamKaydet,
  MusteriTamKaydetSonuc
} from '../models/musteri-tam-kaydet-model';

import {
  MusteriDashboard
} from '../models/musteri-dashboard-model';

import {
  DashboardFiltre
} from '../../../shared/models/dashboard-filtre-model';

import { environment } from '../../../../environments/environment';

import {
  dashboardOzetGetir
} from '../../../shared/utils/dashboard-api';

@Injectable({
  providedIn: 'root'
})
export class MusteriApi {

  private readonly apiUrl =
    `${environment.apiBaseUrl}/Musteri`;

  constructor(
    private http: HttpClient
  ) {
  }

  listele(
    kriterler: MusteriFiltre
  ): Observable<Musteri[]> {

    return this.http.post<Musteri[]>(
      `${this.apiUrl}/Listele`,
      kriterler
    );
  }

  getirById(id: number): Observable<Musteri> {
    return this.http.post<Musteri>(
      `${this.apiUrl}/GetirById/${id}`,
      null
    );
  }

  tamKaydet(
    kayit: MusteriTamKaydet
  ): Observable<MusteriTamKaydetSonuc> {

    return this.http.post<MusteriTamKaydetSonuc>(
      `${this.apiUrl}/TamKaydet`,
      kayit
    );
  }

  guncelle(
    id: number,
    musteri: MusteriKaydet
  ): Observable<void> {

    return this.http.put<void>(
      `${this.apiUrl}/Guncelle/${id}`,
      musteri
    );
  }

  dashboardOzet(
    filtre?: DashboardFiltre
  ): Observable<MusteriDashboard> {

    return dashboardOzetGetir(
      this.http,
      this.apiUrl,
      filtre
    );
  }
}
