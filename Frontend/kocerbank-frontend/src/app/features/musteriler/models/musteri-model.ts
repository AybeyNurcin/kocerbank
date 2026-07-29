import {
  AktifPasifDurumlari
} from '../../../shared/enums/aktif-pasif-durumlari-enum';

export interface Musteri {
  id: number;

  ad: string;
  soyad: string;

  eposta: string;
  telefonNo: string;

  dogumTarihi: string;

  tckn: string;
  vkn: string | null;

  cinsiyet: number;
  musteriTipi: number;

  subeSubeKodu: string;

  durumKodu: AktifPasifDurumlari;

  unvan: string | null;

  recordUser: string;
  recordDate: string;
}
