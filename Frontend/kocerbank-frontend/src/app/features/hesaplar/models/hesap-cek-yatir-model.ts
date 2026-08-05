import {
  HesapHareketTipleri
} from '../../../shared/enums/hesap-hareket-tipleri-enum';

export interface HesapCekYatir {
  hesapId: number;
  islemTipi: HesapHareketTipleri;
  tutar: number;
  recordUser: string | null;
  hareketId: number;
  yeniBakiye: number;
}
