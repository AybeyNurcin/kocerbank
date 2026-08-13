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

import { Personel } from '../models/personel-model';
import { PersonelFiltre } from '../models/personel-filtre-model';
import { PersonelApi } from '../services/personel-api';

import {
  ilkKayitNumarasi,
  sayfadakiKayitlar,
  sonKayitNumarasi,
  toplamSayfaSayisi
} from '../../../shared/utils/sayfalama';

@Component({
  selector: 'app-personel-listesi',
  standalone: false,
  templateUrl: './personel-listesi.html',
  styleUrl: './personel-listesi.css'
})
export class PersonelListesi
  implements OnInit, OnDestroy {

  personeller: Personel[] = [];

  mevcutSayfa: number = 1;
  sayfaBasinaKayit: number = 10;

  aramaKriterleri: PersonelFiltre = {};

  personelFormuAcikMi: boolean = false;
  seciliPersonel: Personel | null = null;

  yukleniyorMu: boolean = false;
  hataMesaji: string = '';

  bilgiMesaji: string =
    'Lütfen en az bir filtreleme kriteri giriniz.';

  private filtreDegisikligi =
    new Subject<void>();

  private filtreAboneligi?: Subscription;

  constructor(
    private personelApi: PersonelApi,
    private changeDetector: ChangeDetectorRef
  ) {
  }

  ngOnInit(): void {

    // Kullanıcı adres alanında yazmayı bıraktıktan
    // 400 ms sonra filtreleme yapar.
    this.filtreAboneligi =
      this.filtreDegisikligi
        .pipe(
          debounceTime(400)
        )
        .subscribe(() => {
          this.personelleriGetir();
        });

  }

  ngOnDestroy(): void {

    this.filtreAboneligi?.unsubscribe();

  }

  adresDegisti(): void {

    // Yalnızca adres alanındaki değişiklikler
    // 400 ms gecikmeli otomatik arama başlatır.
    this.filtreDegisikligi.next();

  }

  ara(): void {

    // Kullanıcının o anda doldurduğu
    // bütün filtrelerle sorgu gönderir.
    this.personelleriGetir();

  }

  get sayfadakiPersoneller(): Personel[] {

    return sayfadakiKayitlar(
      this.personeller,
      this.mevcutSayfa,
      this.sayfaBasinaKayit
    );
  }

  get toplamSayfa(): number {

    return toplamSayfaSayisi(
      this.personeller.length,
      this.sayfaBasinaKayit
    );
  }

  get ilkKayitNumarasi(): number {

    return ilkKayitNumarasi(
      this.personeller.length,
      this.mevcutSayfa,
      this.sayfaBasinaKayit
    );
  }

  get sonKayitNumarasi(): number {

    return sonKayitNumarasi(
      this.personeller.length,
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

  personelleriGetir(
    kriterler: PersonelFiltre =
      this.aramaKriterleri
  ): void {

    // Herhangi bir filtre girilmiş mi kontrol eder.
    if (!this.filtreVarMi(kriterler)) {

      // Filtrelerin tamamı boşsa eski listeyi temizler.
      this.personeller = [];
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

    this.personelApi
      .listele(kriterler)
      .subscribe({

        next: (gelenPersoneller: Personel[]) => {

          this.personeller = gelenPersoneller;

          // Her yeni filtre sonucunda ilk sayfa açılır.
          this.mevcutSayfa = 1;

          this.yukleniyorMu = false;

          this.changeDetector.markForCheck();

        },

        error: (hata) => {

          console.error(
            'Personel listeleme hatası:',
            hata
          );

          this.hataMesaji =
            'Personeller getirilirken bir hata oluştu.';

          this.yukleniyorMu = false;

          this.changeDetector.markForCheck();

        }

      });
  }

  private filtreVarMi(
    kriterler: PersonelFiltre
  ): boolean {

    const idGirildiMi =
      kriterler.id !== undefined &&
      kriterler.id !== null;

    const sicilGirildiMi =
      (kriterler.sicil?.trim().length ?? 0) > 0;

    const adGirildiMi =
      (kriterler.ad?.trim().length ?? 0) > 0;

    const soyadGirildiMi =
      (kriterler.soyad?.trim().length ?? 0) > 0;

    const rolGirildiMi =
      (kriterler.rol?.trim().length ?? 0) > 0;

    const tcknGirildiMi =
      (kriterler.tckn?.trim().length ?? 0) > 0;

    const subeGirildiMi =
      (kriterler.subeKodu?.trim().length ?? 0) > 0;

    const adresGirildiMi =
      (
        kriterler.adres
          ?.trim()
          .length ?? 0
      ) > 0;

    const durumSecildiMi =
      kriterler.durumKodu !== undefined &&
      kriterler.durumKodu !== null;

    const tarihAraligiGirildiMi =
      (
        kriterler.baslangicTarihi
          ?.trim()
          .length ?? 0
      ) > 0 ||
      (
        kriterler.bitisTarihi
          ?.trim()
          .length ?? 0
      ) > 0;

    return (
      idGirildiMi ||
      sicilGirildiMi ||
      adGirildiMi ||
      soyadGirildiMi ||
      rolGirildiMi ||
      tcknGirildiMi ||
      subeGirildiMi ||
      adresGirildiMi ||
      durumSecildiMi ||
      tarihAraligiGirildiMi
    );
  }

  filtreleriTemizle(): void {

    this.aramaKriterleri = {};
    this.personeller = [];
    this.mevcutSayfa = 1;

    this.hataMesaji = '';

    this.bilgiMesaji =
      'Lütfen en az bir filtreleme kriteri giriniz.';

    this.changeDetector.markForCheck();

  }

  personelEklemeFormunuAc(): void {

    this.seciliPersonel = null;
    this.personelFormuAcikMi = true;

  }

  duzenle(personel: Personel): void {

    this.seciliPersonel = {
      ...personel
    };

    this.personelFormuAcikMi = true;

  }

  personelFormunuKapat(): void {

    this.personelFormuAcikMi = false;
    this.seciliPersonel = null;

  }

  personelKaydedildi(): void {

    this.personelFormunuKapat();

    // Filtre varsa mevcut listeyi yeniler.
    // Filtre yoksa backend'e istek göndermez.
    this.personelleriGetir();

  }
}
