import {
  HesapHareketTipleri
} from '../../../shared/enums/hesap-hareket-tipleri-enum';

export interface HesapCekYatir {
  hesapId: number;
  islemTipi: HesapHareketTipleri;
  miktar: number;
  recordUser: string | null;
  hareketId: number;
  yeniBakiye: number;
}
