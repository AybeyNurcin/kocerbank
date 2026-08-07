import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';

import {
  Hesap
} from '../models/hesap-model';

import {
  HesapApi
} from '../services/hesap-api';

import {
  HesapTipi
} from '../../../shared/enums/hesap-tipi-enum';

import {
  DovizCinsi
} from '../../../shared/enums/doviz-cinsi-enum';

@Component({
  selector: 'app-hesap-formu',
  standalone: false,
  templateUrl: './hesap-formu.html',
  styleUrl: './hesap-formu.css'
})
export class HesapFormu {

  @Input()
  musteriId: number = 0;

  @Output()
  kapat = new EventEmitter<void>();

  @Output()
  kaydedildi = new EventEmitter<Hesap>();

  kaydediliyorMu: boolean = false;
  hataMesaji: string = '';

  olusturulanHesap: Hesap | null = null;

  readonly HesapTipi = HesapTipi;
  readonly DovizCinsi = DovizCinsi;

  formModel: {
    hesapAdi: string;
    subeSubeKodu: string | undefined;
    dovizCinsi: DovizCinsi | null;
    hesapTipi: HesapTipi | null;
  } = {
      hesapAdi: '',
      subeSubeKodu: undefined,
      dovizCinsi: null,
      hesapTipi: null
    };

  constructor(
    private hesapApi: HesapApi,
    private changeDetector: ChangeDetectorRef
  ) {
  }

  formuKapat(): void {

    if (this.kaydediliyorMu) {
      return;
    }

    this.kapat.emit();
  }

  hesapOlustur(): void {

    if (this.musteriId <= 0) {
      this.hataMesaji =
        'Geçersiz müşteri ID bilgisi.';

      return;
    }

    if (this.formModel.hesapAdi.trim() === '') {
      this.hataMesaji =
        'Hesap adı girilmesi zorunludur.';

      return;
    }

    if (this.formModel.hesapAdi.trim().length > 50) {
      this.hataMesaji =
        'Hesap adı en fazla 50 karakter olabilir.';

      return;
    }

    if (
      (this.formModel.subeSubeKodu?.trim() ?? '') === ''
    ) {
      this.hataMesaji =
        'Şube seçilmesi zorunludur.';

      return;
    }

    if (this.formModel.dovizCinsi === null) {
      this.hataMesaji =
        'Döviz cinsi seçilmelidir.';

      return;
    }

    if (this.formModel.hesapTipi === null) {
      this.hataMesaji =
        'Hesap tipi seçilmelidir.';

      return;
    }

    this.kaydediliyorMu = true;
    this.hataMesaji = '';

    const gonderilecekHesap: Partial<Hesap> = {
      hesapAdi:
        this.formModel.hesapAdi.trim(),

      subeSubeKodu:
        this.formModel.subeSubeKodu,

      dovizCinsi:
        this.formModel.dovizCinsi,

      musteriBilgileriId:
        this.musteriId,

      hesapTipi:
        this.formModel.hesapTipi
    };

    this.hesapApi
      .ekle(gonderilecekHesap)
      .subscribe({

        next: (eklenenHesap: Hesap) => {

          this.kaydediliyorMu = false;

          this.olusturulanHesap =
            eklenenHesap;

          this.kaydedildi.emit(
            eklenenHesap
          );

          this.changeDetector
            .markForCheck();
        },

        error: (hata) => {

          console.error(
            'Hesap oluşturma hatası:',
            hata
          );

          this.kaydediliyorMu = false;

          if (typeof hata.error === 'string') {
            this.hataMesaji =
              hata.error;
          } else if (
            hata.error &&
            typeof hata.error.mesaj === 'string'
          ) {
            this.hataMesaji =
              hata.error.mesaj;
          } else {
            this.hataMesaji =
              'Hesap oluşturulurken bir hata oluştu.';
          }

          this.changeDetector
            .markForCheck();
        }

      });
  }

  hesaplaraDon(): void {
    this.kapat.emit();
  }

  dovizAdiniGetir(
    dovizCinsi: DovizCinsi
  ): string {

    switch (dovizCinsi) {

      case DovizCinsi.TL:
        return 'Türk Lirası';

      case DovizCinsi.USD:
        return 'Amerikan Doları';

      case DovizCinsi.EUR:
        return 'Euro';

      default:
        return '-';
    }
  }

  dovizSembolunuGetir(
    dovizCinsi: DovizCinsi
  ): string {

    switch (dovizCinsi) {

      case DovizCinsi.TL:
        return '₺';

      case DovizCinsi.USD:
        return '$';

      case DovizCinsi.EUR:
        return '€';

      default:
        return '';
    }
  }

  hesapTipiAdiniGetir(
    hesapTipi: HesapTipi
  ): string {

    switch (hesapTipi) {

      case HesapTipi.Vadesiz:
        return 'Vadesiz';

      case HesapTipi.Vadeli:
        return 'Vadeli';

      case HesapTipi.Yatirim:
        return 'Yatırım';

      default:
        return '-';
    }
  }
}
