import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import {
  ActivatedRoute
} from '@angular/router';

import {
  HesapApi
} from '../services/hesap-api';

import {
  Hesap
} from '../models/hesap-model';

import {
  HesapCekYatir
} from '../models/hesap-cek-yatir-model';

import {
  HesapHareketTipleri
} from '../../../shared/enums/hesap-hareket-tipleri-enum';

import {
  HesapDurumu
} from '../../../shared/enums/hesap-durumu-enum';

import {
  extractErrorMessage
} from '../../../shared/utils/hata-mesaji';

import {
  MusteriApi
} from '../../musteriler/services/musteri-api';

import {
  Musteri
} from '../../musteriler/models/musteri-model';

type IslemTipi = 'Cek' | 'Yatir';
type EkranTipi = 'form' | 'onizleme' | 'basarili';

interface OnizlemeBilgisi {
  adSoyad: string;
  hesapAdi: string;
  iban: string;
  tutar: number;
}

const IBAN_DOGRULAMA_GECIKMESI_MS = 500;

@Component({
  selector: 'app-para-cek-yatir',
  standalone: false,
  templateUrl: './para-cek-yatir.html',
  styleUrl: './para-cek-yatir.css'
})
export class ParaCekYatir
  implements OnInit {

  islemTipi: IslemTipi = 'Cek';
  ekran: EkranTipi = 'form';

  iban: string = 'TR';
  isimSoyisim: string = '';

  readonly hazirTutarlar: number[] = [200, 500, 1000, 3000];
  seciliHazirTutar: number | null = null;

  tutar: number | null = null;

  onizlemeBilgisi: OnizlemeBilgisi | null = null;

  yukleniyorMu: boolean = false;
  ibanDogrulaniyorMu: boolean = false;
  hataMesaji: string = '';

  maskelenmisAdSoyad: string = '';

  private dogrulananHesap: Hesap | null = null;
  private dogrulananMusteri: Musteri | null = null;

  private ibanZamanlayici: ReturnType<typeof setTimeout> | null = null;
  private basariliZamanlayici: ReturnType<typeof setTimeout> | null = null;

  private isimOtomatikDoldurulacakMi: boolean = false;

  constructor(
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private hesapApi: HesapApi,
    private musteriApi: MusteriApi
  ) {
  }

  ngOnInit(): void {

    const gelenIban =
      this.route.snapshot.queryParamMap.get('iban');

    if (gelenIban) {

      this.isimOtomatikDoldurulacakMi = true;
      this.ibanDegisti(gelenIban);

    }

  }

  sekmeSec(
    tip: IslemTipi
  ): void {

    if (this.islemTipi === tip) {
      return;
    }

    this.islemTipi = tip;

    this.seciliHazirTutar = null;
    this.tutar = null;
    this.onizlemeBilgisi = null;
    this.hataMesaji = '';

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

  ibanDegisti(
    deger: string
  ): void {

    const temiz =
      (deger ?? '').replace(/\s+/g, '').toUpperCase();

    if (temiz.length === 0) {

      this.iban = 'TR';

    } else {

      let govde: string;

      if (temiz.startsWith('TR')) {
        govde = temiz.slice(2);
      } else if (temiz === 'T') {
        govde = '';
      } else {
        govde = temiz;
      }

      const tamIban =
        ('TR' + govde).slice(0, 26);

      this.iban =
        tamIban.match(/.{1,4}/g)?.join(' ') ?? tamIban;

    }

    this.ibanDogrulamasiniSifirla();

    if (this.ibanGecerliMi) {

      this.ibanZamanlayici =
        setTimeout(
          () => this.ibanDogrula(),
          IBAN_DOGRULAMA_GECIKMESI_MS
        );

    }

  }

  ibanYapistir(
    event: ClipboardEvent
  ): void {

    event.preventDefault();

    const yapistirilanMetin =
      event.clipboardData?.getData('text') ?? '';

    this.ibanDegisti(yapistirilanMetin);

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

  get isimSoyisimGecerliMi(): boolean {

    return this.isimSoyisim.trim().length > 0;

  }

  /*
   * BAKİYE YETERSİZLİĞİ
   *
   * Yalnızca para çekme işleminde anlamlıdır;
   * para yatırmada bakiye kısıtı yoktur.
   */
  get bakiyeYetersizMi(): boolean {

    return (
      this.islemTipi === 'Cek' &&
      this.dogrulananHesap !== null &&
      this.tutar !== null &&
      this.tutar > 0 &&
      this.tutar > this.dogrulananHesap.bakiye
    );

  }

  get hesapAktifDegilMi(): boolean {

    return (
      this.dogrulananHesap !== null &&
      this.dogrulananHesap.hesapDurumKodu !== HesapDurumu.Aktif
    );

  }

  get ileriAktifMi(): boolean {

    return (
      this.ibanGecerliMi &&
      this.tutarGecerliMi &&
      this.isimSoyisimGecerliMi &&
      this.dogrulananHesap !== null &&
      !this.hesapAktifDegilMi &&
      !this.bakiyeYetersizMi &&
      !this.yukleniyorMu &&
      !this.ibanDogrulaniyorMu
    );

  }

  private ibanDogrulamasiniSifirla(): void {

    if (this.ibanZamanlayici !== null) {
      clearTimeout(this.ibanZamanlayici);
      this.ibanZamanlayici = null;
    }

    this.ibanDogrulaniyorMu = false;
    this.dogrulananHesap = null;
    this.dogrulananMusteri = null;
    this.maskelenmisAdSoyad = '';
    this.hataMesaji = '';

  }

  private ibanDogrula(): void {

    const temizIban =
      this.iban.replace(/\s+/g, '').toUpperCase();

    this.ibanDogrulaniyorMu = true;
    this.hataMesaji = '';

    this.hesapApi
      .listele({ iban: temizIban })
      .subscribe({

        next: (hesaplar: Hesap[]) => {

          const hesap =
            hesaplar[0] ?? null;

          if (hesap === null) {

            this.ibanDogrulaniyorMu = false;

            this.hataMesaji =
              'Bu IBAN\'a ait hesap bulunamadı.';

            this.cdr.detectChanges();

            return;

          }

          this.musteriApi
            .getirById(hesap.musteriBilgileriId)
            .subscribe({

              next: (musteri: Musteri) => {

                this.dogrulananHesap = hesap;
                this.dogrulananMusteri = musteri;

                this.maskelenmisAdSoyad =
                  this.adiMaskele(
                    `${musteri.ad} ${musteri.soyad}`
                  );

                if (this.isimOtomatikDoldurulacakMi) {

                  this.isimSoyisim =
                    `${musteri.ad} ${musteri.soyad}`;

                  this.isimOtomatikDoldurulacakMi = false;

                }

                this.ibanDogrulaniyorMu = false;

                if (hesap.hesapDurumKodu !== HesapDurumu.Aktif) {

                  this.hataMesaji =
                    'Bu hesap aktif değildir.';

                }

                this.cdr.detectChanges();

              },

              error: (hata) => {

                console.error(
                  'Hesap sahibi bilgisi getirme hatası:',
                  hata
                );

                this.ibanDogrulaniyorMu = false;

                this.hataMesaji =
                  extractErrorMessage(
                    hata,
                    'Hesap sahibi bilgileri getirilirken bir hata oluştu.'
                  );

                this.cdr.detectChanges();

              }

            });

        },

        error: (hata) => {

          console.error(
            'IBAN ile hesap arama hatası:',
            hata
          );

          this.ibanDogrulaniyorMu = false;

          this.hataMesaji =
            extractErrorMessage(
              hata,
              'Hesap aranırken bir hata oluştu.'
            );

          this.cdr.detectChanges();

        }

      });

  }

  private adiMaskele(
    adSoyad: string
  ): string {

    return adSoyad
      .trim()
      .split(/\s+/)
      .map(
        (kelime) =>
          kelime.length <= 2
            ? kelime
            : kelime.slice(0, 2) + '*'.repeat(kelime.length - 2)
      )
      .join(' ');

  }

  private isimSoyisimEslesiyorMu(): boolean {

    if (this.dogrulananMusteri === null) {
      return false;
    }

    const normallestir =
      (deger: string) =>
        deger
          .trim()
          .replace(/\s+/g, ' ')
          .toLocaleUpperCase('tr-TR')
          .replace(/İ/g, 'I');

    const gercekAdSoyad =
      `${this.dogrulananMusteri.ad} ${this.dogrulananMusteri.soyad}`;

    return (
      normallestir(gercekAdSoyad) ===
      normallestir(this.isimSoyisim)
    );

  }

  ileriGec(): void {

    if (
      !this.ileriAktifMi ||
      this.tutar === null ||
      this.dogrulananHesap === null ||
      this.dogrulananMusteri === null
    ) {
      return;
    }

    if (!this.isimSoyisimEslesiyorMu()) {

      this.hataMesaji =
        'Girilen isim soyisim, IBAN sahibiyle eşleşmiyor.';

      return;

    }

    const temizIban =
      this.iban.replace(/\s+/g, '').toUpperCase();

    this.onizlemeBilgisi = {
      adSoyad: `${this.dogrulananMusteri.ad} ${this.dogrulananMusteri.soyad}`,
      hesapAdi: this.dogrulananHesap.hesapAdi,
      iban: temizIban,
      tutar: this.tutar
    };

    this.hataMesaji = '';
    this.ekran = 'onizleme';

  }

  geriDon(): void {

    this.hataMesaji = '';
    this.ekran = 'form';

  }

  onayla(): void {

    if (this.dogrulananHesap === null || this.onizlemeBilgisi === null) {
      return;
    }

    this.yukleniyorMu = true;
    this.hataMesaji = '';

    const dto: HesapCekYatir = {
      hesapId: this.dogrulananHesap.id,
      islemTipi:
        this.islemTipi === 'Cek'
          ? HesapHareketTipleri.ParaCekme
          : HesapHareketTipleri.ParaYatirma,
      tutar: this.onizlemeBilgisi.tutar,
      hareketId: 0,
      yeniBakiye: 0
    };

    this.hesapApi
      .paraCekYatir(dto)
      .subscribe({

        next: () => {

          this.yukleniyorMu = false;
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

          this.cdr.detectChanges();

        },

        error: (hata) => {

          console.error(
            'Para çek/yatır işlemi hatası:',
            hata
          );

          this.yukleniyorMu = false;

          this.hataMesaji =
            extractErrorMessage(
              hata,
              'İşlem gerçekleştirilirken bir hata oluştu.'
            );

          this.cdr.detectChanges();

        }

      });

  }

  private formuSifirla(): void {

    if (this.basariliZamanlayici !== null) {
      clearTimeout(this.basariliZamanlayici);
      this.basariliZamanlayici = null;
    }

    this.ibanDogrulamasiniSifirla();

    this.iban = 'TR';
    this.isimSoyisim = '';
    this.tutar = null;
    this.seciliHazirTutar = null;
    this.onizlemeBilgisi = null;
    this.hataMesaji = '';

  }
}