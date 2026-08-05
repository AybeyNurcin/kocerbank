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
  HesapHareketiApi
} from '../services/hesap-hareketi-api';

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

  hesapAdiDuzenleniyorMu: boolean = false;
  duzenlenenHesapAdi: string = '';
  hesapAdiKaydediliyorMu: boolean = false;
  hesapAdiHataMesaji: string = '';

  hesapHareketleri: HesapHareketi[] = [];
  hareketlerHataMesaji: string = '';

  tumHareketlerPopupAcikMi: boolean = false;
  hareketlerMevcutSayfa: number = 1;
  readonly hareketlerSayfaBasinaKayit: number = 10;

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
    private hesapHareketiApi: HesapHareketiApi,
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
      this.hesapHareketleri = [];

      return;

    }

    const ilkHesap =
      this.hesaplar[0];

    this.seciliHesapTipi =
      ilkHesap.hesapTipi;

    this.seciliHesap =
      ilkHesap;

    this.detayAcikMi = false;
    this.tumHareketlerPopupAcikMi = false;
    this.hesapAdiDuzenlemesiniSifirla();

    this.hesapHareketleriniGetir(
      ilkHesap.id
    );

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
    this.tumHareketlerPopupAcikMi = false;
    this.hesapAdiDuzenlemesiniSifirla();

    if (this.seciliHesap !== null) {
      this.hesapHareketleriniGetir(this.seciliHesap.id);
    } else {
      this.hesapHareketleri = [];
    }

  }

  hesapSec(
    hesap: Hesap
  ): void {

    this.seciliHesap =
      hesap;

    this.detayAcikMi = false;
    this.tumHareketlerPopupAcikMi = false;
    this.hesapAdiDuzenlemesiniSifirla();

    this.hesapHareketleriniGetir(hesap.id);

  }

  hesapDetaylariniAcKapat(): void {

    this.detayAcikMi =
      !this.detayAcikMi;

  }

  hesapAdiniDuzenlemeyeBasla(): void {

    if (this.seciliHesap === null) {
      return;
    }

    this.duzenlenenHesapAdi =
      this.seciliHesap.hesapAdi;

    this.hesapAdiHataMesaji = '';
    this.hesapAdiDuzenleniyorMu = true;

  }

  hesapAdiDuzenlemeyiIptalEt(): void {

    this.hesapAdiDuzenlemesiniSifirla();

  }

  hesapAdiniKaydet(): void {

    if (this.seciliHesap === null) {
      return;
    }

    const yeniHesapAdi =
      this.duzenlenenHesapAdi.trim();

    if (yeniHesapAdi.length === 0) {

      this.hesapAdiHataMesaji =
        'Hesap adı boş bırakılamaz.';

      return;

    }

    if (yeniHesapAdi.length > 50) {

      this.hesapAdiHataMesaji =
        'Hesap adı en fazla 50 karakter olabilir.';

      return;

    }

    if (yeniHesapAdi === this.seciliHesap.hesapAdi) {

      this.hesapAdiDuzenlemesiniSifirla();

      return;

    }

    const guncellenecekHesap: Hesap = {
      ...this.seciliHesap,
      hesapAdi: yeniHesapAdi
    };

    this.hesapAdiKaydediliyorMu = true;
    this.hesapAdiHataMesaji = '';

    this.hesapApi
      .guncelle(
        guncellenecekHesap.id,
        guncellenecekHesap
      )
      .subscribe({

        next: () => {

          if (this.seciliHesap !== null) {
            this.seciliHesap.hesapAdi = yeniHesapAdi;
          }

          const listedekiHesap =
            this.hesaplar.find(
              (hesap: Hesap) =>
                hesap.id === guncellenecekHesap.id
            );

          if (listedekiHesap) {
            listedekiHesap.hesapAdi = yeniHesapAdi;
          }

          this.hesapAdiKaydediliyorMu = false;
          this.hesapAdiDuzenleniyorMu = false;

          this.changeDetector.markForCheck();

        },

        error: (hata) => {

          console.error(
            'Hesap adı güncelleme hatası:',
            hata
          );

          this.hesapAdiKaydediliyorMu = false;

          this.hesapAdiHataMesaji =
            hata?.error?.mesaj ??
            'Hesap adı güncellenirken bir hata oluştu.';

          this.changeDetector.markForCheck();

        }

      });

  }

  private hesapAdiDuzenlemesiniSifirla(): void {

    this.hesapAdiDuzenleniyorMu = false;
    this.hesapAdiKaydediliyorMu = false;
    this.duzenlenenHesapAdi = '';
    this.hesapAdiHataMesaji = '';

  }

  private hesapHareketleriniGetir(
    hesapId: number
  ): void {

    this.hareketlerHataMesaji = '';

    this.hesapHareketiApi
      .listele(hesapId)
      .subscribe({

        next: (
          gelenHareketler: HesapHareketi[]
        ) => {

          this.hesapHareketleri =
            gelenHareketler;

          this.hareketlerMevcutSayfa = 1;

          this.changeDetector
            .markForCheck();

        },

        error: (hata) => {

          console.error(
            'Hesap hareketi listeleme hatası:',
            hata
          );

          this.hesapHareketleri = [];

          this.hareketlerHataMesaji =
            hata?.error?.mesaj ??
            'Hesap hareketleri getirilirken bir hata oluştu.';

          this.changeDetector
            .markForCheck();

        }

      });

  }

  private get siraliHesapHareketleri(): HesapHareketi[] {

    return [...this.hesapHareketleri].sort(
      (a: HesapHareketi, b: HesapHareketi) =>
        new Date(b.islemTarihi).getTime() -
        new Date(a.islemTarihi).getTime()
    );

  }

  get sonBesHesapHareketi(): HesapHareketi[] {

    return this.siraliHesapHareketleri.slice(0, 5);

  }

  get sayfadakiHesapHareketleri(): HesapHareketi[] {

    const baslangicIndeksi =
      (this.hareketlerMevcutSayfa - 1) *
      this.hareketlerSayfaBasinaKayit;

    return this.siraliHesapHareketleri.slice(
      baslangicIndeksi,
      baslangicIndeksi + this.hareketlerSayfaBasinaKayit
    );

  }

  get hareketlerToplamSayfa(): number {

    return Math.max(
      1,
      Math.ceil(
        this.hesapHareketleri.length /
        this.hareketlerSayfaBasinaKayit
      )
    );

  }

  tumHareketleriAc(): void {

    this.hareketlerMevcutSayfa = 1;
    this.tumHareketlerPopupAcikMi = true;

  }

  tumHareketleriKapat(): void {

    this.tumHareketlerPopupAcikMi = false;

  }

  hareketlerOncekiSayfa(): void {

    if (this.hareketlerMevcutSayfa > 1) {
      this.hareketlerMevcutSayfa--;
    }

  }

  hareketlerSonrakiSayfa(): void {

    if (this.hareketlerMevcutSayfa < this.hareketlerToplamSayfa) {
      this.hareketlerMevcutSayfa++;
    }

  }

  hareketGirisMi(
    tip: HesapHareketTipleri
  ): boolean {

    return (
      tip === HesapHareketTipleri.ParaYatirma ||
      tip === HesapHareketTipleri.GelenTransfer
    );

  }

  hareketAciklamasiniGetir(
    tip: HesapHareketTipleri
  ): string {

    switch (tip) {

      case HesapHareketTipleri.ParaYatirma:
        return 'Para Yatırma';

      case HesapHareketTipleri.ParaCekme:
        return 'Para Çekme';

      case HesapHareketTipleri.GelenTransfer:
        return 'Gelen Transfer';

      case HesapHareketTipleri.GidenTransfer:
        return 'Giden Transfer';

      default:
        return '-';

    }

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

  paraCekYatirEkraninaGit(): void {

    if (this.seciliHesap === null) {
      return;
    }

    this.router.navigate(
      ['/para-cek-yatir'],
      {
        queryParams: {
          iban: this.seciliHesap.iban
        }
      }
    );

  }
}
