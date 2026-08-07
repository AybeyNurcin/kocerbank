import {
  TransferTipleri
} from '../../../shared/enums/transfer-tipleri-enum';

import {
  TransferKanallari
} from '../../../shared/enums/transfer-kanallari-enum';

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

  transferKanali: TransferKanallari;

  gonderenTutar: number;

  aciklama: string | null;
  // Yalnızca okuma amaçlıdır. İstek gövdesinde gönderilmez;
  // işlemi yapan personelin sicilini backend header'dan atar.
  recordUser?: string | null;


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
