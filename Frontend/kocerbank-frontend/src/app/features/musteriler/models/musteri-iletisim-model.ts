import {
  MusteriIletisimForm
} from './musteri-iletisim-form-model';

export interface MusteriIletisim
  extends MusteriIletisimForm {

  id: number;

  telefonNo: string;
  eposta: string;

  musteriBilgileriId: number;

  recordUser?: string | null;
  recordDate?: string | null;
}
