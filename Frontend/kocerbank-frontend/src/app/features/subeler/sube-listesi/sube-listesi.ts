import {
  ChangeDetectorRef,
  Component,
  OnDestroy,
  OnInit
} from '@angular/core';

import {
  Subject,
  Subscription,
  debounceTime
} from 'rxjs';

import { Sube } from '../models/sube-model';
import { SubeFiltre } from '../models/sube-filtre-model';
import { SubeApi } from '../services/sube-api';

import {
  ilkKayitNumarasi,
  sayfadakiKayitlar,
  sonKayitNumarasi,
  toplamSayfaSayisi
} from '../../../shared/utils/sayfalama';

@Component({
  selector: 'app-sube-listesi',
  standalone: false,
  templateUrl: './sube-listesi.html',
  styleUrl: './sube-listesi.css'
})
export class SubeListesi
  implements OnInit, OnDestroy {

  subeler: Sube[] = [];

  mevcutSayfa: number = 1;
  sayfaBasinaKayit: number = 10;

  aramaKriterleri: SubeFiltre = {};

  subeFormuAcikMi: boolean = false;
  seciliSube: Sube | null = null;

  yukleniyorMu: boolean = false;
  hataMesaji: string = '';

  bilgiMesaji: string =
    'Lütfen en az bir filtreleme kriteri giriniz.';

  private filtreDegisikligi =
    new Subject<void>();

  private filtreAboneligi?: Subscription;

  constructor(
    private subeApi: SubeApi,
    private changeDetector: ChangeDetectorRef
  ) {
  }

  ngOnInit(): void {

    // Kullanıcı adres veya telefon alanında
    // 400 ms sonra filtreleme yapar.
    this.filtreAboneligi =
      this.filtreDegisikligi
        .pipe(
          debounceTime(400)
        )
        .subscribe(() => {
          this.subeleriGetir();
        });

  }

  ngOnDestroy(): void {

    this.filtreAboneligi?.unsubscribe();

  }

  otomatikFiltreDegisti(): void {

    // Adres veya telefon alanındaki değişikliklerden
    // 400 ms sonra otomatik arama başlatır.
    this.filtreDegisikligi.next();

  }

  ara(): void {

    // Kullanıcının o anda doldurduğu
    // bütün filtrelerle sorgu gönderir.
    this.subeleriGetir();

  }

  get sayfadakiSubeler(): Sube[] {

    return sayfadakiKayitlar(
      this.subeler,
      this.mevcutSayfa,
      this.sayfaBasinaKayit
    );
  }

  get toplamSayfa(): number {

    return toplamSayfaSayisi(
      this.subeler.length,
      this.sayfaBasinaKayit
    );
  }

  get ilkKayitNumarasi(): number {

    return ilkKayitNumarasi(
      this.subeler.length,
      this.mevcutSayfa,
      this.sayfaBasinaKayit
    );
  }

  get sonKayitNumarasi(): number {

    return sonKayitNumarasi(
      this.subeler.length,
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

  subeleriGetir(
    kriterler: SubeFiltre =
      this.aramaKriterleri
  ): void {

    // Herhangi bir filtre girilmiş mi kontrol eder.
    if (!this.filtreVarMi(kriterler)) {

      // Filtrelerin tamamı boşsa eski listeyi temizler.
      this.subeler = [];
      this.mevcutSayfa = 1;

      this.yukleniyorMu = false;
      this.hataMesaji = '';

      this.bilgiMesaji =
        'Lütfen en az bir filtreleme kriteri giriniz.';

      this.changeDetector.markForCheck();

      // Backend'e istek göndermeden metottan çıkar.
      return;
    }

    this.yukleniyorMu = true;
    this.hataMesaji = '';
    this.bilgiMesaji = '';

    this.subeApi
      .listele(kriterler)
      .subscribe({

        next: (gelenSubeler: Sube[]) => {

          this.subeler = gelenSubeler;

          // Her yeni filtre sonucunda ilk sayfa açılır.
          this.mevcutSayfa = 1;

          this.yukleniyorMu = false;

          this.changeDetector.markForCheck();

        },

        error: (hata) => {

          console.error(
            'Şube listeleme hatası:',
            hata
          );

          this.hataMesaji =
            'Şubeler getirilirken bir hata oluştu.';

          this.yukleniyorMu = false;

          this.changeDetector.markForCheck();

        }

      });
  }

  private filtreVarMi(
    kriterler: SubeFiltre
  ): boolean {

    const idGirildiMi =
      kriterler.id !== undefined &&
      kriterler.id !== null;

    const subeAdiGirildiMi =
      (kriterler.subeAdi?.trim().length ?? 0) > 0;

    const subeKoduGirildiMi =
      (kriterler.subeKodu?.trim().length ?? 0) > 0;

    const adresGirildiMi =
      (
        kriterler.subeAdres
          ?.trim()
          .length ?? 0
      ) > 0;

    const durumSecildiMi =
      kriterler.subeDurumKodu !== undefined &&
      kriterler.subeDurumKodu !== null;

    const tarihAraligiGirildiMi =
      (
        kriterler.acilisTarihiBaslangic
          ?.trim()
          .length ?? 0
      ) > 0 ||
      (
        kriterler.acilisTarihiBitis
          ?.trim()
          .length ?? 0
      ) > 0;

    return (
      idGirildiMi ||
      subeAdiGirildiMi ||
      subeKoduGirildiMi ||
      adresGirildiMi ||
      durumSecildiMi ||
      tarihAraligiGirildiMi
    );
  }

  filtreleriTemizle(): void {

    this.aramaKriterleri = {};
    this.subeler = [];
    this.mevcutSayfa = 1;

    this.hataMesaji = '';

    this.bilgiMesaji =
      'Lütfen en az bir filtreleme kriteri giriniz.';

    this.changeDetector.markForCheck();

  }

  subeEklemeFormunuAc(): void {

    this.seciliSube = null;
    this.subeFormuAcikMi = true;

  }

  duzenle(sube: Sube): void {

    this.seciliSube = {
      ...sube
    };

    this.subeFormuAcikMi = true;

  }

  subeFormunuKapat(): void {

    this.subeFormuAcikMi = false;
    this.seciliSube = null;

  }

  subeKaydedildi(): void {

    this.subeFormunuKapat();

    // Filtre varsa mevcut listeyi yeniler.
    // Filtre yoksa backend'e istek göndermez.
    this.subeleriGetir();

  }
}
