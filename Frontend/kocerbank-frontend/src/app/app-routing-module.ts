import { NgModule } from '@angular/core';

import {
  RouterModule,
  Routes
} from '@angular/router';

import { PersonelGiris } from './features/auth/personel-giris/personel-giris';
import { Dashboard } from './features/dashboard/dashboard/dashboard';
import { SubeListesi } from './features/subeler/sube-listesi/sube-listesi';
import { PersonelListesi } from './features/personeller/personel-listesi/personel-listesi';
import { MusteriListesi } from './features/musteriler/musteri-listesi/musteri-listesi';
import { AdminLayout } from './layouts/admin-layout/admin-layout';

import {
  authGuard
} from './core/guards/auth-guard';

import {
  HesapListesi
} from './features/hesaplar/hesap-listesi/hesap-listesi';

import {
  ParaCekYatir
} from './features/hesaplar/para-cek-yatir/para-cek-yatir';

const routes: Routes = [

  // Giriş sayfası koruma altında değildir.
  {
    path: 'giris',
    component: PersonelGiris
  },

  // Girişten sonraki bütün sayfaların ortak layout'u.
  {
    path: '',
    component: AdminLayout,

    // AdminLayout açılmadan önce oturum kontrol edilir.
    canActivate: [
      authGuard
    ],

    children: [
      {
        path: 'dashboard',
        component: Dashboard
      },
      {
        path: 'subeler',
        component: SubeListesi
      },
      {
        path: 'personeller',
        component: PersonelListesi
      },
      {
        path: 'musteriler',
        component: MusteriListesi
      },
      {
        path: 'musteriler/:musteriId/hesaplar',
        component: HesapListesi
      },
      {
        path: 'para-cek-yatir',
        component: ParaCekYatir
      },

      // Ana adrese gidilirse Dashboard açılır.
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      }
    ]
  },

  // Tanımlanmayan adresler giriş ekranına gider.
  {
    path: '**',
    redirectTo: 'giris'
  }

];

@NgModule({
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class AppRoutingModule {
}
