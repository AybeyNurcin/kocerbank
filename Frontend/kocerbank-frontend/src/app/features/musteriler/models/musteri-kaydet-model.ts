import {
  AktifPasifDurumlari
} from '../../../shared/enums/aktif-pasif-durumlari-enum';

import {
  MusteriTipi
} from '../../../shared/enums/musteri-tipi-enum';

import {
  Cinsiyet
} from '../../../shared/enums/cinsiyet-enum';

export interface MusteriKaydet {
  ad: string;
  soyad: string;
  eposta: string;
  telefonNo: string;

  dogumTarihi: string | null;

  tckn: string | null;
  vkn: string | null;

  cinsiyet: Cinsiyet | null;
  musteriTipi: MusteriTipi;

  subeSubeKodu: string;

  unvan: string | null;

  durumKodu: AktifPasifDurumlari;
}
