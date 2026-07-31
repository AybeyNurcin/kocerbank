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

  }

  hesapSec(
    hesap: Hesap
  ): void {

    this.seciliHesap =
      hesap;

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
