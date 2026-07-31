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


  // HESAPLAR VE DÖVİZ KURU
  // Backend ve veritabanı bu bölümler için henüz hazır olmadığından
  // örnek/sabit verilerle gösterilir.

  hesapOzet = {
    toplam: 1284,
    aktif: 1190,
    pasif: 94
  };

  dovizKurlari = [
    { kod: 'USD', ad: 'Amerikan Doları', alis: 34.12, satis: 34.48 },
    { kod: 'EUR', ad: 'Euro', alis: 36.85, satis: 37.24 },
    { kod: 'GRAM ALTIN', ad: 'Gram Altın', alis: 2854.30, satis: 2861.75 }
  ];


  constructor(
    private musteriApi: MusteriApi,
    private subeApi: SubeApi,
    private personelApi: PersonelApi,
    private changeDetector: ChangeDetectorRef
  ) {
  }


  ngOnInit(): void {

    this.musteriOzetGetir();
    this.subeOzetGetir();
    this.personelOzetGetir();

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
}
