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

@Component({
  selector: 'app-personel-listesi',
  standalone: false,
  templateUrl: './personel-listesi.html',
  styleUrl: './personel-listesi.css',
})
export class PersonelListesi
  implements OnInit, OnDestroy {

  personeller: Personel[] = [];

  aramaKriterleri: PersonelFiltre = {};

  personelFormuAcikMi: boolean = false;
  seciliPersonel: Personel | null = null;

  yukleniyorMu: boolean = false;
  hataMesaji: string = '';

  private filtreDegisikligi =
    new Subject<void>();

  private filtreAboneligi?: Subscription;

  constructor(
    private personelApi: PersonelApi,
    private changeDetector: ChangeDetectorRef
  ) {
  }

  ngOnInit(): void {

    // Sayfa ilk açıldığında hiçbir filtre girilmediği için liste boş kalır.

    // Kullanıcı yazmayı bıraktıktan 400 ms sonra filtreler.
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

  filtreDegisti(): void {

    this.filtreDegisikligi.next();

  }

  private herhangiBirFiltreGirilmisMi(): boolean {

    const kriterler = this.aramaKriterleri;

    return Object.values(kriterler)
      .some(
        deger =>
          deger !== undefined &&
          deger !== null &&
          deger !== ''
      );

  }

  personelleriGetir(
    kriterler: PersonelFiltre =
      this.aramaKriterleri
  ): void {

    // Hiçbir filtre girilmemişse sonuç gösterilmez.
    if (!this.herhangiBirFiltreGirilmisMi()) {
      this.personeller = [];
      this.hataMesaji = '';
      this.yukleniyorMu = false;

      return;
    }

    this.yukleniyorMu = true;
    this.hataMesaji = '';

    this.personelApi
      .listele(kriterler)
      .subscribe({

        next: (gelenPersoneller: Personel[]) => {
          this.personeller = gelenPersoneller;
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

  filtreleriTemizle(): void {

    this.aramaKriterleri = {};

    // Filtre kalmadığı için liste de boşaltılır.
    this.personeller = [];
    this.hataMesaji = '';

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
    this.personelleriGetir();

  }
}
