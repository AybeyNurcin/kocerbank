import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly oturumAnahtari =
    'kocerbank_personel_oturumu';

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
