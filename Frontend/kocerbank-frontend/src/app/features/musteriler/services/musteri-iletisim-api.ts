import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { MusteriIletisim } from '../models/musteri-iletisim-model';
import { MusteriIletisimGuncelle } from '../models/musteri-iletisim-guncelle-model';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MusteriIletisimApi {

  private readonly apiUrl = `${environment.apiBaseUrl}/MusteriIletisim`;

  constructor(private http: HttpClient) {
  }

  getirByMusteriId(musteriId: number): Observable<MusteriIletisim> {
    return this.http.post<MusteriIletisim>(
      `${this.apiUrl}/GetirById/${musteriId}`,
      null
    );
  }

  guncelle(musteriId: number, iletisim: MusteriIletisimGuncelle): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/Guncelle/${musteriId}`,
      iletisim
    );
  }
}
