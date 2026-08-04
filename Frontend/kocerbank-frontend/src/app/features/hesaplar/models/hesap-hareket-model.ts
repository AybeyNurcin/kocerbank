import {
  HesapHareketTipleri
} from '../../../shared/enums/hesap-hareket-tipleri-enum';

export interface HesapHareketi {
  id: number;
  hesapId: number;

  tarih: string;
  aciklama: string;

  islemTipi: HesapHareketTipleri;
  tutar: number;
}
