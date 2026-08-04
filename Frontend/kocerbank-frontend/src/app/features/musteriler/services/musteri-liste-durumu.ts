import {
  Injectable
} from '@angular/core';

import {
  Musteri
} from '../models/musteri-model';

import {
  MusteriFiltre
} from '../models/musteri-filtre-model';

// Müşteri listesi ekranından hesap ekranına gidip
// geri dönüldüğünde (özellikle tarayıcının geri tuşuyla)
// Angular router bileşeni yeniden oluşturduğu için
// arama kriterleri ve sonuçlar bu singleton serviste
// tutularak kaybolmaları engellenir.
@Injectable({
  providedIn: 'root'
})
export class MusteriListeDurumu {

  aramaKriterleri: MusteriFiltre = {};

  musteriler: Musteri[] = [];

  mevcutSayfa: number = 1;

  bilgiMesaji: string =
    'Filtreleme yapmak için önce müşteri tipini seçiniz.';

}
