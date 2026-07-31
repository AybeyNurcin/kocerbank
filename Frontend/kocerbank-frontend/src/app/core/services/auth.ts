import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

import { Personel } from '../../features/personeller/models/personel-model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly oturumAnahtari =
    'kocerbank_personel_oturumu';

  private readonly apiUrl =
    'http://localhost:5107/api/Personel';

  constructor(private http: HttpClient) {
  }

  login(sicilNo: string, sifre: string): Observable<Personel> {

    return this.http.post<Personel>(
      `${this.apiUrl}/Login`,
      {
        sicil: sicilNo,
        sifre: sifre
      }
    ).pipe(
      tap(() => this.oturumAc(sicilNo))
    );
  }

  oturumAc(sicilNo: string): void {

    sessionStorage.setItem(
      this.oturumAnahtari,
      sicilNo
    );

  }

  oturumKapat(): void {

    sessionStorage.removeItem(
      this.oturumAnahtari
    );

  }

  oturumAcikMi(): boolean {

    return sessionStorage.getItem(
      this.oturumAnahtari
    ) !== null;

  }

  personelSicilNoGetir(): string | null {

    return sessionStorage.getItem(
      this.oturumAnahtari
    );

  }
}
