import {
  HesapTipi
} from '../../../shared/enums/hesap-tipi-enum';

import {
  DovizCinsi
} from '../../../shared/enums/doviz-cinsi-enum';

import {
  HesapDurumu
} from '../../../shared/enums/hesap-durumu-enum';

export interface Hesap {
  id: number;

  hesapAdi: string;
  hesapNo: string;
  iban: string;

  bakiye: number;

  subeSubeKodu: string;
  subeAdi?: string;

  dovizCinsi: DovizCinsi;
  hesapAcilisTarihi: string;
  hesapDurumKodu: HesapDurumu;

  musteriBilgileriId: number;
  hesapTipi: HesapTipi;

  // Yalnızca okuma amaçlıdır. İstek gövdesinde gönderilmez;
  // işlemi yapan personelin sicilini backend header'dan atar.
  recordUser?: string | null;
  recordDate: string;
}
