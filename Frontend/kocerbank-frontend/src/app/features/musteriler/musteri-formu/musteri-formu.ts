import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output
} from '@angular/core';

import {
  Musteri
} from '../models/musteri-model';

import {
  MusteriKaydet
} from '../models/musteri-kaydet-model';

import {
  MusteriIletisimForm
} from '../models/musteri-iletisim-form-model';

import {
  MusteriTamKaydet
} from '../models/musteri-tam-kaydet-model';

import {
  MusteriApi
} from '../services/musteri-api';

import {
  MusteriTipi
} from '../../../shared/enums/musteri-tipi-enum';

import {
  extractErrorMessage
} from '../../../shared/utils/hata-mesaji';

import {
  Cinsiyet
} from '../../../shared/enums/cinsiyet-enum';

import {
  AktifPasifDurumlari
} from '../../../shared/enums/aktif-pasif-durumlari-enum';

@Component({
  selector: 'app-musteri-formu',
  standalone: false,
  templateUrl: './musteri-formu.html',
  styleUrl: './musteri-formu.css'
})
export class MusteriFormu implements OnInit {

  @Input()
  musteri: Musteri | null = null;

  @Output()
  kapat = new EventEmitter<void>();

  @Output()
  kaydedildi = new EventEmitter<void>();


  // FORM DURUMU

  aktifAdim: 1 | 2 = 1;

  seciliMusteriTipi: MusteriTipi =
    MusteriTipi.None;

  kaydediliyorMu: boolean = false;
  hataMesaji: string = '';


  // HTML İÇİN ENUMLAR

  readonly musteriTipleri =
    MusteriTipi;

  readonly cinsiyetler =
    Cinsiyet;

  readonly durumlar =
    AktifPasifDurumlari;


  // BİREYSEL FORM BİLGİLERİ

  bireyselMusteriFormu: MusteriKaydet = {
    ad: '',
    soyad: '',
    eposta: '',
    telefonNo: '',

    dogumTarihi: null,

    tckn: null,
    vkn: null,

    cinsiyet: null,
    musteriTipi: MusteriTipi.Bireysel,

    subeSubeKodu: '',

    unvan: null,

    durumKodu:
      AktifPasifDurumlari.Aktif
  };


  // KURUMSAL FORM BİLGİLERİ

  kurumsalMusteriFormu: MusteriKaydet = {
    ad: '',
    soyad: '',
    eposta: '',
    telefonNo: '',

    dogumTarihi: null,

    tckn: null,
    vkn: null,

    cinsiyet: null,
    musteriTipi: MusteriTipi.Kurumsal,

    subeSubeKodu: '',

    unvan: null,

    durumKodu:
      AktifPasifDurumlari.Aktif
  };


  // İLETİŞİM FORMU

  iletisimFormu: MusteriIletisimForm = {
    evTelefonNo: null,
    isTelefonNo: null,

    evAdres: null,
    isAdres: null
  };


  constructor(
    private musteriApi: MusteriApi,
    private changeDetector: ChangeDetectorRef
  ) {
  }


  ngOnInit(): void {

    if (this.musteri !== null) {
      this.guncellemeFormunuHazirla();
    }

  }


  // GÜNCELLEME FORMUNU HAZIRLAMA

  private guncellemeFormunuHazirla(): void {

    if (this.musteri === null) {
      return;
    }

    this.seciliMusteriTipi =
      this.musteri.musteriTipi;

    const guncellenecekMusteri: MusteriKaydet = {
      ad: this.musteri.ad,
      soyad: this.musteri.soyad,
      eposta: this.musteri.eposta,
      telefonNo: this.musteri.telefonNo,

      dogumTarihi:
        this.musteri.dogumTarihi === null
          ? null
          : this.musteri.dogumTarihi.substring(
            0,
            10
          ),

      tckn: this.musteri.tckn,
      vkn: this.musteri.vkn,

      cinsiyet: this.musteri.cinsiyet,
      musteriTipi: this.musteri.musteriTipi,

      subeSubeKodu:
        this.musteri.subeSubeKodu,

      unvan: this.musteri.unvan,

      durumKodu:
        this.musteri.durumKodu
    };

    if (
      this.musteri.musteriTipi ===
      MusteriTipi.Bireysel
    ) {

      this.bireyselMusteriFormu =
        guncellenecekMusteri;

    }

    if (
      this.musteri.musteriTipi ===
      MusteriTipi.Kurumsal
    ) {

      this.kurumsalMusteriFormu =
        guncellenecekMusteri;

    }

  }


  // AKTİF MÜŞTERİ FORMU

  get aktifMusteriFormu(): MusteriKaydet {

    if (
      this.seciliMusteriTipi ===
      MusteriTipi.Kurumsal
    ) {
      return this.kurumsalMusteriFormu;
    }

    return this.bireyselMusteriFormu;

  }


