import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';

import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';

import { PersonelGiris } from './features/auth/personel-giris/personel-giris';
import { Dashboard } from './features/dashboard/dashboard/dashboard';
import { SubeListesi } from './features/subeler/sube-listesi/sube-listesi';
import { PersonelListesi } from './features/personeller/personel-listesi/personel-listesi';
import { MusteriListesi } from './features/musteriler/musteri-listesi/musteri-listesi';
import { AdminLayout } from './layouts/admin-layout/admin-layout';
import { Navbar } from './shared/components/navbar/navbar';
import { SubeFormu } from './features/subeler/sube-formu/sube-formu';
import { PersonelFormu } from './features/personeller/personel-formu/personel-formu';

import { provideHttpClient } from '@angular/common/http';

@NgModule({
  declarations: [
    App,
    PersonelGiris,
    Dashboard,
    SubeListesi,
    PersonelListesi,
    MusteriListesi,
    AdminLayout,
    Navbar,
    SubeFormu,
    PersonelFormu,
  ],
  imports: [BrowserModule, AppRoutingModule, FormsModule],
  providers: [
  provideBrowserGlobalErrorListeners(),
  provideHttpClient()
  ],
  bootstrap: [App],
})
export class AppModule {}
