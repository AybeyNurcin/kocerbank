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

  // Aktivite zaman damgasının depoya en fazla bu sıklıkla
  // yazılmasını sağlar (mousemove gibi sık tetiklenen olaylarda
  // gereksiz yazım yapılmasını önler).
  private readonly AKTIVITE_YAZMA_ARALIGI_MS = 10 * 1000;

  private readonly apiUrl =
    'http://localhost:5107/api/Personel';

  // Oturum bilgilerinin saklanacağı depoyu belirler. Güvenlik nedeniyle
  // sessionStorage kullanılır (tarayıcı sekmesi kapatılınca oturum
  // bilgileri silinir). Depolama türü değiştirilmek istenirse
  // sadece bu fonksiyon güncellenmelidir.
  private oturumDeposu(): Storage {
    return sessionStorage;
  }

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

    this.oturumDeposu().setItem(
      this.oturumAnahtari,
      sicilNo
    );

    this.oturumDeposu().setItem(
      this.oturumAdSoyadAnahtari,
      `${personel.ad} ${personel.soyad}`
    );

    this.oturumDeposu().setItem(
      this.oturumIdAnahtari,
      String(personel.id)
    );

    this.oturumDeposu().setItem(
      this.oturumSonAktiviteAnahtari,
      String(Date.now())
    );

  }

  oturumKapat(): void {

    this.oturumDeposu().removeItem(
      this.oturumAnahtari
    );

    this.oturumDeposu().removeItem(
      this.oturumAdSoyadAnahtari
    );

    this.oturumDeposu().removeItem(
      this.oturumIdAnahtari
    );

    this.oturumDeposu().removeItem(
      this.oturumSonAktiviteAnahtari
    );

  }

  // Profil düzenleme sonrasında ekranda gösterilen
  // ad-soyad bilgisini oturumdan güncellemek için kullanılır.
  adSoyadGuncelle(ad: string, soyad: string): void {

    this.oturumDeposu().setItem(
      this.oturumAdSoyadAnahtari,
      `${ad} ${soyad}`
    );

  }

  oturumAcikMi(): boolean {

    if (this.oturumDeposu().getItem(this.oturumAnahtari) === null) {
      return false;
    }

    const sonAktivite = this.oturumDeposu().getItem(
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

    return this.oturumDeposu().getItem(
      this.oturumAnahtari
    );

  }

  personelAdSoyadGetir(): string | null {

    return this.oturumDeposu().getItem(
      this.oturumAdSoyadAnahtari
    );

  }

  personelIdGetir(): number | null {

    const id = this.oturumDeposu().getItem(
      this.oturumIdAnahtari
    );

    return id !== null ? Number(id) : null;

  }

  private aktiviteyiGuncelle(): void {

    if (this.oturumDeposu().getItem(this.oturumAnahtari) === null) {
      return;
    }

    const sonAktivite = Number(
      this.oturumDeposu().getItem(this.oturumSonAktiviteAnahtari) ?? 0
    );

    if (Date.now() - sonAktivite < this.AKTIVITE_YAZMA_ARALIGI_MS) {
      return;
    }

    this.oturumDeposu().setItem(
      this.oturumSonAktiviteAnahtari,
      String(Date.now())
    );

  }
}
