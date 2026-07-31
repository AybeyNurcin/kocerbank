import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { MusteriIletisim } from '../models/musteri-iletisim-model';
import { MusteriIletisimGuncelle } from '../models/musteri-iletisim-guncelle-model';

@Injectable({
  providedIn: 'root'
})
export class MusteriIletisimApi {

  private readonly apiUrl = 'http://localhost:5107/api/MusteriIletisim';

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
