import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import {
  ActivatedRoute,
  Router
} from '@angular/router';

import {
  Hesap
} from '../models/hesap-model';

import {
  HesapApi
} from '../services/hesap-api';

import {
  HesapTipi
} from '../../../shared/enums/hesap-tipi-enum';

import {
  DovizCinsi
} from '../../../shared/enums/doviz-cinsi-enum';

import {
  HesapHareketTipleri
} from '../../../shared/enums/hesap-hareket-tipleri-enum';

import {
  HesapHareketi
} from '../models/hesap-hareket-model';

interface HesapTipiSecenegi {
  tip: HesapTipi;
  ad: string;
}

interface DovizToplami {
  dovizCinsi: DovizCinsi;
  toplam: number;
}

@Component({
  selector: 'app-hesap-listesi',
  standalone: false,
  templateUrl: './hesap-listesi.html',
  styleUrl: './hesap-listesi.css'
})
export class HesapListesi
  implements OnInit {

  musteriId: number = 0;

  hesaplar: Hesap[] = [];

  seciliHesapTipi: HesapTipi =
    HesapTipi.Vadesiz;

  seciliHesap: Hesap | null = null;

  detayAcikMi: boolean = false;

  yukleniyorMu: boolean = false;
  hataMesaji: string = '';

  readonly hesapTipleri: HesapTipiSecenegi[] = [
    {
      tip: HesapTipi.Vadesiz,
      ad: 'Vadesiz'
    },
    {
      tip: HesapTipi.Vadeli,
      ad: 'Vadeli'
    },
    {
      tip: HesapTipi.Yatirim,
      ad: 'Yatırım'
    }
  ];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private hesapApi: HesapApi,
    private changeDetector: ChangeDetectorRef
  ) {
  }

  ngOnInit(): void {

    const routeMusteriId =
      this.route.snapshot.paramMap.get(
        'musteriId'
      );

    this.musteriId =
      Number(routeMusteriId);

    if (
      !Number.isInteger(this.musteriId) ||
      this.musteriId <= 0
    ) {

      this.hataMesaji =
        'Geçersiz müşteri ID bilgisi.';

      return;

    }

    this.hesaplariGetir();

  }

  private hesaplariGetir(): void {

    this.yukleniyorMu = true;
    this.hataMesaji = '';

    this.hesapApi
      .musteriyeGoreListele(
        this.musteriId
      )
      .subscribe({

        next: (
          gelenHesaplar: Hesap[]
        ) => {

          this.hesaplar =
            gelenHesaplar;

          this.ilkSecimiYap();

          this.yukleniyorMu = false;

          this.changeDetector
            .markForCheck();

        },

        error: (hata) => {

          console.error(
            'Hesap listeleme hatası:',
            hata
          );

          this.hesaplar = [];
          this.seciliHesap = null;

          this.yukleniyorMu = false;

          this.hataMesaji =
            hata?.error?.mesaj ??
            'Hesaplar getirilirken bir hata oluştu.';

          this.changeDetector
            .markForCheck();

        }

      });

  }

  private ilkSecimiYap(): void {

    if (this.hesaplar.length === 0) {

      this.seciliHesap = null;

      return;

    }

    const ilkHesap =
      this.hesaplar[0];

    this.seciliHesapTipi =
      ilkHesap.hesapTipi;

    this.seciliHesap =
      ilkHesap;

    this.detayAcikMi = false;

  }

  get seciliTiptekiHesaplar(): Hesap[] {

    return this.hesaplar.filter(
      (hesap: Hesap) =>
        hesap.hesapTipi ===
        this.seciliHesapTipi
    );

  }

  hesapTipindekiHesapSayisi(
    hesapTipi: HesapTipi
  ): number {

    return this.hesaplar.filter(
      (hesap: Hesap) =>
        hesap.hesapTipi ===
        hesapTipi
    ).length;

  }

  hesapTipiToplamlariniGetir(
    hesapTipi: HesapTipi
  ): DovizToplami[] {

    const toplamlar =
      new Map<DovizCinsi, number>();

    this.hesaplar
      .filter(
        (hesap: Hesap) =>
          hesap.hesapTipi ===
          hesapTipi
      )
      .forEach(
        (hesap: Hesap) => {

          const mevcutToplam =
            toplamlar.get(
              hesap.dovizCinsi
            ) ?? 0;

          toplamlar.set(
            hesap.dovizCinsi,
            mevcutToplam +
            hesap.bakiye
          );

        }
      );

    return Array.from(
      toplamlar.entries()
    ).map(
      (
        [
          dovizCinsi,
          toplam
        ]
      ) => ({
        dovizCinsi,
        toplam
      })
    );

  }

  hesapTipiniSec(
    hesapTipi: HesapTipi
  ): void {

    this.seciliHesapTipi =
      hesapTipi;

    this.seciliHesap =
      this.seciliTiptekiHesaplar[0] ??
      null;

    this.detayAcikMi = false;

  }

  hesapSec(
    hesap: Hesap
  ): void {

    this.seciliHesap =
      hesap;

    this.detayAcikMi = false;

  }

  hesapDetaylariniAcKapat(): void {

    this.detayAcikMi =
      !this.detayAcikMi;

  }

  get seciliHesapHareketleri(): HesapHareketi[] {

    if (this.seciliHesap === null) {

      return [];

    }

    return [
      {
        id: 5,
        hesapId: this.seciliHesap.id,
        tarih: '2026-08-04',
        aciklama: 'Gelen Transfer',
        islemTipi: HesapHareketTipleri.GelenTransfer,
        tutar: 2000
      },
      {
        id: 4,
        hesapId: this.seciliHesap.id,
        tarih: '2026-08-04',
        aciklama: 'Giden Transfer',
        islemTipi: HesapHareketTipleri.GidenTransfer,
        tutar: 300
      },
      {
        id: 3,
        hesapId: this.seciliHesap.id,
        tarih: '2026-08-03',
        aciklama: 'Gelen Transfer',
        islemTipi: HesapHareketTipleri.GelenTransfer,
        tutar: 750
      },
      {
        id: 2,
        hesapId: this.seciliHesap.id,
        tarih: '2026-08-03',
        aciklama: 'Para Yatırma',
        islemTipi: HesapHareketTipleri.ParaYatirma,
        tutar: 1200
      },
      {
        id: 1,
        hesapId: this.seciliHesap.id,
        tarih: '2026-08-01',
        aciklama: 'Para Çekme',
        islemTipi: HesapHareketTipleri.ParaCekme,
        tutar: 500
      }
    ];

  }

  dovizSembolunuGetir(
    dovizCinsi: DovizCinsi
  ): string {

    switch (dovizCinsi) {

      case DovizCinsi.TL:
        return '₺';

      case DovizCinsi.USD:
        return '$';

      case DovizCinsi.EUR:
        return '€';

      default:
        return '';

    }

  }

  dovizKodunuGetir(
    dovizCinsi: DovizCinsi
  ): string {

    switch (dovizCinsi) {

      case DovizCinsi.TL:
        return 'TL';

      case DovizCinsi.USD:
        return 'USD';

      case DovizCinsi.EUR:
        return 'EUR';

      default:
        return '-';

    }

  }

  hesapTipiAdiniGetir(
    hesapTipi: HesapTipi
  ): string {

    return (
      this.hesapTipleri.find(
        (secenek: HesapTipiSecenegi) =>
          secenek.tip === hesapTipi
      )?.ad ?? 'Hesap'
    );

  }

  hesapDurumuAdiniGetir(
    durumKodu: number
  ): string {

    switch (durumKodu) {

      case 1:
        return 'Aktif';

      case 2:
        return 'Pasif';

      case 3:
        return 'Kapalı';

      default:
        return '-';

    }

  }

  musteriListesineDon(): void {

    this.router.navigate([
      '/musteriler'
    ]);

  }
}
