import {
  TransferTipleri
} from '../../../shared/enums/transfer-tipleri-enum';

import {
  DovizCinsi
} from '../../../shared/enums/doviz-cinsi-enum';

import {
  Hesap
} from './hesap-model';


export interface TransferHesap
  extends Hesap {

  hesapSahibi: string;

}


export interface ParaTransfer {

  /*
   * FRONTEND TARAFINDAN GÖNDERİLEN
   */

  gonderenIBAN: string;
  aliciIBAN: string;

  transferTipi: TransferTipleri;

  gonderenTutar: number;

  aciklama: string | null;
  recordUser: string | null;


  /*
   * BACKEND SERVICE TARAFINDAN DOLDURULAN
   */

  gonderenHesapId: number;
  aliciHesapId: number;

  gonderenDovizTipi: DovizCinsi;
  aliciDovizTipi: DovizCinsi;

  dovizKuru: number;

  gonderenHesap: TransferHesap | null;
  aliciHesap: TransferHesap | null;

  kurAciklamasi: string | null;
  kurTarihi: string | null;


  /*
   * TRANSFER SONUCUNDA DOLDURULAN
   */

  transferId: number;

  gonderenHareketId: number;
  aliciHareketId: number;

  gonderenYeniBakiye: number;
  aliciYeniBakiye: number;

  aliciTutar: number;

}
