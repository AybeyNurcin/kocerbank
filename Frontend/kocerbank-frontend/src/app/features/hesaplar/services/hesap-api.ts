import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Hesap } from '../models/hesap-model';

@Injectable({
  providedIn: 'root'
})
export class HesapApi {

  private readonly apiUrl = 'http://localhost:5107/api/Hesap';

  constructor(private http: HttpClient) {
  }

  musteriyeGoreListele(musteriId: number): Observable<Hesap[]> {
    return this.http.post<Hesap[]>(
      `${this.apiUrl}/listele`,
      {
        musteriBilgileriId: musteriId
      }
    );
  }

  getirById(hesapId: number): Observable<Hesap> {
    return this.http.post<Hesap>(
      `${this.apiUrl}/GetirById/${hesapId}`,
      null
    );
  }
}
