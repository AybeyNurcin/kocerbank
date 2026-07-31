import {
  ChangeDetectorRef,
  Component
} from '@angular/core';

type IslemTipi = 'Cek' | 'Yatir';
type EkranTipi = 'form' | 'onizleme' | 'basarili';

interface HesapSahibi {
  adSoyad: string;
  hesapAdi: string;
}

interface OnizlemeBilgisi {
  adSoyad: string;
  hesapAdi: string;
  iban: string;
  tutar: number;
}

@Component({
  selector: 'app-para-cek-yatir',
  standalone: false,
  templateUrl: './para-cek-yatir.html',
  styleUrl: './para-cek-yatir.css'
})
export class ParaCekYatir {

  // Mock hesap sahibi verisi: gerçek bir API bağlantısı olmadığından
  // girilen IBAN'a göre bu listeden deterministik bir kayıt seçilir.
  private readonly mockHesapSahipleri: HesapSahibi[] = [
    { adSoyad: 'Ahmet Yılmaz', hesapAdi: 'Vadesiz Hesap' },
    { adSoyad: 'Elif Demir', hesapAdi: 'Vadeli Hesap' },
    { adSoyad: 'Mehmet Kaya', hesapAdi: 'Yatırım Hesabı' },
    { adSoyad: 'Zeynep Arslan', hesapAdi: 'Vadesiz Hesap' }
  ];

  islemTipi: IslemTipi = 'Cek';
  ekran: EkranTipi = 'form';

  iban: string = '';

  readonly hazirTutarlar: number[] = [200, 500, 1000, 3000];
  seciliHazirTutar: number | null = null;

  tutar: number | null = null;

  onizlemeBilgisi: OnizlemeBilgisi | null = null;

  private basariliZamanlayici: ReturnType<typeof setTimeout> | null = null;

  constructor(
    private cdr: ChangeDetectorRef
  ) {
  }

  sekmeSec(
    tip: IslemTipi
  ): void {

    if (this.islemTipi === tip) {
      return;
    }

    this.islemTipi = tip;
    this.formuSifirla();

  }

  hazirTutarSec(
    hazirTutar: number
  ): void {

    this.seciliHazirTutar = hazirTutar;
    this.tutar = hazirTutar;

  }

  ozelTutarGirildi(): void {

    this.seciliHazirTutar = null;

  }

  get ibanGecerliMi(): boolean {

    const temizIban =
      this.iban.replace(/\s+/g, '').toUpperCase();

    return /^TR\d{24}$/.test(temizIban);

  }

  get tutarGecerliMi(): boolean {

    return (
      this.tutar !== null &&
      this.tutar > 0
    );

  }

  get ileriAktifMi(): boolean {

    return (
      this.ibanGecerliMi &&
      this.tutarGecerliMi
    );

  }

  ileriGec(): void {

    if (!this.ileriAktifMi || this.tutar === null) {
      return;
    }

    const temizIban =
      this.iban.replace(/\s+/g, '').toUpperCase();

    const hesapSahibi =
      this.ibanSahibiBul(temizIban);

    this.onizlemeBilgisi = {
      adSoyad: hesapSahibi.adSoyad,
      hesapAdi: hesapSahibi.hesapAdi,
      iban: temizIban,
      tutar: this.tutar
    };

    this.ekran = 'onizleme';

  }

  geriDon(): void {

    this.ekran = 'form';

  }

  onayla(): void {

    this.ekran = 'basarili';

    this.basariliZamanlayici =
      setTimeout(
        () => {
          this.formuSifirla();
          this.ekran = 'form';
          this.cdr.detectChanges();
        },
        3000
      );

  }

  private ibanSahibiBul(
    iban: string
  ): HesapSahibi {

    let karakterToplami = 0;

    for (const karakter of iban) {
      karakterToplami += karakter.charCodeAt(0);
    }

    const index =
      karakterToplami % this.mockHesapSahipleri.length;

    return this.mockHesapSahipleri[index];

  }

  private formuSifirla(): void {

    if (this.basariliZamanlayici !== null) {
      clearTimeout(this.basariliZamanlayici);
      this.basariliZamanlayici = null;
    }

    this.iban = '';
    this.tutar = null;
    this.seciliHazirTutar = null;
    this.onizlemeBilgisi = null;

  }
}
