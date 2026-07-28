import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import { Sube } from '../models/sube-model';
import { SubeFiltre } from '../models/sube-filtre-model';
import { SubeApi } from '../services/sube-api';

@Component({
  selector: 'app-sube-listesi',
  standalone: false,
  templateUrl: './sube-listesi.html',
  styleUrl: './sube-listesi.css'
})
export class SubeListesi implements OnInit {

  subeler: Sube[] = [];

  aramaKriterleri: SubeFiltre = {};

  subeFormuAcikMi: boolean = false;
  seciliSube: Sube | null = null;

  yukleniyorMu: boolean = false;
  hataMesaji: string = '';

  constructor(
    private subeApi: SubeApi,
    private changeDetector: ChangeDetectorRef
  ) {
  }

  ngOnInit(): void {
    this.subeleriGetir();
  }

  subeleriGetir(
    kriterler: SubeFiltre = this.aramaKriterleri
  ): void {

    this.yukleniyorMu = true;
    this.hataMesaji = '';

    this.subeApi
      .listele(kriterler)
      .subscribe({

        next: (gelenSubeler: Sube[]) => {
          console.log(
            'Backend’den gelen şubeler:',
            gelenSubeler
          );

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

  ara(): void {
    this.subeleriGetir(
      this.aramaKriterleri
    );
  }

  filtreleriTemizle(): void {
    this.aramaKriterleri = {};

    this.subeleriGetir(
      this.aramaKriterleri
    );
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
