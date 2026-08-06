import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import {
  MusteriApi
} from '../../musteriler/services/musteri-api';

import {
  MusteriDashboard
} from '../../musteriler/models/musteri-dashboard-model';

import {
  SubeApi
} from '../../subeler/services/sube-api';

import {
  SubeDashboard
} from '../../subeler/models/sube-dashboard-model';

import {
  PersonelApi
} from '../../personeller/services/personel-api';

import {
  PersonelDashboard
} from '../../personeller/models/personel-dashboard-model';

import {
  HesapApi
} from '../../hesaplar/services/hesap-api';

import {
  HesapDashboard
} from '../../hesaplar/models/hesap-dashboard-model';

import {
  DovizKuruApi
} from '../../doviz-kuru/services/doviz-kuru-api';

import {
  DovizKurulari
} from '../../doviz-kuru/models/doviz-kuru-model';

interface DovizKuruSatiri {
  kod: string;
  ad: string;
  alis: number;
  satis: number;
}

@Component({
  selector: 'app-dashboard',
  standalone: false,
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard
  implements OnInit {

  // MÜŞTERİLER (CANLI VERİ)

  musteriOzet: MusteriDashboard | null = null;

  musteriYukleniyorMu: boolean = false;
  musteriHataMesaji: string = '';


  // ŞUBELER (CANLI VERİ)

  subeOzet: SubeDashboard | null = null;

  subeYukleniyorMu: boolean = false;
  subeHataMesaji: string = '';


  // PERSONELLER (CANLI VERİ)

  personelOzet: PersonelDashboard | null = null;

  personelYukleniyorMu: boolean = false;
  personelHataMesaji: string = '';


  // HESAPLAR (CANLI VERİ)

  hesapOzet: HesapDashboard | null = null;

  hesapYukleniyorMu: boolean = false;
  hesapHataMesaji: string = '';


  // DÖVİZ KURU (CANLI VERİ - TCMB)

  private readonly dovizAdlari: { [kod: string]: string } = {
    USD: 'Amerikan Doları',
    EUR: 'Euro'
  };

  dovizKurlari: DovizKuruSatiri[] = [];
  dovizKuruTarihi: string | null = null;

  dovizKuruYukleniyorMu: boolean = false;
  dovizKuruHataMesaji: string = '';


  constructor(
    private musteriApi: MusteriApi,
    private subeApi: SubeApi,
    private personelApi: PersonelApi,
    private hesapApi: HesapApi,
    private dovizKuruApi: DovizKuruApi,
    private changeDetector: ChangeDetectorRef
  ) {
  }


  ngOnInit(): void {

    this.musteriOzetGetir();
    this.subeOzetGetir();
    this.personelOzetGetir();
    this.hesapOzetGetir();
    this.dovizKurlariGetir();

  }


  musteriOzetGetir(): void {

    this.musteriYukleniyorMu = true;
    this.musteriHataMesaji = '';

    this.musteriApi
      .dashboardOzet()
      .subscribe({

        next: (
          ozet: MusteriDashboard
        ) => {

          this.musteriOzet = ozet;
          this.musteriYukleniyorMu = false;

          this.changeDetector.markForCheck();

        },

        error: (hata) => {

          console.error(
            'Müşteri dashboard özeti getirilirken hata:',
            hata
          );

          this.musteriOzet = null;

          this.musteriHataMesaji =
            'Müşteri özet bilgileri getirilirken bir hata oluştu.';

          this.musteriYukleniyorMu = false;

          this.changeDetector.markForCheck();

        }

      });
  }


  subeOzetGetir(): void {

    this.subeYukleniyorMu = true;
    this.subeHataMesaji = '';

    this.subeApi
      .dashboardOzet()
      .subscribe({

        next: (
          ozet: SubeDashboard
        ) => {

          this.subeOzet = ozet;
          this.subeYukleniyorMu = false;

          this.changeDetector.markForCheck();

        },

        error: (hata) => {

          console.error(
            'Şube dashboard özeti getirilirken hata:',
            hata
          );

          this.subeOzet = null;

          this.subeHataMesaji =
            'Şube özet bilgileri getirilirken bir hata oluştu.';

          this.subeYukleniyorMu = false;

          this.changeDetector.markForCheck();

        }

      });
  }


  personelOzetGetir(): void {

    this.personelYukleniyorMu = true;
    this.personelHataMesaji = '';

    this.personelApi
      .dashboardOzet()
      .subscribe({

        next: (
          ozet: PersonelDashboard
        ) => {

          this.personelOzet = ozet;
          this.personelYukleniyorMu = false;

          this.changeDetector.markForCheck();

        },

        error: (hata) => {

          console.error(
            'Personel dashboard özeti getirilirken hata:',
            hata
          );

          this.personelOzet = null;

          this.personelHataMesaji =
            'Personel özet bilgileri getirilirken bir hata oluştu.';

          this.personelYukleniyorMu = false;

          this.changeDetector.markForCheck();

        }

      });
  }


  hesapOzetGetir(): void {

    this.hesapYukleniyorMu = true;
    this.hesapHataMesaji = '';

    this.hesapApi
      .dashboardOzet()
      .subscribe({

        next: (
          ozet: HesapDashboard
        ) => {

          this.hesapOzet = ozet;
          this.hesapYukleniyorMu = false;

          this.changeDetector.markForCheck();

        },

        error: (hata) => {

          console.error(
            'Hesap dashboard özeti getirilirken hata:',
            hata
          );

          this.hesapOzet = null;

          this.hesapHataMesaji =
            'Hesap özet bilgileri getirilirken bir hata oluştu.';

          this.hesapYukleniyorMu = false;

          this.changeDetector.markForCheck();

        }

      });
  }


  dovizKurlariGetir(): void {

    this.dovizKuruYukleniyorMu = true;
    this.dovizKuruHataMesaji = '';

    this.dovizKuruApi
      .guncelKurlar()
      .subscribe({

        next: (
          kurlar: DovizKurulari
        ) => {

          this.dovizKuruTarihi = kurlar.kurTarihi;

          this.dovizKurlari = Object.keys(kurlar.kurlar).map(
            (kod) => ({
              kod: kod,
              ad: this.dovizAdlari[kod] ?? kod,
              alis: kurlar.kurlar[kod].alis,
              satis: kurlar.kurlar[kod].satis
            })
          );

          this.dovizKuruYukleniyorMu = false;

          this.changeDetector.markForCheck();

        },

        error: (hata) => {

          console.error(
            'Döviz kurları getirilirken hata:',
            hata
          );

          this.dovizKurlari = [];

          this.dovizKuruHataMesaji =
            'Döviz kurları getirilirken bir hata oluştu.';

          this.dovizKuruYukleniyorMu = false;

          this.changeDetector.markForCheck();

        }

      });
  }
}
