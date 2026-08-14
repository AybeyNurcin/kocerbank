import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

import { Musteri } from '../models/musteri-model';
import { MusteriIletisim as MusteriIletisimModel } from '../models/musteri-iletisim-model';
import { MusteriIletisimGuncelle } from '../models/musteri-iletisim-guncelle-model';
import { MusteriIletisimApi } from '../services/musteri-iletisim-api';
import { extractErrorMessage } from '../../../shared/utils/hata-mesaji';
import { epostaGecerliMi, sadeceRakamMi, telefonBicimlendir, telefonTemizle } from '../../../shared/utils/format-kontrol';

@Component({
  selector: 'app-musteri-iletisim',
  standalone: false,
  templateUrl: './musteri-iletisim.html',
  styleUrl: './musteri-iletisim.css'
})
export class MusteriIletisim implements OnInit {

  @Input()
  musteri: Musteri | null = null;

  @Output()
  kapat = new EventEmitter<void>();

  @Output()
  guncellendi = new EventEmitter<void>();

  iletisimBilgisi: MusteriIletisimModel | null = null;

  duzenlemeFormu: MusteriIletisimGuncelle = {
    telefonNo: '',
    eposta: '',
    evTelefonNo: null,
    isTelefonNo: null,
    evAdres: null,
    isAdres: null
  };

  yukleniyorMu: boolean = false;
  kaydediliyorMu: boolean = false;
  duzenlemeModuMu: boolean = false;

  hataMesaji: string = '';
  basariMesaji: string = '';

  constructor(
    private musteriIletisimApi: MusteriIletisimApi,
    private changeDetector: ChangeDetectorRef
  ) {
  }

  ngOnInit(): void {
    this.iletisimBilgileriniGetir();
  }

  private iletisimBilgileriniGetir(): void {
    if (this.musteri === null) {
      this.hataMesaji = 'Müşteri bilgisi bulunamadı.';
      return;
    }

    this.yukleniyorMu = true;
    this.hataMesaji = '';
    this.basariMesaji = '';

    this.musteriIletisimApi.getirByMusteriId(this.musteri.id).subscribe({
      next: (gelenIletisim: MusteriIletisimModel) => {
        this.iletisimBilgisi = gelenIletisim;
        this.yukleniyorMu = false;
        this.changeDetector.markForCheck();
      },

      error: (hata) => {
        console.error('Müşteri iletişim bilgileri getirme hatası:', hata);

        this.iletisimBilgisi = null;
        this.yukleniyorMu = false;
        this.hataMesaji = extractErrorMessage(hata, 'İletişim bilgileri getirilirken bir hata oluştu.');

        this.changeDetector.markForCheck();
      }
    });
  }

  duzenlemeyiBaslat(): void {
    if (this.iletisimBilgisi === null) {
      return;
    }

    this.duzenlemeFormu = {
      telefonNo: this.iletisimBilgisi.telefonNo,
      eposta: this.iletisimBilgisi.eposta,
      evTelefonNo: this.iletisimBilgisi.evTelefonNo,
      isTelefonNo: this.iletisimBilgisi.isTelefonNo,
      evAdres: this.iletisimBilgisi.evAdres,
      isAdres: this.iletisimBilgisi.isAdres
    };

    this.duzenlemeModuMu = true;
    this.hataMesaji = '';
    this.basariMesaji = '';
  }

  /*
   * TELEFON ALANLARI DEĞİŞİMİ
   *
   * IBAN girişindeki (para-transfer.ts) biçimlendirme
   * mantığıyla aynı: yazarken "0XXX XXX XXXX" olacak
   * şekilde otomatik gruplanır, başında 0 yoksa eklenir.
   */

  cepTelefonDegisti(deger: string): void {
    this.duzenlemeFormu.telefonNo = telefonBicimlendir(deger);
  }

  evTelefonDegisti(deger: string): void {
    this.duzenlemeFormu.evTelefonNo = telefonBicimlendir(deger);
  }

