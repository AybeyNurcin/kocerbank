import {
  HesapTipi
} from '../../../shared/enums/hesap-tipi-enum';

import {
  DovizCinsi
} from '../../../shared/enums/doviz-cinsi-enum';

import {
  AktifPasifDurumlari
} from '../../../shared/enums/aktif-pasif-durumlari-enum';

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
  hesapDurumKodu: AktifPasifDurumlari;

  musteriBilgileriId: number;
  hesapTipi: HesapTipi;

  recordUser: string | null;
  recordDate: string;
}
