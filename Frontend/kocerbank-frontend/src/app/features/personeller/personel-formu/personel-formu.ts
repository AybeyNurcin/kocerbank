import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges
} from '@angular/core';

import { Personel } from '../models/personel-model';
import { PersonelApi } from '../services/personel-api';

import { Sube } from '../../subeler/models/sube-model';
import { SubeApi } from '../../subeler/services/sube-api';

import {
  AktifPasifDurumlari
} from '../../../shared/enums/aktif-pasif-durumlari-enum';

@Component({
  selector: 'app-personel-formu',
  standalone: false,
  templateUrl: './personel-formu.html',
  styleUrl: './personel-formu.css'
})
export class PersonelFormu implements OnChanges, OnInit {

  @Input()
  personel: Personel | null = null;

  @Output()
  kapat = new EventEmitter<void>();

  @Output()
  kaydedildi = new EventEmitter<void>();

  subeler: Sube[] = [];

  sifreGorunurMu: boolean = false;

  subeAramaTerimi: string = '';
  subeListesiAcikMi: boolean = false;

  kaydediliyorMu: boolean = false;
  hataMesaji: string = '';

  formModel = {
    id: 0,
    ad: '',
    soyad: '',
    rol: '',
    sifre: '',
    tckn: '',
    telefonNo: '',
    adres: '',
    email: '',
    subeKodu: '',
    durumKodu:
      AktifPasifDurumlari.Aktif
  };

  constructor(
    private personelApi: PersonelApi,
    private subeApi: SubeApi,
    private changeDetector: ChangeDetectorRef
  ) {
  }

  ngOnInit(): void {

    this.subeApi
      .listele({})
      .subscribe({

        next: (gelenSubeler: Sube[]) => {
          this.subeler = gelenSubeler;
          this.subeAramaTeriminiAyarla();
          this.changeDetector.markForCheck();
        },

        error: (hata) => {
          console.error(
            'Şube listeleme hatası:',
            hata
          );
        }

      });
  }

  ngOnChanges(
    changes: SimpleChanges
  ): void {

    if (!changes['personel']) {
      return;
    }

    this.hataMesaji = '';
    this.sifreGorunurMu = false;

    if (this.personel === null) {
      this.formModel = {
        id: 0,
        ad: '',
        soyad: '',
        rol: '',
        sifre: '',
        tckn: '',
        telefonNo: '',
        adres: '',
        email: '',
        subeKodu: '',
        durumKodu:
          AktifPasifDurumlari.Aktif
      };

      this.subeAramaTerimi = '';

      return;
    }

    this.formModel = {
      id: this.personel.id,
      ad: this.personel.ad,
      soyad: this.personel.soyad,
      rol: this.personel.rol,
      sifre: this.personel.sifre,
      tckn: this.personel.tckn,
      telefonNo: this.personel.telefonNo,
      adres: this.personel.adres,
      email: this.personel.email,
      subeKodu: this.personel.subeKodu,
      durumKodu: this.personel.durumKodu
    };

    this.subeAramaTeriminiAyarla();
  }

  private subeAramaTeriminiAyarla(): void {

    const seciliSube =
      this.subeler.find(
        sube => sube.subeKodu === this.formModel.subeKodu
      );

    this.subeAramaTerimi =
      seciliSube
        ? `${seciliSube.subeKodu} - ${seciliSube.subeAdi}`
        : this.formModel.subeKodu;
  }

  get filtrelenmisSubeler(): Sube[] {

    const terim =
      this.subeAramaTerimi.trim().toLocaleLowerCase('tr');

    if (terim === '') {
      return this.subeler;
    }

    return this.subeler.filter(
      sube =>
        sube.subeKodu.toLocaleLowerCase('tr').includes(terim) ||
        sube.subeAdi.toLocaleLowerCase('tr').includes(terim)
    );
  }

  subeAramaGirisiDegisti(): void {

    this.subeListesiAcikMi = true;

    if (this.subeAramaTerimi.trim() === '') {
      this.formModel.subeKodu = '';
    }
  }

  subeListesiniAc(): void {
    this.subeListesiAcikMi = true;
  }

  subeListesiniKapat(): void {
    this.subeListesiAcikMi = false;
  }

  subeSec(sube: Sube): void {

    this.formModel.subeKodu = sube.subeKodu;
    this.subeAramaTerimi = `${sube.subeKodu} - ${sube.subeAdi}`;
    this.subeListesiAcikMi = false;
  }

  sifreGorunurluguDegistir(): void {
    this.sifreGorunurMu = !this.sifreGorunurMu;
  }

  formuKapat(): void {
    this.kapat.emit();
  }

  kaydet(): void {

    if (
      this.formModel.ad.trim() === '' ||
      this.formModel.soyad.trim() === '' ||
      this.formModel.rol.trim() === '' ||
      this.formModel.sifre.trim() === '' ||
      this.formModel.tckn.trim() === '' ||
      this.formModel.telefonNo.trim() === '' ||
      this.formModel.adres.trim() === '' ||
      this.formModel.email.trim() === '' ||
      this.formModel.subeKodu.trim() === ''
    ) {
      this.hataMesaji =
        'Şube, ad, soyad, rol, şifre, TC kimlik no, telefon, adres ve e-posta zorunludur.';

      return;
    }

    if (this.formModel.tckn.trim().length !== 11) {
      this.hataMesaji =
        'TC kimlik numarası 11 haneli olmalıdır.';

      return;
    }

    this.kaydediliyorMu = true;
    this.hataMesaji = '';

    const gonderilecekPersonel = {
      ad: this.formModel.ad,
      soyad: this.formModel.soyad,
      rol: this.formModel.rol,
      sifre: this.formModel.sifre,
      tckn: this.formModel.tckn,
      telefonNo: this.formModel.telefonNo,
      adres: this.formModel.adres,
      email: this.formModel.email,
      subeKodu: this.formModel.subeKodu,
      durumKodu: this.formModel.durumKodu
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

    if (typeof hata.error === 'string') {
      this.hataMesaji = hata.error;
    } else if (hata.error?.mesaj) {
      this.hataMesaji = hata.error.mesaj;
    } else {
      this.hataMesaji = varsayilanMesaj;
    }

    this.changeDetector.markForCheck();
  }
}
