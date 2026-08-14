import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import {
  ActivatedRoute
} from '@angular/router';

import {
  MusteriApi
} from '../../musteriler/services/musteri-api';

import {
  Musteri
} from '../../musteriler/models/musteri-model';

import {
  HesapApi
} from '../services/hesap-api';

import {
  Hesap
} from '../models/hesap-model';

import {
  ParaTransferApi
} from '../services/para-transfer-api';

import {
  ParaTransfer
} from '../models/para-transfer-model';

import {
  MusteriTipi
} from '../../../shared/enums/musteri-tipi-enum';

import {
  HesapDurumu
} from '../../../shared/enums/hesap-durumu-enum';

import {
  DovizCinsi
} from '../../../shared/enums/doviz-cinsi-enum';

import {
  TransferTipleri
} from '../../../shared/enums/transfer-tipleri-enum';

import {
  extractErrorMessage
} from '../../../shared/utils/hata-mesaji';

import {
  TransferKanallari
} from '../../../shared/enums/transfer-kanallari-enum';


type EkranTipi =
  'form' | 'onizleme' | 'basarili';


@Component({
  selector: 'app-virman',
  standalone: false,
  templateUrl: './virman.html',
  styleUrl: './virman.css'
})
export class VirmanComponent
  implements OnInit {

  ekran: EkranTipi = 'form';


  // TC ARAMA

  tc: string = '';

  aramaYapiliyorMu: boolean = false;
  aramaHataMesaji: string = '';

  musteri: Musteri | null = null;

  /*
   * Müşterinin virman yapılabilir
   * (aktif, TL) hesapları.
   */
  hesaplar: Hesap[] = [];


  // HESAP SEÇİMİ

  gonderenHesap: Hesap | null = null;
  aliciHesap: Hesap | null = null;


  // TUTAR VE AÇIKLAMA

  tutar: number = 0;
  aciklama: string | null = null;


  // TRANSFER

  transferYapiliyorMu: boolean = false;
  hataMesaji: string = '';

  sonuc: ParaTransfer | null = null;


  /*
   * Hesap sayfasından bir hesap seçili şekilde
   * gelindiyse, müşteri arandıktan sonra bu IBAN'a
   * sahip hesap otomatik olarak gönderen seçilir.
   */
  private otomatikSeciliGonderenIban:
    string | null = null;


  constructor(
    private musteriApi: MusteriApi,
    private hesapApi: HesapApi,
    private paraTransferApi: ParaTransferApi,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) {
  }


  /*
   * HESAP SAYFASINDAN GELİNDİYSE MÜŞTERİYİ
   * VE HESABI OTOMATİK SEÇ
   */

  ngOnInit(): void {

    const tcParametresi =
      this.route.snapshot
        .queryParamMap
        .get('tc');

    if (!tcParametresi) {
      return;
    }

    this.otomatikSeciliGonderenIban =
      this.route.snapshot
        .queryParamMap
        .get('gonderenIban');

    this.tcDegisti(tcParametresi);

    this.ara();

  }


  /*
   * TCKN 11 haneli ve rakamlardan oluşmalı,
   * ilk hane 0 olamaz.
   */
  get tcGecerliMi(): boolean {
    return /^[1-9]\d{10}$/.test(this.tc);
  }

  tcDegisti(deger: string): void {
    this.tc = (deger ?? '').replace(/\D/g, '').slice(0, 11);

    this.aramaHataMesaji = '';
    this.musteri = null;
    this.hesaplar = [];
    this.gonderenHesap = null;
    this.aliciHesap = null;
  }


  /*
   * MÜŞTERİYİ TC İLE ARA
   */
  ara(): void {

    if (!this.tcGecerliMi || this.aramaYapiliyorMu) {
      return;
    }

    this.aramaYapiliyorMu = true;
    this.aramaHataMesaji = '';
    this.musteri = null;
    this.hesaplar = [];
    this.gonderenHesap = null;
    this.aliciHesap = null;

    this.musteriApi.listele({ tckn: this.tc }).subscribe({

      next: (sonuclar) => {

        const bulunanMusteri = sonuclar[0] ?? null;

        if (bulunanMusteri === null) {
          this.aramaYapiliyorMu = false;
          this.aramaHataMesaji =
            'Bu TC kimlik numarasına ait müşteri bulunamadı.';
          this.cdr.detectChanges();
          return;
        }

        this.musteri = bulunanMusteri;
        this.musterininHesaplariniGetir(bulunanMusteri.id);

      },

      error: (hata) => {
        this.aramaYapiliyorMu = false;
        this.aramaHataMesaji =
          extractErrorMessage(
            hata,
            'Müşteri aranırken bir hata oluştu.'
          );
        this.cdr.detectChanges();
      }

    });

  }

  private musterininHesaplariniGetir(musteriId: number): void {

    this.hesapApi.listele({
      musteriBilgileriId: musteriId,
      dovizCinsi: DovizCinsi.TL
    }).subscribe({

      next: (hesaplar) => {

        this.aramaYapiliyorMu = false;

        this.hesaplar = hesaplar.filter(
          (hesap) => hesap.hesapDurumKodu === HesapDurumu.Aktif
        );

        if (this.hesaplar.length < 2) {
          this.aramaHataMesaji =
            'Bu müşterinin virman yapılabilecek en az iki aktif TL hesabı bulunmuyor.';
        }

        if (this.otomatikSeciliGonderenIban) {

          const otomatikHesap =
            this.hesaplar.find(
              (hesap) =>
                hesap.iban ===
                this.otomatikSeciliGonderenIban
            );

          if (otomatikHesap) {
            this.gonderenHesapSec(otomatikHesap);
          }

          this.otomatikSeciliGonderenIban = null;

        }

        this.cdr.detectChanges();

      },

      error: (hata) => {
        this.aramaYapiliyorMu = false;
        this.aramaHataMesaji =
          extractErrorMessage(
            hata,
            'Müşteri hesapları getirilirken bir hata oluştu.'
          );
        this.cdr.detectChanges();
      }

    });

  }

  get musteriAdSoyad(): string {

    if (this.musteri === null) {
      return '';
    }

    if (
      this.musteri.musteriTipi === MusteriTipi.Kurumsal &&
      this.musteri.unvan
    ) {
      return this.musteri.unvan;
    }

    return `${this.musteri.ad} ${this.musteri.soyad}`;

  }


  // HESAP SEÇİMİ

  /*
   * Alıcı hesap listesi, gönderen olarak
   * seçilen hesap hariç diğer hesaplardır.
   */
  get digerHesaplar(): Hesap[] {

    if (this.gonderenHesap === null) {
      return [];
    }

    return this.hesaplar.filter(
      (hesap) => hesap.id !== this.gonderenHesap!.id
    );

  }

  gonderenHesapSec(hesap: Hesap): void {

    this.gonderenHesap = hesap;

    /*
     * Gönderen olarak yeniden seçilen hesap,
     * daha önce alıcı olarak seçilmiş olabilir.
     */
    if (this.aliciHesap?.id === hesap.id) {
      this.aliciHesap = null;
    }

    this.tutar = 0;

  }

  aliciHesapSec(hesap: Hesap): void {
    this.aliciHesap = hesap;
  }


  // TUTAR

  tutarDegisti(deger: number | null): void {
    this.tutar = deger ?? 0;
  }

  get bakiyeYetersizMi(): boolean {

    return (
      this.gonderenHesap !== null &&
      this.tutar > 0 &&
      this.tutar > this.gonderenHesap.bakiye
    );

  }


  // İLERİ / GERİ

  get ileriAktifMi(): boolean {

    return (
      this.gonderenHesap !== null &&
      this.aliciHesap !== null &&
      this.tutar > 0 &&
      !this.bakiyeYetersizMi
    );

  }

  ileriGec(): void {

    if (!this.ileriAktifMi) {
      return;
    }

    this.hataMesaji = '';
    this.ekran = 'onizleme';

  }

  geriDon(): void {

    if (this.transferYapiliyorMu) {
      return;
    }

    this.hataMesaji = '';
    this.ekran = 'form';

  }


  // ONAYLA

  onayla(): void {

    if (
      this.transferYapiliyorMu ||
      this.gonderenHesap === null ||
      this.aliciHesap === null
    ) {
      return;
    }

    this.transferYapiliyorMu = true;
    this.hataMesaji = '';

    const dto = this.transferIstekDtoOlustur(
      this.gonderenHesap,
      this.aliciHesap
    );

    this.paraTransferApi.paraTransferiYap(dto).subscribe({

      next: (sonuc) => {
        this.transferYapiliyorMu = false;
        this.sonuc = sonuc;
        this.ekran = 'basarili';
        this.cdr.detectChanges();
      },

      error: (hata) => {
        this.transferYapiliyorMu = false;
        this.hataMesaji =
          extractErrorMessage(
            hata,
            'Virman işlemi gerçekleştirilirken bir hata oluştu.'
          );
        this.cdr.detectChanges();
      }

    });

  }

  private transferIstekDtoOlustur(
    gonderenHesap: Hesap,
    aliciHesap: Hesap
  ): ParaTransfer {

    return {
      gonderenIBAN: gonderenHesap.iban,
      aliciIBAN: aliciHesap.iban,

      /*
       * Gerçek transfer tipi, hesapların sahiplik
       * ve döviz bilgisine göre backend'de belirlenir.
       */
      transferTipi: TransferTipleri.None,

      transferKanali: TransferKanallari.Virman,

      gonderenTutar: this.tutar,

      aciklama: this.aciklama,

      aliciAdSoyad: null,

      gonderenHesapId: 0,
      aliciHesapId: 0,

      gonderenDovizTipi: DovizCinsi.None,
      aliciDovizTipi: DovizCinsi.None,

      dovizKuru: 0,

      gonderenHesap: null,
      aliciHesap: null,

      kurAciklamasi: null,
      kurTarihi: null,

      transferId: 0,

      gonderenHareketId: 0,
      aliciHareketId: 0,

      gonderenYeniBakiye: 0,
      aliciYeniBakiye: 0,

      aliciTutar: 0,

      komisyonTutari: 0
    };

  }


  // YENİ İŞLEM

  yeniIslemYap(): void {

    this.tc = '';
    this.aramaHataMesaji = '';
    this.musteri = null;
    this.hesaplar = [];

    this.gonderenHesap = null;
    this.aliciHesap = null;

    this.tutar = 0;
    this.aciklama = null;

    this.hataMesaji = '';
    this.sonuc = null;

    this.ekran = 'form';

  }

}
