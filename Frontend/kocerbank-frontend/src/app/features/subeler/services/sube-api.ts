import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Sube } from '../models/sube-model';
import { SubeFiltre } from '../models/sube-filtre-model';

@Injectable({
  providedIn: 'root'
})
export class SubeApi {

  private readonly apiUrl =
    'http://localhost:5107/api/Sube';

  constructor(private http: HttpClient) {
  }

  listele(
    kriterler: SubeFiltre = {}
  ): Observable<Sube[]> {

    return this.http.post<Sube[]>(
      `${this.apiUrl}/Listele`,
      kriterler
    );
  }
}
