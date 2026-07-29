import {
  AktifPasifDurumlari
} from '../../../shared/enums/aktif-pasif-durumlari-enum';

export interface PersonelFiltre {
  id?: number;
  ad?: string;
  soyad?: string;
  rol?: string;
  tckn?: string;
  telefonNo?: string;
  subeKodu?: string;
  adres?: string;
  email?: string;
  durumKodu?: AktifPasifDurumlari;
}
