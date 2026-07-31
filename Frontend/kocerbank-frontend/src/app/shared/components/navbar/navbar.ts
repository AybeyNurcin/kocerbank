import { Component } from '@angular/core';

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

  constructor(private authService: AuthService) {
  }

  get sicilNo(): string {

    return this.authService.personelSicilNoGetir() ?? '';

  }
}
