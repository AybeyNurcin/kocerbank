import {
  HesapHareketTipleri
} from '../../../shared/enums/hesap-hareket-tipleri-enum';

export interface HesapCekYatir {
  hesapId: number;
  islemTipi: HesapHareketTipleri;
  tutar: number;
  // Yalnızca okuma amaçlıdır. İstek gövdesinde gönderilmez;
  // işlemi yapan personelin sicilini backend header'dan atar.
  recordUser?: string | null;
  hareketId: number;
  yeniBakiye: number;
}
