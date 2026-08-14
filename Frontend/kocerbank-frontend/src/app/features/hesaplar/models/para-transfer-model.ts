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


export interface ParaTransferiDetay {

  transferKanali: TransferKanallari;

  gonderenAdSoyad: string;
  gonderenIBAN: string;

  aliciAdSoyad: string;
  aliciIBAN: string;

  tutar: number;

  gonderenDovizCinsi: DovizCinsi;
  aliciDovizCinsi: DovizCinsi;

  dovizKuru: number;
  aliciTutar: number;

  aciklama: string | null;

  /*
   * İşlem türüne göre sabit masraf tutarı
   * (EFT: 4,67 TL; SWIFT: 16 EUR karşılığı).
   * Yalnızca bilgilendirme amaçlıdır.
   */
  komisyonTutari: number;

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
   * Yalnızca EFT'de (alıcının bizim bankamızda
   * hesabı yoksa) kullanılır. Gerçek bir hesapla
   * doğrulanamaz; kullanıcının girdiği isim
   * doğrudan kayda yazılır.
   */
  aliciAdSoyad: string | null;


  /*
   * BACKEND SERVICE TARAFINDAN DOLDURULAN
   */

  gonderenHesapId: number;
  // EFT'de alıcının bizim bankamızda hesabı olmadığı için null olabilir.
  aliciHesapId: number | null;

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

  /*
   * İşlem türüne göre sabit masraf tutarı
   * (EFT: 4,67 TL; SWIFT: 16 EUR karşılığı).
   * Yalnızca bilgilendirme amaçlıdır.
   */
  komisyonTutari: number;

}
