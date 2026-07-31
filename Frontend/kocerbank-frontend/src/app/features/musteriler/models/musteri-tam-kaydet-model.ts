import {
  MusteriKaydet
} from './musteri-kaydet-model';

import {
  MusteriIletisimForm
} from './musteri-iletisim-form-model';

export interface MusteriTamKaydet {
  musteri: MusteriKaydet;
  iletisim: MusteriIletisimForm;
}

export interface MusteriTamKaydetSonuc {
  musteriId: number;
  iletisimId: number;
  kayitOlusturmaTarihi: string;
}
