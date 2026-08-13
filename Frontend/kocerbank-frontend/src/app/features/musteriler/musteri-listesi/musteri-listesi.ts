import {
  ChangeDetectorRef,
  Component
} from '@angular/core';

import {
  Router
} from '@angular/router';

import {
  Musteri
} from '../models/musteri-model';

import {
  MusteriFiltre
} from '../models/musteri-filtre-model';

import {
  MusteriApi
} from '../services/musteri-api';

import {
  MusteriListeDurumu
} from '../services/musteri-liste-durumu';

import {
  ilkKayitNumarasi,
  sayfadakiKayitlar,
  sonKayitNumarasi,
  toplamSayfaSayisi
} from '../../../shared/utils/sayfalama';

@Component({
  selector: 'app-musteri-listesi',
  standalone: false,
  templateUrl: './musteri-listesi.html',
  styleUrl: './musteri-listesi.css'
})
export class MusteriListesi {

  // Aşağıdaki dört alan, hesaplar ekranına gidip
  // (özellikle tarayıcı geri tuşuyla) geri dönüldüğünde
  // kaybolmamaları için MusteriListeDurumu servisinde
  // tutulur, bu alanlar yalnızca ona vekillik eder.

  get musteriler(): Musteri[] {
    return this.durum.musteriler;
  }
  set musteriler(deger: Musteri[]) {
    this.durum.musteriler = deger;
  }

  get aramaKriterleri(): MusteriFiltre {
    return this.durum.aramaKriterleri;
  }
  set aramaKriterleri(deger: MusteriFiltre) {
    this.durum.aramaKriterleri = deger;
  }

  get mevcutSayfa(): number {
    return this.durum.mevcutSayfa;
  }
  set mevcutSayfa(deger: number) {
    this.durum.mevcutSayfa = deger;
  }

  get bilgiMesaji(): string {
    return this.durum.bilgiMesaji;
  }
  set bilgiMesaji(deger: string) {
    this.durum.bilgiMesaji = deger;
  }

  sayfaBasinaKayit: number = 10;

  musteriFormuAcikMi: boolean = false;
  iletisimPopupAcikMi: boolean = false;

  seciliMusteri: Musteri | null = null;

  yukleniyorMu: boolean = false;
  hataMesaji: string = '';


  constructor(
    private musteriApi: MusteriApi,
    private router: Router,
    private changeDetector: ChangeDetectorRef,
    private durum: MusteriListeDurumu
  ) {
  }


  // ARAMA İŞLEMLERİ

  ara(): void {

    // Diğer filtreler yalnızca bu butona
    // basıldığında sorguya gönderilir.
    this.musterileriGetir();

  }


  musteriTipiDegisti(): void {

    const musteriTipi =
      this.aramaKriterleri.musteriTipi;

    if (
      musteriTipi === undefined ||
      musteriTipi === null
    ) {

      // Müşteri tipi kaldırılırsa
      // bütün müşteri filtrelerini temizler.
      this.aramaKriterleri = {};
      this.musteriler = [];

      this.mevcutSayfa = 1;
      this.yukleniyorMu = false;
      this.hataMesaji = '';

      this.bilgiMesaji =
        'Filtreleme yapmak için önce müşteri tipini seçiniz.';

      this.changeDetector.markForCheck();

      return;
    }

    if (musteriTipi === 1) {

      // Bireysel müşteri seçildiyse
      // kurumsal alanları temizler.
      delete this.aramaKriterleri.vkn;
      delete this.aramaKriterleri.unvan;

    }

    if (musteriTipi === 2) {

      // Kurumsal müşteri seçildiyse
      // bireysel alanları temizler.
      delete this.aramaKriterleri.tckn;
      delete this.aramaKriterleri.cinsiyet;
      delete this.aramaKriterleri.dogumTarihiBaslangic;
      delete this.aramaKriterleri.dogumTarihiBitis;

    }

    // Müşteri tipi değiştiğinde otomatik sorgu göndermez.
    this.bilgiMesaji =
      'Filtreleri belirledikten sonra Ara butonuna basınız.';

    this.changeDetector.markForCheck();

  }


  // SAYFALAMA