  // MÜŞTERİ TİPİ SEÇİMİ

  musteriTipiniSec(
    musteriTipi: MusteriTipi
  ): void {

    // Güncellemede müşteri tipi değiştirilemez.
    if (this.musteri !== null) {
      return;
    }

    this.seciliMusteriTipi =
      musteriTipi;

    this.hataMesaji = '';

  }


  // ADIM İŞLEMLERİ

  sonrakiAdimaGec(): void {

    this.hataMesaji = '';

    if (
      this.seciliMusteriTipi ===
      MusteriTipi.None
    ) {

      this.hataMesaji =
        'Lütfen müşteri tipini seçiniz.';

      return;

    }

    if (!this.ilkAdimGecerliMi()) {
      return;
    }

    this.aktifAdim = 2;

  }


  oncekiAdimaDon(): void {

    this.aktifAdim = 1;
    this.hataMesaji = '';

  }


  // MÜŞTERİ FORMU KONTROLÜ

  private ilkAdimGecerliMi(): boolean {

    const form =
      this.aktifMusteriFormu;

    if (
      form.ad.trim() === '' ||
      form.soyad.trim() === '' ||
      form.eposta.trim() === '' ||
      form.telefonNo.trim() === '' ||
      form.subeSubeKodu.trim() === ''
    ) {

      this.hataMesaji =
        'Lütfen zorunlu müşteri bilgilerini doldurunuz.';

      return false;

    }

    if (
      form.durumKodu ===
      AktifPasifDurumlari.None
    ) {

      this.hataMesaji =
        'Lütfen müşteri durumunu seçiniz.';

      return false;

    }

    if (
      this.seciliMusteriTipi ===
      MusteriTipi.Bireysel
    ) {

      if (
        form.dogumTarihi === null ||
        form.tckn === null ||
        form.tckn.trim() === '' ||
        form.cinsiyet === null ||
        form.cinsiyet === Cinsiyet.None
      ) {

        this.hataMesaji =
          'Lütfen bireysel müşteri bilgilerini eksiksiz doldurunuz.';

        return false;

      }

    }

    if (
      this.seciliMusteriTipi ===
      MusteriTipi.Kurumsal
    ) {

      if (
        form.vkn === null ||
        form.vkn.trim() === '' ||
        form.unvan === null ||
        form.unvan.trim() === ''
      ) {

        this.hataMesaji =
          'Lütfen kurumsal müşteri bilgilerini eksiksiz doldurunuz.';

        return false;

      }

    }

    return true;

  }


  // MÜŞTERİ GÜNCELLEME

  guncelle(): void {

    if (
      this.musteri === null ||
      this.kaydediliyorMu
    ) {
      return;
    }

    this.hataMesaji = '';

    if (!this.ilkAdimGecerliMi()) {
      return;
    }

    this.kaydediliyorMu = true;

    this.musteriApi
      .guncelle(
        this.musteri.id,
        {
          ...this.aktifMusteriFormu
        }
      )
      .subscribe({

        next: () => {

          this.kaydediliyorMu =
            false;

          this.kaydedildi.emit();

        },

        error: (hata) => {

          console.error(
            'Müşteri güncelleme hatası:',
            hata
          );

          this.kaydediliyorMu =
            false;

          this.hataMesaji =
            extractErrorMessage(
              hata,
              'Müşteri güncellenirken bir hata oluştu.'
            );

          this.changeDetector
            .markForCheck();

        }

      });

  }


  // MÜŞTERİ VE İLETİŞİM TAM KAYIT

  kaydet(): void {

    if (
      this.musteri !== null ||
      this.kaydediliyorMu
    ) {
      return;
    }

    if (
      this.seciliMusteriTipi ===
      MusteriTipi.None
    ) {

      this.hataMesaji =
        'Müşteri tipi seçilmelidir.';

      return;

    }

    const kayit: MusteriTamKaydet = {
      musteri: {
        ...this.aktifMusteriFormu
      },

      iletisim: {
        ...this.iletisimFormu
      }
    };

    this.kaydediliyorMu = true;
    this.hataMesaji = '';

    this.musteriApi
      .tamKaydet(kayit)
      .subscribe({

        next: () => {

          this.kaydediliyorMu =
            false;

          this.kaydedildi.emit();

        },

        error: (hata) => {

          console.error(
            'Müşteri tam kayıt hatası:',
            hata
          );

          this.kaydediliyorMu =
            false;

          this.hataMesaji =
            extractErrorMessage(
              hata,
              'Müşteri kaydedilirken bir hata oluştu.'
            );

          this.changeDetector
            .markForCheck();

        }

      });

  }


  // FORMU KAPATMA

  formuKapat(): void {

    if (this.kaydediliyorMu) {
      return;
    }

    this.kapat.emit();

  }
}
