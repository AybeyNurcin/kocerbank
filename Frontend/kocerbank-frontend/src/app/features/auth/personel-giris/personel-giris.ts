import { Component } from '@angular/core';

import {
  ActivatedRoute,
  Router
} from '@angular/router';

import {
  AuthService
} from '../../../core/services/auth';

@Component({
  selector: 'app-personel-giris',
  standalone: false,
  templateUrl: './personel-giris.html',
  styleUrl: './personel-giris.css'
})
export class PersonelGiris {

  sicilNo: string = '';
  sifre: string = '';
  hataMesaji: string = '';

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private authService: AuthService
  ) {
  }

  girisYap(): void {

    if (this.sicilNo.trim() === '') {
      this.hataMesaji =
        'Sicil numarası boş bırakılamaz.';

      return;
    }

    if (this.sifre.trim() === '') {
      this.hataMesaji =
        'Şifre boş bırakılamaz.';

      return;
    }

    if (
      this.sicilNo === 'KB0001' &&
      this.sifre === '1234'
    ) {
      this.hataMesaji = '';

      // Personelin oturumunu açar.
      this.authService.oturumAc(
        this.sicilNo
      );

      // Guard tarafından saklanan adresi alır.
      const returnUrl =
        this.activatedRoute.snapshot
          .queryParamMap
          .get('returnUrl');

      // Kullanıcı doğrudan korumalı bir sayfaya
      // gitmek istemişse o sayfaya yönlendirilir.
      if (returnUrl !== null) {
        this.router.navigateByUrl(
          returnUrl
        );

        return;
      }

      // Normal girişte Dashboard açılır.
      this.router.navigate([
        '/dashboard'
      ]);

      return;
    }

    this.hataMesaji =
      'Sicil numarası veya şifre hatalıdır.';
  }
}
