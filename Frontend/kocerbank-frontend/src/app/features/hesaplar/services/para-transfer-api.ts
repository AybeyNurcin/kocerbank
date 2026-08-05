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
  ParaTransfer
} from '../models/para-transfer-model';


@Injectable({
  providedIn: 'root'
})
export class ParaTransferApi {

  private readonly apiUrl =
    'http://localhost:5107/api/ParaTransfer';


  constructor(
    private http: HttpClient
  ) {
  }


  /*
   * HESAPLARI, HESAP SAHİPLERİNİ
   * VE KURU GETİRİR.
   *
   * Para aktarmaz.
   */

  transferBilgileriniGetir(
    dto: ParaTransfer
  ): Observable<ParaTransfer> {

    return this.http.post<ParaTransfer>(
      `${this.apiUrl}/TransferBilgileriniGetir`,
      dto
    );

  }


  /*
   * GERÇEK PARA TRANSFERİNİ YAPAR.
   *
   * Yalnızca Onayla butonunda çağrılacak.
   */

  paraTransferiYap(
    dto: ParaTransfer
  ): Observable<ParaTransfer> {

    return this.http.post<ParaTransfer>(
      `${this.apiUrl}/ParaTransferiYap`,
      dto
    );

  }

}
