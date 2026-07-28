import {
  Component,
  OnInit
} from '@angular/core';

import { Sube } from '../models/sube-model';
import { SubeApi } from '../services/sube-api';

@Component({
  selector: 'app-sube-listesi',
  standalone: false,
  templateUrl: './sube-listesi.html',
  styleUrl: './sube-listesi.css'
})
export class SubeListesi implements OnInit {

  subeler: Sube[] = [];

  subeFormuAcikMi: boolean = false;
  yukleniyorMu: boolean = false;
  hataMesaji: string = '';

  constructor(private subeApi: SubeApi) {
  }

  ngOnInit(): void {
    this.subeleriGetir();
  }

  subeleriGetir(): void {
    this.yukleniyorMu = true;
    this.hataMesaji = '';

    this.subeApi.listele().subscribe({

      next: (gelenSubeler: Sube[]) => {
        console.log(
          'Backend’den gelen şubeler:',
          gelenSubeler
        );

        this.subeler = gelenSubeler;
        this.yukleniyorMu = false;
      },

      error: (hata) => {
        console.error(
          'Şube listeleme hatası:',
          hata
        );

        this.hataMesaji =
          'Şubeler getirilirken bir hata oluştu.';

        this.yukleniyorMu = false;
      }

    });
  }

  subeFormunuAc(): void {
    this.subeFormuAcikMi = true;
  }

  subeFormunuKapat(): void {
    this.subeFormuAcikMi = false;
  }

  duzenle(id: number): void {
    console.log(
      'Düzenlenecek şube ID:',
      id
    );
  }

  sil(id: number): void {
    console.log(
      'Silinecek şube ID:',
      id
    );
  }
}
