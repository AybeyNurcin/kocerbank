import {
  Component,
  OnDestroy,
  OnInit
} from '@angular/core';

import { Router } from '@angular/router';

import { AuthService } from '../../core/services/auth';

@Component({
  selector: 'app-admin-layout',
  standalone: false,
  templateUrl: './admin-layout.html',
  styleUrl: './admin-layout.css',
})
export class AdminLayout implements OnInit, OnDestroy {

  // Sayfa değişmeden tek bir ekranda hareketsiz kalınan durumlarda
  // oturumun zaman aşımına uğrayıp uğramadığını düzenli olarak kontrol eder.
  private oturumKontrolAraligi: any;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {
  }

  ngOnInit(): void {

    this.oturumKontrolAraligi = setInterval(() => {

      if (!this.authService.oturumAcikMi()) {
        this.router.navigate(['/giris']);
      }

    }, 60 * 1000);

  }

  ngOnDestroy(): void {
    clearInterval(this.oturumKontrolAraligi);
  }

}
