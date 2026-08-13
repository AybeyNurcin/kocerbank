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
  HesapDurumu
} from '../../../shared/enums/hesap-durumu-enum';

import {
  extractErrorMessage
} from '../../../shared/utils/hata-mesaji';

import {
  HesapHareketTipleri
} from '../../../shared/enums/hesap-hareket-tipleri-enum';

import {
  HesapHareketi
} from '../models/hesap-hareket-model';

import {
  ParaTransferApi
} from '../services/para-transfer-api';

import {
  ParaTransferiDetay
} from '../models/para-transfer-model';

import {
  TransferKanallari
} from '../../../shared/enums/transfer-kanallari-enum';

import {
  MusteriApi
} from '../../musteriler/services/musteri-api';

import {
  Musteri
} from '../../musteriler/models/musteri-model';


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

  /*
   * Para transferi ekranlarına geçerken müşteri
   * bilgilerini (TCKN vb.) otomatik doldurmak için
   * tutulur.
   */
  musteri: Musteri | null = null;

  hesaplar: Hesap[] = [];

  seciliHesapTipi: HesapTipi =
    HesapTipi.Vadesiz;

  seciliHesap: Hesap | null = null;

  detayAcikMi: boolean = false;

  ibanKopyalandiMi: boolean = false;

  yukleniyorMu: boolean = false;
  hataMesaji: string = '';


  // YENİ HESAP FORMU

  hesapFormuAcikMi: boolean = false;

  /*
   * Hesap oluşturulduktan sonra liste yenilenirken
   * yeni hesabın otomatik seçilmesi için tutulur.
   */
  yeniOlusturulanHesapId: number | null = null;


  // HESAP ADI DÜZENLEME

  hesapAdiDuzenleniyorMu: boolean = false;
  duzenlenenHesapAdi: string = '';
  hesapAdiKaydediliyorMu: boolean = false;
  hesapAdiHataMesaji: string = '';


  // HESAP DURUMU DÜZENLEME

  hesapDurumuKaydediliyorMu: boolean = false;
  hesapDurumuHataMesaji: string = '';


  // HESAP HAREKETLERİ

  hesapHareketleri: HesapHareketi[] = [];
  hareketlerHataMesaji: string = '';

  tumHareketlerPopupAcikMi: boolean = false;

  hareketlerMevcutSayfa: number = 1;

  readonly hareketlerSayfaBasinaKayit:
    number = 10;


  // HAREKET DETAYI

  seciliHareket: HesapHareketi | null = null;

  hareketDetayAcikMi: boolean = false;

  transferDetayi: ParaTransferiDetay | null = null;
  transferDetayiYukleniyorMu: boolean = false;
  transferDetayiHataMesaji: string = '';


  readonly hesapTipleri:
    HesapTipiSecenegi[] = [
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
    private paraTransferApi: ParaTransferApi,
    private musteriApi: MusteriApi,
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
    this.musteriyiGetir();
  }


  private musteriyiGetir(): void {

    this.musteriApi
      .getirById(this.musteriId)
      .subscribe({

        next: (musteri: Musteri) => {

          this.musteri = musteri;

          this.changeDetector
            .markForCheck();
        },

        error: (hata) => {

          console.error(
            'Müşteri bilgisi getirme hatası:',
            hata
          );
        }

      });
  }


  private hesaplariGetir(): void {

    this.yukleniyorMu = true;
    this.hataMesaji = '';

    this.hesapApi
      .listele({
        musteriBilgileriId: this.musteriId
      })
      .subscribe({

        next: (
          gelenHesaplar: Hesap[]
        ) => {

          this.hesaplar =
            gelenHesaplar;

          /*
           * Yeni bir hesap oluşturulmuşsa
           * yenilenen listede onu bulup seçer.
           */
          if (
            this.yeniOlusturulanHesapId !==
            null
          ) {

            const yeniHesap =
              this.hesaplar.find(
                (hesap: Hesap) =>
                  hesap.id ===
                  this.yeniOlusturulanHesapId
              );

            if (yeniHesap) {

              this.seciliHesapTipi =
                yeniHesap.hesapTipi;

              this.hesapSec(
                yeniHesap
              );

            } else {

              this.ilkSecimiYap();
            }

            this.yeniOlusturulanHesapId =
              null;

          } else {

            this.ilkSecimiYap();
          }

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
          this.hesapHareketleri = [];

          this.yeniOlusturulanHesapId =
            null;

          this.yukleniyorMu = false;

          this.hataMesaji =
            extractErrorMessage(
              hata,
              'Hesaplar getirilirken bir hata oluştu.'
            );

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

    this.tumHareketlerPopupAcikMi =
      false;

    this.hareketDetayiniKapat();

    this.hesapAdiDuzenlemesiniSifirla();

    this.hesapDurumuHataMesaji = '';

    this.hesapHareketleriniGetir(
      ilkHesap.id
    );
  }


  // YENİ HESAP FORMU

  hesapFormunuAc(): void {

    if (this.musteriId <= 0) {
      return;
    }

    this.hesapFormuAcikMi = true;
  }


  hesapFormunuKapat(): void {

    this.hesapFormuAcikMi = false;
  }


  hesapOlusturuldu(
    eklenenHesap: Hesap
  ): void {

    /*
     * Başarı ekranı popup içerisinde açık kalır.
     * Arkadaki hesap listesi yenilenir.
     */
    this.yeniOlusturulanHesapId =
      eklenenHesap.id;

    this.hesaplariGetir();
  }


  get seciliTiptekiHesaplar():
    Hesap[] {

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

    this.tumHareketlerPopupAcikMi =
      false;

    this.hareketDetayiniKapat();

    this.hesapAdiDuzenlemesiniSifirla();

    this.hesapDurumuHataMesaji = '';

    if (this.seciliHesap !== null) {

      this.hesapHareketleriniGetir(
        this.seciliHesap.id
      );

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

    this.tumHareketlerPopupAcikMi =
      false;

    this.hareketDetayiniKapat();

    this.hesapAdiDuzenlemesiniSifirla();

    this.hesapDurumuHataMesaji = '';

    this.hesapHareketleriniGetir(
      hesap.id
    );
  }


  hesapDetaylariniAcKapat(): void {

    this.detayAcikMi =
      !this.detayAcikMi;
  }


  ibanKopyala(): void {

    if (this.seciliHesap === null) {
      return;
    }

    navigator.clipboard
      .writeText(this.seciliHesap.iban)
      .then(() => {

        this.ibanKopyalandiMi = true;

        setTimeout(() => {
          this.ibanKopyalandiMi = false;
        }, 2000);
      });
  }


  // HESAP ADI DÜZENLEME

  hesapAdiniDuzenlemeyeBasla(): void {

    if (this.seciliHesap === null) {
      return;
    }

    this.duzenlenenHesapAdi =
      this.seciliHesap.hesapAdi;

    this.hesapAdiHataMesaji = '';

    this.hesapAdiDuzenleniyorMu =
      true;
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

    if (
      yeniHesapAdi ===
      this.seciliHesap.hesapAdi
    ) {

      this.hesapAdiDuzenlemesiniSifirla();

      return;
    }

    const guncellenecekHesap:
      Hesap = {
      ...this.seciliHesap,
      hesapAdi: yeniHesapAdi
    };

    this.hesapAdiKaydediliyorMu =
      true;

    this.hesapAdiHataMesaji = '';

    this.hesapApi
      .guncelle(
        guncellenecekHesap.id,
        guncellenecekHesap
      )
      .subscribe({

        next: () => {

          if (
            this.seciliHesap !== null
          ) {
            this.seciliHesap.hesapAdi =
              yeniHesapAdi;
          }

          const listedekiHesap =
            this.hesaplar.find(
              (hesap: Hesap) =>
                hesap.id ===
                guncellenecekHesap.id
            );

          if (listedekiHesap) {
            listedekiHesap.hesapAdi =
              yeniHesapAdi;
          }

          this.hesapAdiKaydediliyorMu =
            false;

          this.hesapAdiDuzenleniyorMu =
            false;

          this.changeDetector
            .markForCheck();
        },

        error: (hata) => {

          console.error(
            'Hesap adı güncelleme hatası:',
            hata
          );

          this.hesapAdiKaydediliyorMu =
            false;

          this.hesapAdiHataMesaji =
            extractErrorMessage(
              hata,
              'Hesap adı güncellenirken bir hata oluştu.'
            );

          this.changeDetector
            .markForCheck();
        }

      });
  }


  private hesapAdiDuzenlemesiniSifirla():
    void {

    this.hesapAdiDuzenleniyorMu =
      false;

    this.hesapAdiKaydediliyorMu =
      false;

    this.duzenlenenHesapAdi = '';

    this.hesapAdiHataMesaji = '';
  }


  // HESAP DURUMU DÜZENLEME

  hesapDurumunuGuncelle(
    event: Event
  ): void {

    if (this.seciliHesap === null) {
      return;
    }

    const yeniDurumKodu = Number(
      (event.target as HTMLSelectElement)
        .value
    );

    if (
      yeniDurumKodu ===
      this.seciliHesap.hesapDurumKodu
    ) {
      return;
    }

    const guncellenecekHesap:
      Hesap = {
      ...this.seciliHesap,
      hesapDurumKodu: yeniDurumKodu
    };

    this.hesapDurumuKaydediliyorMu =
      true;

    this.hesapDurumuHataMesaji = '';

    this.hesapApi
      .guncelle(
        guncellenecekHesap.id,
        guncellenecekHesap
      )
      .subscribe({

        next: () => {

          if (
            this.seciliHesap !== null
          ) {
            this.seciliHesap.hesapDurumKodu =
              yeniDurumKodu;
          }

          const listedekiHesap =
            this.hesaplar.find(
              (hesap: Hesap) =>
                hesap.id ===
                guncellenecekHesap.id
            );

          if (listedekiHesap) {
            listedekiHesap.hesapDurumKodu =
              yeniDurumKodu;
          }

          this.hesapDurumuKaydediliyorMu =
            false;

          this.changeDetector
            .markForCheck();
        },

        error: (hata) => {

          console.error(
            'Hesap durumu güncelleme hatası:',
            hata
          );

          this.hesapDurumuKaydediliyorMu =
            false;

          this.hesapDurumuHataMesaji =
            extractErrorMessage(
              hata,
              'Hesap durumu güncellenirken bir hata oluştu.'
            );

          this.changeDetector
            .markForCheck();
        }

      });
  }


  // HESAP HAREKETLERİ

  private hesapHareketleriniGetir(
    hesapId: number
  ): void {

    this.hareketlerHataMesaji = '';

    this.hesapHareketiApi
      .listele(hesapId)
      .subscribe({

        next: (
          gelenHareketler:
            HesapHareketi[]
        ) => {

          this.hesapHareketleri =
            gelenHareketler;

          this.hareketlerMevcutSayfa =
            1;

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
            extractErrorMessage(
              hata,
              'Hesap hareketleri getirilirken bir hata oluştu.'
            );

          this.changeDetector
            .markForCheck();
        }

      });
  }


  private get siraliHesapHareketleri():
    HesapHareketi[] {

    return [
      ...this.hesapHareketleri
    ].sort(
      (
        a: HesapHareketi,
        b: HesapHareketi
      ) =>
        new Date(
          b.islemTarihi
        ).getTime() -
        new Date(
          a.islemTarihi
        ).getTime()
    );
  }


  get sonBesHesapHareketi():
    HesapHareketi[] {

    return this.siraliHesapHareketleri
      .slice(0, 5);
  }


  get sayfadakiHesapHareketleri():
    HesapHareketi[] {

    const baslangicIndeksi =
      (
        this.hareketlerMevcutSayfa -
        1
      ) *
      this.hareketlerSayfaBasinaKayit;

    return this.siraliHesapHareketleri
      .slice(
        baslangicIndeksi,
        baslangicIndeksi +
        this.hareketlerSayfaBasinaKayit
      );
  }


  get hareketlerToplamSayfa():
    number {

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

    this.tumHareketlerPopupAcikMi =
      true;
  }


  tumHareketleriKapat(): void {

    this.tumHareketlerPopupAcikMi =
      false;
  }


  hareketlerOncekiSayfa(): void {

    if (
      this.hareketlerMevcutSayfa > 1
    ) {
      this.hareketlerMevcutSayfa--;
    }
  }


  hareketlerSonrakiSayfa(): void {

    if (
      this.hareketlerMevcutSayfa <
      this.hareketlerToplamSayfa
    ) {
      this.hareketlerMevcutSayfa++;
    }
  }


  // HAREKET DETAYI

  hareketDetayiniAc(
    hareket: HesapHareketi
  ): void {

    this.seciliHareket = hareket;
    this.hareketDetayAcikMi = true;

    this.transferDetayi = null;
    this.transferDetayiHataMesaji = '';

    if (
      !this.hareketTransferMi(
        hareket.hesapHareketiTipi
      ) ||
      hareket.paraTransferiId === null
    ) {
      return;
    }

    this.transferDetayiYukleniyorMu = true;

    this.paraTransferApi
      .transferDetayiGetir(
        hareket.paraTransferiId
      )
      .subscribe({

        next: (
          detay: ParaTransferiDetay
        ) => {

          this.transferDetayi = detay;

          this.transferDetayiYukleniyorMu =
            false;

          this.changeDetector
            .markForCheck();
        },

        error: (hata) => {

          console.error(
            'Transfer detayı getirme hatası:',
            hata
          );

          this.transferDetayiYukleniyorMu =
            false;

          this.transferDetayiHataMesaji =
            extractErrorMessage(
              hata,
              'Transfer detayı getirilirken bir hata oluştu.'
            );

          this.changeDetector
            .markForCheck();
        }

      });
  }


  hareketDetayiniKapat(): void {

    this.hareketDetayAcikMi = false;
    this.seciliHareket = null;

    this.transferDetayi = null;
    this.transferDetayiYukleniyorMu = false;
    this.transferDetayiHataMesaji = '';
  }


  hareketTransferMi(
    tip: HesapHareketTipleri
  ): boolean {

    return (
      tip ===
      HesapHareketTipleri.GelenTransfer ||
      tip ===
      HesapHareketTipleri.GidenTransfer
    );
  }


  kanalHavaleEftMi(): boolean {

    return (
      this.transferDetayi?.transferKanali ===
      TransferKanallari.HavaleEft
    );
  }


  kanalSwiftMi(): boolean {

    return (
      this.transferDetayi?.transferKanali ===
      TransferKanallari.Swift
    );
  }


  kanalVirmanMi(): boolean {

    return (
      this.transferDetayi?.transferKanali ===
      TransferKanallari.Virman
    );
  }


  gonderimTuruAdiniGetir(): string {

    if (this.kanalVirmanMi()) {
      return 'Virman';
    }

    if (this.kanalSwiftMi()) {
      return 'SWIFT';
    }

    if (this.kanalHavaleEftMi()) {
      return 'Havale/EFT';
    }

    return '-';
  }


  farkliDovizMi(): boolean {

    return (
      this.transferDetayi !== null &&
      this.transferDetayi.gonderenDovizCinsi !==
      this.transferDetayi.aliciDovizCinsi
    );
  }


  hareketGirisMi(
    tip: HesapHareketTipleri
  ): boolean {

    return (
      tip ===
      HesapHareketTipleri.ParaYatirma ||
      tip ===
      HesapHareketTipleri.GelenTransfer
    );
  }


  hareketAciklamasiniGetir(
    tip: HesapHareketTipleri
  ): string {

    switch (tip) {

      case HesapHareketTipleri
        .ParaYatirma:

        return 'Para Yatırma';

      case HesapHareketTipleri
        .ParaCekme:

        return 'Para Çekme';

      case HesapHareketTipleri
        .GelenTransfer:

        return 'Gelen Transfer';

      case HesapHareketTipleri
        .GidenTransfer:

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
        (
          secenek:
            HesapTipiSecenegi
        ) =>
          secenek.tip === hesapTipi
      )?.ad ?? 'Hesap'
    );
  }


  hesapDurumuAdiniGetir(
    durumKodu: number
  ): string {

    switch (durumKodu) {

      case HesapDurumu.Aktif:
        return 'Aktif';

      case HesapDurumu.Pasif:
        return 'Pasif';

      case HesapDurumu.Kapali:
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
      [
        '/para-cek-yatir'
      ],
      {
        queryParams: {
          iban:
            this.seciliHesap.iban
        }
      }
    );
  }


  havaleEftEkraninaGit(): void {

    if (this.seciliHesap === null) {
      return;
    }

    this.router.navigate(
      [
        '/havale-eft'
      ],
      {
        queryParams: {
          gonderenIban:
            this.seciliHesap.iban
        }
      }
    );
  }


  swiftEkraninaGit(): void {

    if (this.seciliHesap === null) {
      return;
    }

    this.router.navigate(
      [
        '/swift'
      ],
      {
        queryParams: {
          gonderenIban:
            this.seciliHesap.iban
        }
      }
    );
  }


  virmanEkraninaGit(): void {

    if (this.seciliHesap === null) {
      return;
    }

    const queryParams:
      Record<string, string> = {
      gonderenIban:
        this.seciliHesap.iban
    };

    if (this.musteri?.tckn) {
      queryParams['tc'] =
        this.musteri.tckn;
    }

    this.router.navigate(
      [
        '/virman'
      ],
      {
        queryParams
      }
    );
  }
}
