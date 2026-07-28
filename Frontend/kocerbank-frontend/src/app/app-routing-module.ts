import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { PersonelGiris } from './features/auth/personel-giris/personel-giris';
import { Dashboard } from './features/dashboard/dashboard/dashboard';
import { SubeListesi } from './features/subeler/sube-listesi/sube-listesi';
import { PersonelListesi } from './features/personeller/personel-listesi/personel-listesi';
import { MusteriListesi } from './features/musteriler/musteri-listesi/musteri-listesi';
import { AdminLayout } from './layouts/admin-layout/admin-layout';

const routes: Routes = [
  {
    path: 'giris',
    component: PersonelGiris
  },
  {
    path: '',
    component: AdminLayout,
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
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      }
    ]
  },
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
export class AppRoutingModule { }
