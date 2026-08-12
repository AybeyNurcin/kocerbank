import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges
} from '@angular/core';

import { Personel } from '../models/personel-model';
import { PersonelApi } from '../services/personel-api';

import {
  AktifPasifDurumlari
} from '../../../shared/enums/aktif-pasif-durumlari-enum';

import {
  extractErrorMessage
} from '../../../shared/utils/hata-mesaji';

@Component({
  selector: 'app-personel-formu',
  standalone: false,
  templateUrl: './personel-formu.html',
  styleUrl: './personel-formu.css'
})
export class PersonelFormu implements OnChanges {

  @Input()
  personel: Personel | null = null;

  @Output()
  kapat = new EventEmitter<void>();

  @Output()
  kaydedildi = new EventEmitter<void>();

  kaydediliyorMu: boolean = false;
  hataMesaji: string = '';

  sifreGoster: boolean = false;

  formModel: {
    id: number;
    ad: string;
    soyad: string;
    email: string;
    sifre: string;
    rol: string;
    tckn: string;
    telefonNo: string;
    subeKodu: string | undefined;
    adres: string;
    durumKodu: AktifPasifDurumlari;
  } = {
    id: 0,
    ad: '',
    soyad: '',
    email: '',
    sifre: '',
    rol: '',
    tckn: '',
    telefonNo: '',
    subeKodu: '',
    adres: '',
    durumKodu:
      AktifPasifDurumlari.Aktif
  };

  constructor(
    private personelApi: PersonelApi,
    private changeDetector: ChangeDetectorRef
  ) {
  }

  ngOnChanges(
    changes: SimpleChanges
  ): void {

    if (!changes['personel']) {
      return;
    }

    this.hataMesaji = '';
    this.sifreGoster = false;

    if (this.personel === null) {
      this.formModel = {
        id: 0,
        ad: '',
        soyad: '',
        email: '',
        sifre: '',
        rol: '',
        tckn: '',
        telefonNo: '',
        subeKodu: '',
        adres: '',
        durumKodu:
          AktifPasifDurumlari.Aktif
      };

      return;
    }

    this.formModel = {
      id: this.personel.id,
      ad: this.personel.ad,
      soyad: this.personel.soyad,
      email: this.personel.email,
      sifre: this.personel.sifre,
      rol: this.personel.rol,
      tckn: this.personel.tckn,
      telefonNo: this.personel.telefonNo,
      subeKodu: this.personel.subeKodu,
      adres: this.personel.adres,
      durumKodu:
        this.personel.durumKodu
    };
  }

  sifreGosterDegistir(): void {
    this.sifreGoster = !this.sifreGoster;
  }

  formuKapat(): void {
    this.kapat.emit();
  }

  kaydet(): void {

    if (
      this.formModel.ad.trim() === '' ||
      this.formModel.soyad.trim() === '' ||
      this.formModel.email.trim() === '' ||
      this.formModel.rol.trim() === '' ||
      this.formModel.tckn.trim() === '' ||
      this.formModel.telefonNo.trim() === '' ||
      (this.formModel.subeKodu?.trim() ?? '') === '' ||
      this.formModel.adres.trim() === ''
    ) {
      this.hataMesaji =
        'Şube ve durum dahil bütün alanlar zorunludur.';

      return;
    }

    if (
      this.personel === null &&
      this.formModel.sifre.trim().length < 8
    ) {
      this.hataMesaji =
        'Şifre en az 8 karakter olmalıdır.';

      return;
    }

    if (this.formModel.tckn.trim().length !== 11) {
      this.hataMesaji =
        'TC Kimlik Numarası 11 haneli olmalıdır.';

      return;
    }

    this.kaydediliyorMu = true;
    this.hataMesaji = '';

    const gonderilecekPersonel = {
      ad: this.formModel.ad,
      soyad: this.formModel.soyad,
      email: this.formModel.email,
      sifre: this.formModel.sifre,
      rol: this.formModel.rol,
      tckn: this.formModel.tckn,
      telefonNo: this.formModel.telefonNo,
      subeKodu: this.formModel.subeKodu ?? '',
      adres: this.formModel.adres,
      durumKodu:
        this.formModel.durumKodu
    };

    if (this.personel === null) {
      this.personelApi
        .ekle(gonderilecekPersonel)
        .subscribe({

          next: () => {
            this.islemBasarili();
          },

          error: (hata) => {
            this.islemHatali(
              hata,
              'Personel eklenirken hata oluştu.'
            );
          }

        });

      return;
    }

    this.personelApi
      .guncelle(
        this.personel.id,
        gonderilecekPersonel
      )
      .subscribe({

        next: () => {
          this.islemBasarili();
        },

        error: (hata) => {
          this.islemHatali(
            hata,
            'Personel güncellenirken hata oluştu.'
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