  get sayfadakiMusteriler(): Musteri[] {

    return sayfadakiKayitlar(
      this.musteriler,
      this.mevcutSayfa,
      this.sayfaBasinaKayit
    );
  }


  get toplamSayfa(): number {

    return toplamSayfaSayisi(
      this.musteriler.length,
      this.sayfaBasinaKayit
    );
  }


  get ilkKayitNumarasi(): number {

    return ilkKayitNumarasi(
      this.musteriler.length,
      this.mevcutSayfa,
      this.sayfaBasinaKayit
    );
  }


  get sonKayitNumarasi(): number {

    return sonKayitNumarasi(
      this.musteriler.length,
      this.mevcutSayfa,
      this.sayfaBasinaKayit
    );
  }


  oncekiSayfa(): void {

    if (this.mevcutSayfa > 1) {
      this.mevcutSayfa--;
    }

  }


  sonrakiSayfa(): void {

    if (
      this.mevcutSayfa <
      this.toplamSayfa
    ) {
      this.mevcutSayfa++;
    }

  }


  // MÜŞTERİ LİSTELEME

  musterileriGetir(
    kriterler: MusteriFiltre =
      this.aramaKriterleri
  ): void {

    if (!this.musteriTipiSecildiMi(kriterler)) {

      this.musteriler = [];
      this.mevcutSayfa = 1;

      this.yukleniyorMu = false;
      this.hataMesaji = '';

      this.bilgiMesaji =
        'Filtreleme yapmak için önce müşteri tipini seçiniz.';

      this.changeDetector.markForCheck();

      return;
    }

    this.yukleniyorMu = true;
    this.hataMesaji = '';
    this.bilgiMesaji = '';

    this.musteriApi
      .listele(kriterler)
      .subscribe({

        next: (
          gelenMusteriler: Musteri[]
        ) => {

          this.musteriler =
            gelenMusteriler;

          this.mevcutSayfa = 1;
          this.yukleniyorMu = false;

          this.changeDetector.markForCheck();

        },

        error: (hata) => {

          console.error(
            'Müşteri listeleme hatası:',
            hata
          );

          this.musteriler = [];
          this.mevcutSayfa = 1;

          this.hataMesaji =
            'Müşteriler getirilirken bir hata oluştu.';

          this.yukleniyorMu = false;

          this.changeDetector.markForCheck();

        }

      });
  }


  private musteriTipiSecildiMi(
    kriterler: MusteriFiltre
  ): boolean {

    return (
      kriterler.musteriTipi === 1 ||
      kriterler.musteriTipi === 2
    );
  }


  filtreleriTemizle(): void {

    this.aramaKriterleri = {};
    this.musteriler = [];

    this.mevcutSayfa = 1;
    this.yukleniyorMu = false;
    this.hataMesaji = '';

    this.bilgiMesaji =
      'Filtreleme yapmak için önce müşteri tipini seçiniz.';

    this.changeDetector.markForCheck();

  }


  // MÜŞTERİ EKLEME VE DÜZENLEME

  musteriEklemeFormunuAc(): void {

    this.seciliMusteri = null;
    this.musteriFormuAcikMi = true;

  }


  duzenle(
    musteri: Musteri
  ): void {

    this.seciliMusteri = {
      ...musteri
    };

    this.musteriFormuAcikMi = true;

  }


  musteriFormunuKapat(): void {

    this.musteriFormuAcikMi = false;
    this.seciliMusteri = null;

  }


  musteriKaydedildi(): void {

    this.musteriFormunuKapat();
    this.musterileriGetir();

  }


  // İLETİŞİM POPUP'I

  iletisimBilgileriniAc(
    musteri: Musteri
  ): void {

    this.seciliMusteri = {
      ...musteri
    };

    this.iletisimPopupAcikMi = true;

  }


  iletisimPopupKapat(): void {

    this.iletisimPopupAcikMi = false;
    this.seciliMusteri = null;

  }


  // HESAP BİLGİLERİ SAYFASI

  hesapBilgilerineGit(
    musteri: Musteri
  ): void {

    this.router.navigate([
      '/musteriler',
      musteri.id,
      'hesaplar'
    ]);

  }

  iletisimBilgileriGuncellendi(): void {
    this.musterileriGetir();
  }
}
