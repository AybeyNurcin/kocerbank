import {
  AktifPasifDurumlari
} from '../../../shared/enums/aktif-pasif-durumlari-enum';

export interface SubeFiltre {
  id?: number;
  subeAdi?: string;
  subeKodu?: string;
  subeTelefonNo?: string;
  subeAdres?: string;
  subeDurumKodu?: AktifPasifDurumlari;
}
