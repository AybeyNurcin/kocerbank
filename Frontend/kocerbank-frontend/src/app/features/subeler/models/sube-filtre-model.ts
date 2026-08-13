import {
  AktifPasifDurumlari
} from '../../../shared/enums/aktif-pasif-durumlari-enum';

export interface SubeFiltre {
  id?: number;
  subeAdi?: string;
  subeKodu?: string;
  subeAdres?: string;
  subeDurumKodu?: AktifPasifDurumlari;

  acilisTarihiBaslangic?: string;
  acilisTarihiBitis?: string;
}
