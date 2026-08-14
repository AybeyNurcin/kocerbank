import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import {
  Observable
} from 'rxjs';

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

import {
  DashboardFiltre
} from '../../../shared/models/dashboard-filtre-model';

import {
  tarihiIsoyaCevir
} from '../../../shared/utils/takvim';

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


  // TARİH ARALIĞI FİLTRESİ

  baslangicTarihi: string = '';
  bitisTarihi: string = '';

  // Aktif "son N ay/yıl" hızlı seçim butonu (vurgu için).
  aktifHizliAralik: number | null = null;


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

    this.ozetYukle(
      this.musteriApi.dashboardOzet(this.filtreOlustur()),
      {
        ozetAta: (deger) => this.musteriOzet = deger,
        yukleniyorMuAta: (deger) => this.musteriYukleniyorMu = deger,
        hataMesajiAta: (deger) => this.musteriHataMesaji = deger
      },
      'Müşteri dashboard özeti getirilirken hata:',
      'Müşteri özet bilgileri getirilirken bir hata oluştu.'
    );
  }


  subeOzetGetir(): void {

    this.ozetYukle(
      this.subeApi.dashboardOzet(this.filtreOlustur()),
      {
        ozetAta: (deger) => this.subeOzet = deger,
        yukleniyorMuAta: (deger) => this.subeYukleniyorMu = deger,
        hataMesajiAta: (deger) => this.subeHataMesaji = deger
      },
      'Şube dashboard özeti getirilirken hata:',
      'Şube özet bilgileri getirilirken bir hata oluştu.'
    );
  }


  personelOzetGetir(): void {

    this.ozetYukle(
      this.personelApi.dashboardOzet(this.filtreOlustur()),
      {
        ozetAta: (deger) => this.personelOzet = deger,
        yukleniyorMuAta: (deger) => this.personelYukleniyorMu = deger,
        hataMesajiAta: (deger) => this.personelHataMesaji = deger
      },
      'Personel dashboard özeti getirilirken hata:',
      'Personel özet bilgileri getirilirken bir hata oluştu.'
    );
  }


  hesapOzetGetir(): void {

    this.ozetYukle(
      this.hesapApi.dashboardOzet(this.filtreOlustur()),
      {
        ozetAta: (deger) => this.hesapOzet = deger,
        yukleniyorMuAta: (deger) => this.hesapYukleniyorMu = deger,
        hataMesajiAta: (deger) => this.hesapHataMesaji = deger
      },
      'Hesap dashboard özeti getirilirken hata:',
      'Hesap özet bilgileri getirilirken bir hata oluştu.'
    );
  }


  /*
   * Dört dashboard kartının (müşteri/şube/personel/hesap) ortak
   * "yükleniyor -> başarılı/hatalı" akışı. Alan adları her kart
   * için farklı olduğundan (musteriOzet, subeOzet, ...) doğrudan
   * atama yapmak yerine setter fonksiyonları alır.
   */
  private ozetYukle<T>(
    gozlemlenebilir: Observable<T>,
    alanlar: {
      ozetAta: (deger: T | null) => void;
      yukleniyorMuAta: (deger: boolean) => void;
      hataMesajiAta: (deger: string) => void;
    },
    hataLogEtiketi: string,
    varsayilanHataMesaji: string
  ): void {

    alanlar.yukleniyorMuAta(true);
    alanlar.hataMesajiAta('');

    gozlemlenebilir.subscribe({

      next: (ozet: T) => {

        alanlar.ozetAta(ozet);
        alanlar.yukleniyorMuAta(false);

        this.changeDetector.markForCheck();

      },

      error: (hata) => {

        console.error(hataLogEtiketi, hata);

        alanlar.ozetAta(null);
        alanlar.hataMesajiAta(varsayilanHataMesaji);
        alanlar.yukleniyorMuAta(false);

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


  filtreOlustur(): DashboardFiltre {

    return {
      baslangicTarihi: this.baslangicTarihi || null,
      bitisTarihi: this.bitisTarihi || null
    };

  }


  filtreUygula(): void {

    this.musteriOzetGetir();
    this.subeOzetGetir();
    this.personelOzetGetir();
    this.hesapOzetGetir();

  }


  filtreTemizle(): void {

    this.baslangicTarihi = '';
    this.bitisTarihi = '';
    this.aktifHizliAralik = null;

    this.filtreUygula();

  }


  baslangicTarihiDegisti(
    deger: string | null
  ): void {

    this.baslangicTarihi = deger ?? '';
    this.aktifHizliAralik = null;

  }


  bitisTarihiDegisti(
    deger: string | null
  ): void {

    this.bitisTarihi = deger ?? '';
    this.aktifHizliAralik = null;

  }


  // "Son N Ay/Yıl" hızlı seçimi: bugün ile bugünden N ay
  // öncesi arasındaki aralığı seçip filtreyi hemen uygular.
  hizliAralikSec(
    ayFarki: number
  ): void {

    const bugun = new Date();

    const gecmisTarih = new Date(bugun);
    gecmisTarih.setMonth(gecmisTarih.getMonth() - ayFarki);

    this.bitisTarihi = tarihiIsoyaCevir(
      bugun.getFullYear(),
      bugun.getMonth(),
      bugun.getDate()
    );

    this.baslangicTarihi = tarihiIsoyaCevir(
      gecmisTarih.getFullYear(),
      gecmisTarih.getMonth(),
      gecmisTarih.getDate()
    );

    this.aktifHizliAralik = ayFarki;

    this.filtreUygula();

  }
}
