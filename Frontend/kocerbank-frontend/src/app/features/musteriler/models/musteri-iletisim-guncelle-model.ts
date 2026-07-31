import { MusteriIletisimForm } from './musteri-iletisim-form-model';

export interface MusteriIletisimGuncelle extends MusteriIletisimForm {
  telefonNo: string;
  eposta: string;
}
