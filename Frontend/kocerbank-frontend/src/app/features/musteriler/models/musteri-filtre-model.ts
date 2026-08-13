import {
  AktifPasifDurumlari
} from '../../../shared/enums/aktif-pasif-durumlari-enum';

import {
  MusteriTipi
} from '../../../shared/enums/musteri-tipi-enum';

import {
  Cinsiyet
} from '../../../shared/enums/cinsiyet-enum';

export interface MusteriFiltre {
  ad?: string;
  soyad?: string;

  dogumTarihiBaslangic?: string;
  dogumTarihiBitis?: string;

  tckn?: string;
  vkn?: string;

  cinsiyet?: Cinsiyet;
  musteriTipi?: MusteriTipi;

  subeSubeKodu?: string;
  unvan?: string;

  durumKodu?: AktifPasifDurumlari;
}
