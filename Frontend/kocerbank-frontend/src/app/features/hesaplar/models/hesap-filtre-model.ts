import { DovizCinsi } from '../../../shared/enums/doviz-cinsi-enum';

export interface HesapFiltre {
  id?: number;
  hesapAdi?: string;
  hesapNo?: string;
  iban?: string;
  bakiye?: number;
  subeSubeKodu?: string;
  dovizCinsi?: DovizCinsi;
  hesapAcilisTarihi?: string;
  hesapDurumKodu?: number;
  musteriBilgileriId?: number;
  hesapTipi?: number;
}
