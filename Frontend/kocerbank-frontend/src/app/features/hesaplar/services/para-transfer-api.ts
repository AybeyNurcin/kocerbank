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
  ParaTransfer,
  ParaTransferiDetay,
  TransferHesap
} from '../models/para-transfer-model';

import {
  TransferKanallari
} from '../../../shared/enums/transfer-kanallari-enum';

import { environment } from '../../../../environments/environment';


@Injectable({
  providedIn: 'root'
})
export class ParaTransferApi {

  private readonly apiUrl =
    `${environment.apiBaseUrl}/ParaTransfer`;


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
   * KARŞI TARAF IBAN'I BEKLENMEDEN
   * TEK BİR IBAN'IN HESAP BİLGİSİNİ GETİRİR.
   *
   * Para aktarmaz.
   */

  tekHesapBilgisiGetir(
    iban: string,
    kanal: TransferKanallari
  ): Observable<TransferHesap> {

    return this.http.post<TransferHesap>(
      `${this.apiUrl}/TekHesapBilgisiGetir` +
      `?iban=${encodeURIComponent(iban)}` +
      `&kanal=${kanal}`,
      null
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


  /*
   * HESAP HAREKETİ DETAY EKRANI İÇİN
   * TRANSFER BİLGİLERİNİ GETİRİR.
   *
   * Para aktarmaz.
   */

  transferDetayiGetir(
    id: number
  ): Observable<ParaTransferiDetay> {

    return this.http.post<ParaTransferiDetay>(
      `${this.apiUrl}/TransferDetayiGetir/${id}`,
      null
    );

  }

}
