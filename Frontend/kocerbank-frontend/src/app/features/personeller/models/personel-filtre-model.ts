import {
  AktifPasifDurumlari
} from '../../../shared/enums/aktif-pasif-durumlari-enum';

export interface PersonelFiltre {
  id?: number;
  sicil?: string;
  ad?: string;
  soyad?: string;
  rol?: string;
  tckn?: string;
  subeKodu?: string;
  adres?: string;
  durumKodu?: AktifPasifDurumlari;

  baslangicTarihi?: string;
  bitisTarihi?: string;
}
