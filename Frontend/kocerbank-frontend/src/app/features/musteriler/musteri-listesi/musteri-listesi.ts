import {
  ChangeDetectorRef,
  Component,
  OnDestroy,
  OnInit
} from '@angular/core';

import {
  Router
} from '@angular/router';

import {
  Subject,
  Subscription,
  debounceTime
} from 'rxjs';

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
  Sube
} from '../../subeler/models/sube-model';

import {
  SubeApi
} from '../../subeler/services/sube-api';

@Component({
  selector: 'app-musteri-listesi',
  standalone: false,
  templateUrl: './musteri-listesi.html',
  styleUrl: './musteri-listesi.css'
})
export class MusteriListesi
  implements OnInit, OnDestroy {

  musteriler: Musteri[] = [];

  aramaKriterleri: MusteriFiltre = {};

  mevcutSayfa: number = 1;
  sayfaBasinaKayit: number = 10;

  musteriFormuAcikMi: boolean = false;
  iletisimPopupAcikMi: boolean = false;

  seciliMusteri: Musteri | null = null;

  yukleniyorMu: boolean = false;
  hataMesaji: string = '';

  bilgiMesaji: string =
    'Filtreleme yapmak için önce müşteri tipini seçiniz.';


  // ŞUBE AUTOCOMPLETE ALANLARI

  tumSubeler: Sube[] = [];

  subeAramaMetni: string = '';
  subeSecenekleriAcikMi: boolean = false;
  subelerYukleniyorMu: boolean = false;


  // TELEFON İÇİN OTOMATİK ARAMA

  private telefonDegisikligi =
    new Subject<void>();

  private telefonAboneligi?: Subscription;


  constructor(
    private musteriApi: MusteriApi,
    private subeApi: SubeApi,
    private router: Router,
    private changeDetector: ChangeDetectorRef
  ) {
  }


  ngOnInit(): void {

    // Yalnızca telefon numarası yazılırken
    // 400 ms sonra otomatik müşteri araması yapar.
    this.telefonAboneligi =
      this.telefonDegisikligi
        .pipe(
          debounceTime(400)
        )
        .subscribe(() => {
          this.telefonlaOtomatikAra();
        });

    // Şube autocomplete seçeneklerini getirir.
    this.subeSecenekleriniGetir();

  }


  ngOnDestroy(): void {

    this.telefonAboneligi?.unsubscribe();

  }


  // ARAMA İŞLEMLERİ

  telefonDegisti(): void {

    // Yalnızca telefon alanındaki değişiklik
    // gecikmeli sorgu gönderir.
    this.telefonDegisikligi.next();

  }

  private telefonlaOtomatikAra(): void {

    const telefonNo =
      this.aramaKriterleri.telefonNo?.trim();

    // Telefon alanı tamamen silindiyse
    // backend'e istek göndermez.
    if (
      telefonNo === undefined ||
      telefonNo === ''
    ) {

      this.musteriler = [];
      this.mevcutSayfa = 1;

      this.yukleniyorMu = false;
      this.hataMesaji = '';

      this.bilgiMesaji =
        'Filtreleri belirledikten sonra Ara butonuna basınız.';

      this.changeDetector.markForCheck();

      return;
    }

    // Telefon doluysa 400 ms sonunda
    // mevcut kriterlerle otomatik sorgu gönderir.
    this.musterileriGetir();

  }


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

      this.subeAramaMetni = '';
      this.subeSecenekleriAcikMi = false;

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
      delete this.aramaKriterleri.dogumTarihi;

    }

    // Müşteri tipi değiştiğinde otomatik sorgu göndermez.
    this.bilgiMesaji =
      'Filtreleri belirledikten sonra Ara butonuna basınız.';

    this.changeDetector.markForCheck();

  }


  // ŞUBE AUTOCOMPLETE

  private subeSecenekleriniGetir(): void {

    this.subelerYukleniyorMu = true;

    // Buradaki filtresiz listeleme müşteri tablosu
    // için değil, şube seçim seçenekleri içindir.
    this.subeApi
      .listele({})
      .subscribe({

        next: (gelenSubeler: Sube[]) => {

          this.tumSubeler = gelenSubeler;
          this.subelerYukleniyorMu = false;

          this.changeDetector.markForCheck();

        },

        error: (hata) => {

          console.error(
            'Şube seçenekleri getirilirken hata oluştu:',
            hata
          );

          this.tumSubeler = [];
          this.subelerYukleniyorMu = false;

          this.changeDetector.markForCheck();

        }

      });
  }


  get filtrelenmisSubeler(): Sube[] {

    const aranan =
      this.subeAramaMetni
        .trim()
        .toLocaleLowerCase('tr-TR');

    // Kullanıcı henüz bir şey yazmadıysa
    // ilk 10 şubeyi seçenek olarak gösterir.
    if (aranan === '') {

      return this.tumSubeler.slice(
        0,
        10
      );

    }

    // Yazılan metni hem şube kodunda
    // hem de şube adında arar.
    return this.tumSubeler
      .filter((sube: Sube) => {

        const subeKodu =
          sube.subeKodu
            .toLocaleLowerCase('tr-TR');

        const subeAdi =
          sube.subeAdi
            .toLocaleLowerCase('tr-TR');

        return (
          subeKodu.includes(aranan) ||
          subeAdi.includes(aranan)
        );

      })
      .slice(
        0,
        10
      );
  }


  subeAramasiDegisti(): void {

    this.subeSecenekleriAcikMi = true;

    // Kullanıcı seçilmiş şubenin üzerine
    // tekrar yazarsa eski seçimi temizler.
    const tamEslesenSube =
      this.tumSubeler.find(
        (sube: Sube) =>
          this.subeGorunumMetni(sube) ===
          this.subeAramaMetni
      );

    this.aramaKriterleri.subeKodu =
      tamEslesenSube?.subeKodu;

  }


  subeSecenekleriniAc(): void {

    this.subeSecenekleriAcikMi = true;

  }


  subeSecenekleriniKapat(): void {

    // Kullanıcının seçeneğe tıklayabilmesi için
    // çok kısa süre bekleyerek kapatır.
    setTimeout(() => {

      this.subeSecenekleriAcikMi = false;

      this.changeDetector.markForCheck();

    }, 150);

  }


  subeSec(
    sube: Sube
  ): void {

    // Backend müşteri filtresine
    // yalnızca seçilen şubenin kodu gönderilir.
    this.aramaKriterleri.subeKodu =
      sube.subeKodu;

    // Input içerisinde kod ve ad birlikte görünür.
    this.subeAramaMetni =
      this.subeGorunumMetni(sube);

    this.subeSecenekleriAcikMi = false;

  }


  subeGorunumMetni(
    sube: Sube
  ): string {

    return (
      sube.subeKodu +
      ' - ' +
      sube.subeAdi
    );

  }


  // SAYFALAMA

  get sayfadakiMusteriler(): Musteri[] {

    const baslangicIndeksi =
      (this.mevcutSayfa - 1) *
      this.sayfaBasinaKayit;

    const bitisIndeksi =
      baslangicIndeksi +
      this.sayfaBasinaKayit;

    return this.musteriler.slice(
      baslangicIndeksi,
      bitisIndeksi
    );
  }


  get toplamSayfa(): number {

    return Math.ceil(
      this.musteriler.length /
      this.sayfaBasinaKayit
    );
  }


  get ilkKayitNumarasi(): number {

    if (this.musteriler.length === 0) {
      return 0;
    }

    return (
      (this.mevcutSayfa - 1) *
      this.sayfaBasinaKayit
    ) + 1;
  }


  get sonKayitNumarasi(): number {

    return Math.min(
      this.mevcutSayfa *
      this.sayfaBasinaKayit,
      this.musteriler.length
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

    this.subeAramaMetni = '';
    this.subeSecenekleriAcikMi = false;

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
      'hesap-bilgileri'
    ]);

  }
}
