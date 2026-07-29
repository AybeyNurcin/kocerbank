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

  aramaKriterleri: SubeFiltre = {};

  subeFormuAcikMi: boolean = false;
  seciliSube: Sube | null = null;

  yukleniyorMu: boolean = false;
  hataMesaji: string = '';

  private filtreDegisikligi =
    new Subject<void>();

  private filtreAboneligi?: Subscription;

  constructor(
    private subeApi: SubeApi,
    private changeDetector: ChangeDetectorRef
  ) {
  }

  ngOnInit(): void {

    // Sayfa ilk açıldığında bütün şubeleri getirir.
    this.subeleriGetir();

    // Kullanıcı yazmayı bıraktıktan 400 ms sonra filtreler.
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

  subeleriGetir(
    kriterler: SubeFiltre =
      this.aramaKriterleri
  ): void {

    this.yukleniyorMu = true;
    this.hataMesaji = '';

    this.subeApi
      .listele(kriterler)
      .subscribe({

        next: (gelenSubeler: Sube[]) => {
          this.subeler = gelenSubeler;
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

  filtreleriTemizle(): void {

    this.aramaKriterleri = {};

    // Temizleme işleminde 400 ms beklemiyoruz.
    this.subeleriGetir();

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
    this.subeleriGetir();

  }
}
