import {
  HttpInterceptorFn
} from '@angular/common/http';

import {
  inject
} from '@angular/core';

import {
  AuthService
} from '../services/auth';

export const personelSicilInterceptor:
  HttpInterceptorFn = (request, next) => {

    const authService =
      inject(AuthService);

    const personelSicilNo =
      authService.personelSicilNoGetir();

    // Login sırasında henüz oturum açılmamış olacağı için
    // sicil yoksa istek değiştirilmeden gönderilir.
    if (
      personelSicilNo === null ||
      personelSicilNo.trim() === ''
    ) {
      return next(request);
    }

    const personelBilgiliRequest =
      request.clone({
        setHeaders: {
          'X-Personel-Sicil':
            personelSicilNo.trim()
        }
      });

    return next(personelBilgiliRequest);
  };
