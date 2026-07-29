import { Injectable } from '@angular/core';
import {
  HttpClient
} from '@angular/common/http';

import {
  Observable
} from 'rxjs';

import { Personel } from '../models/personel-model';
import { PersonelFiltre } from '../models/personel-filtre-model';

@Injectable({
  providedIn: 'root'
})
export class PersonelApi {

  private readonly apiUrl =
    'http://localhost:5107/api/Personel';

  constructor(private http: HttpClient) {
  }

  listele(
    kriterler: PersonelFiltre = {}
  ): Observable<Personel[]> {

    return this.http.post<Personel[]>(
      `${this.apiUrl}/listele`,
      kriterler
    );
  }

  ekle(personel: {
    ad: string;
    soyad: string;
    rol: string;
    sifre: string;
    tckn: string;
    telefonNo: string;
    adres: string;
    email: string;
    subeKodu: string;
    durumKodu: number;
  }): Observable<Personel> {

    return this.http.post<Personel>(
      `${this.apiUrl}/Ekle`,
      personel
    );
  }

  guncelle(
    id: number,
    personel: {
      ad: string;
      soyad: string;
      rol: string;
      sifre: string;
      tckn: string;
      telefonNo: string;
      adres: string;
      email: string;
      subeKodu: string;
      durumKodu: number;
    }
  ): Observable<void> {

    return this.http.put<void>(
      `${this.apiUrl}/Güncelle/${id}`,
      personel
    );
  }
}
