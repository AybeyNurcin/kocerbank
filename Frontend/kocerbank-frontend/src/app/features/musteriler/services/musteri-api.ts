import {
  Injectable
} from '@angular/core';

import {
  HttpClient
} from '@angular/common/http';

import {
  Observable
} from 'rxjs';

import {
  Musteri
} from '../models/musteri-model';

import {
  MusteriFiltre
} from '../models/musteri-filtre-model';

import {
  MusteriKaydet
} from '../models/musteri-kaydet-model';

@Injectable({
  providedIn: 'root'
})
export class MusteriApi {

  private readonly apiUrl =
    'http://localhost:5107/api/Musteri';

  constructor(
    private http: HttpClient
  ) {
  }

  listele(
    kriterler: MusteriFiltre
  ): Observable<Musteri[]> {

    return this.http.post<Musteri[]>(
      `${this.apiUrl}/Listele`,
      kriterler
    );
  }

  getirById(
    id: number
  ): Observable<Musteri> {

    return this.http.get<Musteri>(
      `${this.apiUrl}/${id}`
    );
  }

  ekle(
    musteri: MusteriKaydet
  ): Observable<Musteri> {

    return this.http.post<Musteri>(
      `${this.apiUrl}/Ekle`,
      musteri
    );
  }

  guncelle(
    id: number,
    musteri: MusteriKaydet
  ): Observable<void> {

    return this.http.put<void>(
      `${this.apiUrl}/${id}`,
      musteri
    );
  }
}