  isTelefonDegisti(deger: string): void {
    this.duzenlemeFormu.isTelefonNo = telefonBicimlendir(deger);
  }

  duzenlemeyiIptalEt(): void {
    if (this.kaydediliyorMu) {
      return;
    }

    this.duzenlemeModuMu = false;
    this.hataMesaji = '';
    this.basariMesaji = '';
  }

  degisiklikleriKaydet(): void {
    if (this.musteri === null || this.iletisimBilgisi === null || this.kaydediliyorMu) {
      return;
    }

    if (this.duzenlemeFormu.telefonNo.trim() === '') {
      this.hataMesaji = 'Cep telefonu girilmesi zorunludur.';
      return;
    }

    const cepTelefonTemiz = telefonTemizle(this.duzenlemeFormu.telefonNo);

    if (!sadeceRakamMi(cepTelefonTemiz)) {
      this.hataMesaji = 'Cep telefonu yalnızca rakamlardan oluşmalıdır.';
      return;
    }

    if (this.duzenlemeFormu.eposta.trim() === '') {
      this.hataMesaji = 'E-posta girilmesi zorunludur.';
      return;
    }

    if (!epostaGecerliMi(this.duzenlemeFormu.eposta)) {
      this.hataMesaji = 'Geçerli bir e-posta adresi girilmelidir.';
      return;
    }

    const evTelefonTemiz = this.metniTemizle(telefonTemizle(this.duzenlemeFormu.evTelefonNo ?? ''));
    const isTelefonTemiz = this.metniTemizle(telefonTemizle(this.duzenlemeFormu.isTelefonNo ?? ''));

    if (evTelefonTemiz !== null && !sadeceRakamMi(evTelefonTemiz)) {
      this.hataMesaji = 'Ev telefonu yalnızca rakamlardan oluşmalıdır.';
      return;
    }

    if (isTelefonTemiz !== null && !sadeceRakamMi(isTelefonTemiz)) {
      this.hataMesaji = 'İş telefonu yalnızca rakamlardan oluşmalıdır.';
      return;
    }

    const guncellenecekBilgiler: MusteriIletisimGuncelle = {
      telefonNo: cepTelefonTemiz,
      eposta: this.duzenlemeFormu.eposta.trim(),
      evTelefonNo: evTelefonTemiz,
      isTelefonNo: isTelefonTemiz,
      evAdres: this.metniTemizle(this.duzenlemeFormu.evAdres),
      isAdres: this.metniTemizle(this.duzenlemeFormu.isAdres)
    };

    this.kaydediliyorMu = true;
    this.hataMesaji = '';
    this.basariMesaji = '';

    this.musteriIletisimApi.guncelle(this.musteri.id, guncellenecekBilgiler).subscribe({
      next: () => {
        if (this.iletisimBilgisi !== null) {
          this.iletisimBilgisi = {
            ...this.iletisimBilgisi,
            ...guncellenecekBilgiler
          };
        }

        this.duzenlemeFormu = {
          ...guncellenecekBilgiler
        };

        this.kaydediliyorMu = false;
        this.duzenlemeModuMu = false;
        this.basariMesaji = 'İletişim bilgileri başarıyla güncellendi.';

        this.guncellendi.emit();

        this.changeDetector.markForCheck();
      },

      error: (hata) => {
        console.error('Müşteri iletişim bilgileri güncelleme hatası:', hata);

        this.kaydediliyorMu = false;
        this.hataMesaji = extractErrorMessage(hata, 'İletişim bilgileri güncellenirken bir hata oluştu.');

        this.changeDetector.markForCheck();
      }
    });
  }

  private metniTemizle(metin: string | null): string | null {
    if (metin === null || metin.trim() === '') {
      return null;
    }

    return metin.trim();
  }

  popupKapat(): void {
    if (this.kaydediliyorMu) {
      return;
    }

    this.kapat.emit();
  }
}
