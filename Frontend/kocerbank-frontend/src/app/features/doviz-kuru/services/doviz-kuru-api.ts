import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { DovizKurulari } from '../models/doviz-kuru-model';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DovizKuruApi {

  private readonly apiUrl = `${environment.apiBaseUrl}/DovizKuru`;

  constructor(private http: HttpClient) {
  }

  guncelKurlar(): Observable<DovizKurulari> {
    return this.http.post<DovizKurulari>(
      `${this.apiUrl}/GuncelKurlar`,
      null
    );
  }
}
