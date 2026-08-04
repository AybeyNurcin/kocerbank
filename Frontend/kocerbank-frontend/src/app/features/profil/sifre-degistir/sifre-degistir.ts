import {
  ChangeDetectorRef,
  Component
} from '@angular/core';

import { Router } from '@angular/router';

import { PersonelApi } from '../../personeller/services/personel-api';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-sifre-degistir',
  standalone: false,
  templateUrl: './sifre-degistir.html',
  styleUrl: './sifre-degistir.css'
})
export class SifreDegistir {

  kaydediliyorMu: boolean = false;
  hataMesaji: string = '';
  basariMesaji: string = '';

  formModel: {
    eskiSifre: string;
    yeniSifre: string;
    yeniSifreTekrar: string;
  } = {
    eskiSifre: '',
    yeniSifre: '',
    yeniSifreTekrar: ''
  };

  constructor(
    private personelApi: PersonelApi,
    private authService: AuthService,
    private router: Router,
    private changeDetector: ChangeDetectorRef
  ) {
  }

  degistir(): void {

    const personelId = this.authService.personelIdGetir();

    if (personelId === null) {
      this.router.navigate(['/giris']);

      return;
    }

    this.basariMesaji = '';

    if (
      this.formModel.eskiSifre.trim() === '' ||
      this.formModel.yeniSifre.trim() === '' ||
      this.formModel.yeniSifreTekrar.trim() === ''
    ) {
      this.hataMesaji = 'Bütün alanlar zorunludur.';

      return;
    }

    if (this.formModel.yeniSifre.length < 8) {
      this.hataMesaji = 'Yeni şifre en az 8 karakter olmalıdır.';

      return;
    }

    // Yeni şifrelerin birbiriyle uyuştuğu, eski şifre veritabanında
    // doğrulanmadan önce kontrol edilir.
    if (this.formModel.yeniSifre !== this.formModel.yeniSifreTekrar) {
      this.hataMesaji = 'Yeni şifreler birbiriyle uyuşmuyor.';

      return;
    }

    this.kaydediliyorMu = true;
    this.hataMesaji = '';

    this.personelApi
      .sifreDegistir(personelId, {
        eskiSifre: this.formModel.eskiSifre,
        yeniSifre: this.formModel.yeniSifre
      })
      .subscribe({

        next: () => {
          this.kaydediliyorMu = false;
          this.basariMesaji = 'Şifreniz başarıyla değiştirildi.';

          this.formModel = {
            eskiSifre: '',
            yeniSifre: '',
            yeniSifreTekrar: ''
          };

          this.changeDetector.markForCheck();
        },

        error: (hata) => {
          this.islemHatali(
            hata,
            'Şifre değiştirilirken hata oluştu.'
          );

          this.kaydediliyorMu = false;
        }

      });
  }

  private islemHatali(hata: any, varsayilanMesaj: string): void {

    console.error(hata);

    if (typeof hata.error === 'string') {
      this.hataMesaji = hata.error;
    } else if (hata.error && typeof hata.error.mesaj === 'string') {
      this.hataMesaji = hata.error.mesaj;
    } else {
      this.hataMesaji = varsayilanMesaj;
    }

    this.changeDetector.markForCheck();
  }
}
