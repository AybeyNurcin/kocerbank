import {
  HesapHareketTipleri
} from '../../../shared/enums/hesap-hareket-tipleri-enum';

import {
  DovizCinsi
} from '../../../shared/enums/doviz-cinsi-enum';

export interface HesapHareketi {
  id: number;
  hesapBilgileriId: number;
  paraTransferiId: number | null;

  hesapHareketiTipi: HesapHareketTipleri;
  tutar: number;
  dovizCinsi: DovizCinsi;

  oncekiBakiye: number;
  sonrakiBakiye: number;

  /*
   * Yalnızca giden transfer hareketlerinde
   * (Eft/SwiftEft ya da SWIFT kanalından yapılan
   * Havale/Virman) dolu gelir; diğerlerinde 0'dır.
   */
  komisyonTutari: number;

  islemTarihi: string;

  recordUser: string | null;
  recordDate: string | null;
}
