import {
  Component,
  ElementRef,
  HostListener
} from '@angular/core';

import { Router } from '@angular/router';

import {
  AuthService
} from '../../../core/services/auth';

@Component({
  selector: 'app-navbar',
  standalone: false,
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar {

  profilMenusuAcikMi: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private elementRef: ElementRef
  ) {
  }

  get sicilNo(): string {

    return this.authService.personelSicilNoGetir() ?? '';

  }

  get adSoyad(): string {

    return this.authService.personelAdSoyadGetir() ?? '';

  }

  profilMenusunuDegistir(): void {
    this.profilMenusuAcikMi = !this.profilMenusuAcikMi;
  }

  profilMenusunuKapat(): void {
    this.profilMenusuAcikMi = false;
  }

  cikisYap(): void {

    this.profilMenusunuKapat();

    this.authService.oturumKapat();

    this.router.navigate(['/giris']);

  }

  // Menü dışına tıklanınca açık olan profil menüsünü kapatır.
  @HostListener('document:click', ['$event'])
  disariTiklamaKontrolu(event: MouseEvent): void {

    if (!this.profilMenusuAcikMi) {
      return;
    }

    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.profilMenusunuKapat();
    }

  }
}
