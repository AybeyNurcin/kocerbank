import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges
} from '@angular/core';

import { Sube } from '../models/sube-model';
import { SubeApi } from '../services/sube-api';

import {
  AktifPasifDurumlari
} from '../../../shared/enums/aktif-pasif-durumlari-enum';

import {
  extractErrorMessage
} from '../../../shared/utils/hata-mesaji';

import {
  onbirHaneliRakamMi
} from '../../../shared/utils/format-kontrol';

@Component({
  selector: 'app-sube-formu',
  standalone: false,
  templateUrl: './sube-formu.html',
  styleUrl: './sube-formu.css'
})
export class SubeFormu implements OnChanges {

  @Input()
  sube: Sube | null = null;

  @Output()
  kapat = new EventEmitter<void>();

  @Output()
  kaydedildi = new EventEmitter<void>();

  kaydediliyorMu: boolean = false;
  hataMesaji: string = '';

  formModel = {
    id: 0,
    subeAdi: '',
    subeTelefonNo: '',
    subeAdres: '',
    subeDurumKodu:
      AktifPasifDurumlari.Aktif
  };

  constructor(
    private subeApi: SubeApi,
    private changeDetector: ChangeDetectorRef
  ) {
  }

  ngOnChanges(
    changes: SimpleChanges
  ): void {

    if (!changes['sube']) {
      return;
    }

    this.hataMesaji = '';

    if (this.sube === null) {
      this.formModel = {
        id: 0,
        subeAdi: '',
        subeTelefonNo: '',
        subeAdres: '',
        subeDurumKodu:
          AktifPasifDurumlari.Aktif
      };

      return;
    }

    this.formModel = {
      id: this.sube.id,
      subeAdi: this.sube.subeAdi,
      subeTelefonNo:
        this.sube.subeTelefonNo,
      subeAdres: this.sube.subeAdres,
      subeDurumKodu:
        this.sube.subeDurumKodu
    };
  }

  formuKapat(): void {
    this.kapat.emit();
  }

  kaydet(): void {

    if (
      this.formModel.subeAdi.trim() === '' ||
      this.formModel.subeTelefonNo.trim() === '' ||
      this.formModel.subeAdres.trim() === ''
    ) {
      this.hataMesaji =
        'Şube adı, telefon ve adres zorunludur.';

      return;
    }

    if (
      !onbirHaneliRakamMi(
        this.formModel.subeTelefonNo
      )
    ) {
      this.hataMesaji =
        'Şube telefon numarası 11 haneli ve yalnızca rakamlardan oluşmalıdır.';

      return;
    }

    this.kaydediliyorMu = true;
    this.hataMesaji = '';

    const gonderilecekSube = {
      subeAdi: this.formModel.subeAdi,
      subeTelefonNo:
        this.formModel.subeTelefonNo,
      subeAdres: this.formModel.subeAdres,
      subeDurumKodu:
        this.formModel.subeDurumKodu
    };

    if (this.sube === null) {
      this.subeApi
        .ekle(gonderilecekSube)
        .subscribe({

          next: () => {
            this.islemBasarili();
          },

          error: (hata) => {
            this.islemHatali(
              hata,
              'Şube eklenirken hata oluştu.'
            );
          }

        });

      return;
    }

    this.subeApi
      .guncelle(
        this.sube.id,
        gonderilecekSube
      )
      .subscribe({

        next: () => {
          this.islemBasarili();
        },

        error: (hata) => {
          this.islemHatali(
            hata,
            'Şube güncellenirken hata oluştu.'
          );
        }

      });
  }

  private islemBasarili(): void {
    this.kaydediliyorMu = false;

    this.kaydedildi.emit();
    this.kapat.emit();
  }

  private islemHatali(
    hata: any,
    varsayilanMesaj: string
  ): void {

    console.error(hata);

    this.kaydediliyorMu = false;

    this.hataMesaji =
      extractErrorMessage(hata, varsayilanMesaj);

    this.changeDetector.markForCheck();
  }
}
