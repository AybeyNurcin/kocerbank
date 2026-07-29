import {
  NgModule,
  provideBrowserGlobalErrorListeners
} from '@angular/core';

import {
  BrowserModule
} from '@angular/platform-browser';

import {
  FormsModule
} from '@angular/forms';

import {
  provideHttpClient
} from '@angular/common/http';

import {
  AppRoutingModule
} from './app-routing-module';

import {
  App
} from './app';

import {
  PersonelGiris
} from './features/auth/personel-giris/personel-giris';

import {
  Dashboard
} from './features/dashboard/dashboard/dashboard';

import {
  SubeListesi
} from './features/subeler/sube-listesi/sube-listesi';

import {
  SubeFormu
} from './features/subeler/sube-formu/sube-formu';

import {
  PersonelListesi
} from './features/personeller/personel-listesi/personel-listesi';

import {
  PersonelFormu
} from './features/personeller/personel-formu/personel-formu';

import {
  MusteriListesi
} from './features/musteriler/musteri-listesi/musteri-listesi';

import {
  MusteriFormu
} from './features/musteriler/musteri-formu/musteri-formu';

import {
  MusteriIletisim
} from './features/musteriler/musteri-iletisim/musteri-iletisim';

import {
  AdminLayout
} from './layouts/admin-layout/admin-layout';

import {
  Navbar
} from './shared/components/navbar/navbar';

@NgModule({
  declarations: [
    App,
    PersonelGiris,
    Dashboard,

    SubeListesi,
    SubeFormu,

    PersonelListesi,
    PersonelFormu,

    MusteriListesi,
    MusteriFormu,
    MusteriIletisim,

    AdminLayout,
    Navbar
  ],

  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule
  ],

  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient()
  ],

  bootstrap: [
    App
  ]
})
export class AppModule {
}
