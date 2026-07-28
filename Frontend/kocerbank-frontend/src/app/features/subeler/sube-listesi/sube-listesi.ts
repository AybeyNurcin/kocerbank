import { Component } from '@angular/core';
import { Sube } from '../models/sube-model';
import { AktifPasifDurumlari } from '../../../shared/enums/aktif-pasif-durumlari-enum';

@Component({
  selector: 'app-sube-listesi',
  standalone: false,
  templateUrl: './sube-listesi.html',
  styleUrl: './sube-listesi.css'
})
export class SubeListesi {

  subeFormuAcikMi: boolean = false;

subeler: Sube[] = [
  {
    id: 1,
    subeKodu: 'S0001',
    subeAdi: 'Ümraniye Şubesi',
    subeTelefonNo: '02165236841',
    subeAdres: 'Ümraniye, İstanbul',
    subeDurumKodu: AktifPasifDurumlari.Aktif,
    recordUser: 'KB0001',
    recordDate: '2026-07-28T10:00:00'
  },
  {
    id: 2,
    subeKodu: 'S0002',
    subeAdi: 'Esenler Şubesi',
    subeTelefonNo: '02125234124',
    subeAdres: 'Esenler, İstanbul',
    subeDurumKodu: AktifPasifDurumlari.Pasif,
    recordUser: 'KB0001',
    recordDate: '2026-07-28T10:00:00'
  }
];

  subeFormunuAc(): void {
    this.subeFormuAcikMi = true;
  }

  subeFormunuKapat(): void {
    this.subeFormuAcikMi = false;
  }

  duzenle(id: number): void {
    console.log('Düzenlenecek şube ID:', id);
  }

  sil(id: number): void {
    console.log('Silinecek şube ID:', id);
  }
}
