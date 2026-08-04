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

  private readonly oturumAdSoyadAnahtari =
    'kocerbank_personel_ad_soyad';

  private readonly oturumIdAnahtari =
    'kocerbank_personel_id';

  private readonly oturumSonAktiviteAnahtari =
    'kocerbank_personel_son_aktivite';

  // Bu süre kadar hareketsiz kalınırsa oturum otomatik sonlandırılır.
  private readonly OTURUM_ZAMAN_ASIMI_MS = 30 * 60 * 1000;

  // Aktivite zaman damgasının localStorage'a en fazla bu sıklıkla
  // yazılmasını sağlar (mousemove gibi sık tetiklenen olaylarda
  // gereksiz yazım yapılmasını önler).
  private readonly AKTIVITE_YAZMA_ARALIGI_MS = 10 * 1000;

  private readonly apiUrl =
    'http://localhost:5107/api/Personel';

  constructor(private http: HttpClient) {

    ['click', 'keydown', 'mousemove'].forEach((olayAdi) => {
      document.addEventListener(
        olayAdi,
        () => this.aktiviteyiGuncelle()
      );
    });

  }

  login(sicilNo: string, sifre: string): Observable<Personel> {

    return this.http.post<Personel>(
      `${this.apiUrl}/Login`,
      {
        sicil: sicilNo,
        sifre: sifre
      }
    ).pipe(
      tap((personel) => this.oturumAc(sicilNo, personel))
    );
  }

  oturumAc(sicilNo: string, personel: Personel): void {

    localStorage.setItem(
      this.oturumAnahtari,
      sicilNo
    );

    localStorage.setItem(
      this.oturumAdSoyadAnahtari,
      `${personel.ad} ${personel.soyad}`
    );

    localStorage.setItem(
      this.oturumIdAnahtari,
      String(personel.id)
    );

    localStorage.setItem(
      this.oturumSonAktiviteAnahtari,
      String(Date.now())
    );

  }

  oturumKapat(): void {

    localStorage.removeItem(
      this.oturumAnahtari
    );

    localStorage.removeItem(
      this.oturumAdSoyadAnahtari
    );

    localStorage.removeItem(
      this.oturumIdAnahtari
    );

    localStorage.removeItem(
      this.oturumSonAktiviteAnahtari
    );

  }

  // Profil düzenleme sonrasında ekranda gösterilen
  // ad-soyad bilgisini oturumdan güncellemek için kullanılır.
  adSoyadGuncelle(ad: string, soyad: string): void {

    localStorage.setItem(
      this.oturumAdSoyadAnahtari,
      `${ad} ${soyad}`
    );

  }

  oturumAcikMi(): boolean {

    if (localStorage.getItem(this.oturumAnahtari) === null) {
      return false;
    }

    const sonAktivite = localStorage.getItem(
      this.oturumSonAktiviteAnahtari
    );

    const gecenSure =
      Date.now() - Number(sonAktivite ?? 0);

    if (gecenSure > this.OTURUM_ZAMAN_ASIMI_MS) {
      this.oturumKapat();

      return false;
    }

    return true;

  }

  personelSicilNoGetir(): string | null {

    return localStorage.getItem(
      this.oturumAnahtari
    );

  }

  personelAdSoyadGetir(): string | null {

    return localStorage.getItem(
      this.oturumAdSoyadAnahtari
    );

  }

  personelIdGetir(): number | null {

    const id = localStorage.getItem(
      this.oturumIdAnahtari
    );

    return id !== null ? Number(id) : null;

  }

  private aktiviteyiGuncelle(): void {

    if (localStorage.getItem(this.oturumAnahtari) === null) {
      return;
    }

    const sonAktivite = Number(
      localStorage.getItem(this.oturumSonAktiviteAnahtari) ?? 0
    );

    if (Date.now() - sonAktivite < this.AKTIVITE_YAZMA_ARALIGI_MS) {
      return;
    }

    localStorage.setItem(
      this.oturumSonAktiviteAnahtari,
      String(Date.now())
    );

  }
}
