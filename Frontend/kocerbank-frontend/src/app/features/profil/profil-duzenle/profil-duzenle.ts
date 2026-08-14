import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import { Router } from '@angular/router';

import { Personel } from '../../personeller/models/personel-model';
import { PersonelApi } from '../../personeller/services/personel-api';
import { AuthService } from '../../../core/services/auth';
import { extractErrorMessage } from '../../../shared/utils/hata-mesaji';
import { epostaGecerliMi, onbirHaneliRakamMi } from '../../../shared/utils/format-kontrol';

@Component({
  selector: 'app-profil-duzenle',
  standalone: false,
  templateUrl: './profil-duzenle.html',
  styleUrl: './profil-duzenle.css'
})
export class ProfilDuzenle implements OnInit {

  yukleniyorMu: boolean = true;
  kaydediliyorMu: boolean = false;
  hataMesaji: string = '';
  basariMesaji: string = '';

  private personelId: number | null = null;
  mevcutPersonel: Personel | null = null;

  formModel: {
    ad: string;
    soyad: string;
    telefonNo: string;
    email: string;
    adres: string;
  } = {
    ad: '',
    soyad: '',
    telefonNo: '',
    email: '',
    adres: ''
  };

  constructor(
    private personelApi: PersonelApi,
    private authService: AuthService,
    private router: Router,
    private changeDetector: ChangeDetectorRef
  ) {
  }

  ngOnInit(): void {

    this.personelId = this.authService.personelIdGetir();

    if (this.personelId === null) {
      this.router.navigate(['/giris']);

      return;
    }

    this.personelApi
      .getirById(this.personelId)
      .subscribe({

        next: (personel) => {
          this.mevcutPersonel = personel;

          this.formModel = {
            ad: personel.ad,
            soyad: personel.soyad,
            telefonNo: personel.telefonNo,
            email: personel.email,
            adres: personel.adres
          };

          this.yukleniyorMu = false;

          this.changeDetector.markForCheck();
        },

        error: (hata) => {
          this.islemHatali(
            hata,
            'Profil bilgileri getirilirken hata oluştu.'
          );

          this.yukleniyorMu = false;
        }

      });
  }

  kaydet(): void {

    if (this.mevcutPersonel === null || this.personelId === null) {
      return;
    }

    if (
      this.formModel.ad.trim() === '' ||
      this.formModel.soyad.trim() === '' ||
      this.formModel.telefonNo.trim() === '' ||
      this.formModel.email.trim() === '' ||
      this.formModel.adres.trim() === ''
    ) {
      this.hataMesaji = 'Bütün alanlar zorunludur.';

      return;
    }

    if (!epostaGecerliMi(this.formModel.email)) {
      this.hataMesaji = 'Geçerli bir e-posta adresi girilmelidir.';

      return;
    }

    if (!onbirHaneliRakamMi(this.formModel.telefonNo)) {
      this.hataMesaji = 'Telefon numarası 11 haneli ve yalnızca rakamlardan oluşmalıdır.';

      return;
    }

    this.kaydediliyorMu = true;
    this.hataMesaji = '';
    this.basariMesaji = '';

    const gonderilecekPersonel = {
      ad: this.formModel.ad,
      soyad: this.formModel.soyad,
      email: this.formModel.email,
      sifre: this.mevcutPersonel.sifre,
      rol: this.mevcutPersonel.rol,
      tckn: this.mevcutPersonel.tckn,
      telefonNo: this.formModel.telefonNo,
      subeKodu: this.mevcutPersonel.subeKodu,
      adres: this.formModel.adres,
      durumKodu: this.mevcutPersonel.durumKodu
    };

    this.personelApi
      .guncelle(this.personelId, gonderilecekPersonel)
      .subscribe({

        next: () => {
          this.authService.adSoyadGuncelle(
            this.formModel.ad,
            this.formModel.soyad
          );

          this.kaydediliyorMu = false;
          this.basariMesaji = 'Bilgileriniz başarıyla güncellendi.';

          this.changeDetector.markForCheck();
        },

        error: (hata) => {
          this.islemHatali(
            hata,
            'Bilgiler güncellenirken hata oluştu.'
          );

          this.kaydediliyorMu = false;
        }

      });
  }

  private islemHatali(hata: any, varsayilanMesaj: string): void {

    console.error(hata);

    this.hataMesaji =
      extractErrorMessage(hata, varsayilanMesaj);

    this.changeDetector.markForCheck();
  }
}
