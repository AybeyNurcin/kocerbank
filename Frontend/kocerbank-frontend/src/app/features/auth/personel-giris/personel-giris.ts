import { Component } from '@angular/core';
import { Router } from '@angular/router';

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

  constructor(private router: Router) {
  }

  girisYap(): void {

    if (this.sicilNo.trim() === '') {
      this.hataMesaji = 'Sicil numarası boş bırakılamaz.';
      return;
    }

    if (this.sifre.trim() === '') {
      this.hataMesaji = 'Şifre boş bırakılamaz.';
      return;
    }

    if (
      this.sicilNo === 'KB0001' &&
      this.sifre === '1234'
    ) {
      this.hataMesaji = '';
      this.router.navigate(['/dashboard']);
      return;
    }

    this.hataMesaji =
      'Sicil numarası veya şifre hatalıdır.';
  }
}
