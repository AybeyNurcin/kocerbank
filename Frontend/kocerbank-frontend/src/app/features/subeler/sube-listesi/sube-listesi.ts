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

    // Kullanıcı yazmayı bıraktıktan
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

  filtreDegisti(): void {

    this.filtreDegisikligi.next();

  }

  get sayfadakiSubeler(): Sube[] {

    const baslangicIndeksi =
      (this.mevcutSayfa - 1) *
      this.sayfaBasinaKayit;

    const bitisIndeksi =
      baslangicIndeksi +
      this.sayfaBasinaKayit;

    return this.subeler.slice(
      baslangicIndeksi,
      bitisIndeksi
    );
  }

  get toplamSayfa(): number {

    return Math.ceil(
      this.subeler.length /
      this.sayfaBasinaKayit
    );
  }

  get ilkKayitNumarasi(): number {

    if (this.subeler.length === 0) {
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
      this.subeler.length
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

    const telefonGirildiMi =
      (
        kriterler.subeTelefonNo
          ?.trim()
          .length ?? 0
      ) > 0;

    const adresGirildiMi =
      (
        kriterler.subeAdres
          ?.trim()
          .length ?? 0
      ) > 0;

    const durumSecildiMi =
      kriterler.subeDurumKodu !== undefined &&
      kriterler.subeDurumKodu !== null;

    return (
      idGirildiMi ||
      subeAdiGirildiMi ||
      subeKoduGirildiMi ||
      telefonGirildiMi ||
      adresGirildiMi ||
      durumSecildiMi
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
