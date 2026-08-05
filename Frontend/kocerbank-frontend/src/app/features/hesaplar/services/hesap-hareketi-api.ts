import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { HesapHareketi } from '../models/hesap-hareket-model';

@Injectable({
  providedIn: 'root'
})
export class HesapHareketiApi {

  private readonly apiUrl = 'http://localhost:5107/api/HesapHareketi';

  constructor(private http: HttpClient) {
  }

  listele(hesapBilgileriId: number): Observable<HesapHareketi[]> {
    return this.http.post<HesapHareketi[]>(
      `${this.apiUrl}/Listele/${hesapBilgileriId}`,
      null
    );
  }
}
