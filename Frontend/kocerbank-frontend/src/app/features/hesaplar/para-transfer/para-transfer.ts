import {
  ChangeDetectorRef,
  Component,
  OnDestroy
} from '@angular/core';

import {
  ParaTransferApi
} from '../services/para-transfer-api';

import {
  ParaTransfer
} from '../models/para-transfer-model';

import {
  TransferTipleri
} from '../../../shared/enums/transfer-tipleri-enum';

import {
  DovizCinsi
} from '../../../shared/enums/doviz-cinsi-enum';

import {
  AuthService
} from '../../../core/services/auth';


type TransferSekmesi =
  'Havale' | 'Virman';

type EkranTipi =
  'form' | 'onizleme' | 'basarili';


const BILGI_GETIRME_GECIKMESI_MS =
  500;


@Component({
  selector: 'app-para-transfer',
  standalone: false,
  templateUrl: './para-transfer.html',
  styleUrl: './para-transfer.css'
})
export class ParaTransferComponent
  implements OnDestroy {

  transferSekmesi: TransferSekmesi =
    'Havale';

  ekran: EkranTipi =
    'form';

  transfer: ParaTransfer =
    this.bosTransferOlustur();

  transferBilgileriYukleniyorMu =
    false;

  transferYapiliyorMu =
    false;

  hataMesaji =
    '';

  private bilgiGetirmeZamanlayici:
    ReturnType<typeof setTimeout> | null =
    null;

  private basariliZamanlayici:
    ReturnType<typeof setTimeout> | null =
    null;

  /*
   * Eski API isteğinin yeni form
   * değerlerini ezmesini engeller.
   */
  private bilgiIstegiNo =
    0;


  constructor(
    private paraTransferApi:
      ParaTransferApi,

    private authService:
      AuthService,

    private cdr:
      ChangeDetectorRef
  ) {
  }


  ngOnDestroy(): void {

    this.zamanlayicilariTemizle();

    /*
     * Devam eden eski bilgi isteğini
     * geçersiz duruma getirir.
     */
    this.bilgiIstegiNo++;

  }


  /*
   * HAVALE / VİRMAN SEKMESİ
   */

  sekmeSec(
    sekme: TransferSekmesi
  ): void {

    if (
      this.transferSekmesi === sekme
    ) {
      return;
    }

    this.transferSekmesi =
      sekme;

    this.formuSifirla();

  }


  /*
   * GÖNDEREN IBAN DEĞİŞİMİ
   */

  gonderenIbanDegisti(
    deger: string
  ): void {

    this.transfer.gonderenIBAN =
      this.ibanBicimlendir(deger);

    this.formBilgileriDegisti();

  }


  /*
   * ALICI IBAN DEĞİŞİMİ
   */

  aliciIbanDegisti(
    deger: string
  ): void {

    this.transfer.aliciIBAN =
      this.ibanBicimlendir(deger);

    this.formBilgileriDegisti();

  }


  /*
   * TUTAR DEĞİŞİMİ
   */

  tutarDegisti(
    deger: number | null
  ): void {

    this.transfer.gonderenTutar =
      deger ?? 0;

    /*
     * Tutar değişince önceki kur ve
     * alıcı tutarı artık geçerli değildir.
     */
    this.formBilgileriDegisti();

  }


  /*
   * AÇIKLAMA DEĞİŞİMİ
   *
   * Açıklama değiştiğinde hesap ve kur
   * bilgilerini tekrar almaya gerek yoktur.
   */

  aciklamaDegisti(
    deger: string
  ): void {

    this.transfer.aciklama =
      deger;

  }


  /*
   * FORMDA HESAPLAMAYI ETKİLEYEN
   * BİR ALAN DEĞİŞTİ
   */

  private formBilgileriDegisti():
    void {

    /*
     * Devam eden eski API isteğini
     * geçersiz hale getirir.
     */
    this.bilgiIstegiNo++;

    this.transferSonucBilgileriniSifirla();

    this.bilgileriGetirmeyiPlanla();

  }


  /*
   * GÖNDEREN IBAN YAPIŞTIRMA
   */

  gonderenIbanYapistir(
    event: ClipboardEvent
  ): void {

    event.preventDefault();

    const metin =
      event.clipboardData
        ?.getData('text') ?? '';

    this.gonderenIbanDegisti(metin);

  }


  /*
   * ALICI IBAN YAPIŞTIRMA
   */

  aliciIbanYapistir(
    event: ClipboardEvent
  ): void {

    event.preventDefault();

    const metin =
      event.clipboardData
        ?.getData('text') ?? '';

    this.aliciIbanDegisti(metin);

  }


  /*
   * IBAN BİÇİMLENDİRME
   */

  private ibanBicimlendir(
    deger: string
  ): string {

    const temiz =
      (deger ?? '')
        .replace(/\s+/g, '')
        .toUpperCase();

    if (
      temiz.length === 0 ||
      temiz === 'T'
    ) {
      return 'TR';
    }

    let govde: string;

    if (temiz.startsWith('TR')) {

      govde =
        temiz.slice(2);

    } else {

      govde =
        temiz;

    }

    /*
     * Türk IBAN'ı toplam 26 karakterdir.
     */
    const tamIban =
      ('TR' + govde)
        .slice(0, 26);

    return (
      tamIban
        .match(/.{1,4}/g)
        ?.join(' ') ??
      tamIban
    );

  }


  private ibanTemizle(
    iban: string
  ): string {

    return iban
      .replace(/\s+/g, '')
      .toUpperCase();

  }


  /*
   * IBAN GEÇERLİLİKLERİ
   */

  get gonderenIbanGecerliMi():
    boolean {

    return this.ibanGecerliMi(
      this.transfer.gonderenIBAN
    );

  }


  get aliciIbanGecerliMi():
    boolean {

    return this.ibanGecerliMi(
      this.transfer.aliciIBAN
    );

  }


  private ibanGecerliMi(
    iban: string
  ): boolean {

    const temizIban =
      this.ibanTemizle(iban);

    return /^TR\d{24}$/.test(
      temizIban
    );

  }


  /*
   * AYNI IBAN KONTROLÜ
   */

  get ibanlarAyniMi():
    boolean {

    if (
      !this.gonderenIbanGecerliMi ||
      !this.aliciIbanGecerliMi
    ) {
      return false;
    }

    return (
      this.ibanTemizle(
        this.transfer.gonderenIBAN
      ) ===
      this.ibanTemizle(
        this.transfer.aliciIBAN
      )
    );

  }


  /*
   * TUTAR GEÇERLİ Mİ?
   */

  get tutarGecerliMi():
    boolean {

    return (
      this.transfer.gonderenTutar > 0
    );

  }


  /*
   * BİLGİ ENDPOINT'İ
   * ÇAĞRILABİLİR Mİ?
   */

  private get bilgiGetirilebilirMi():
    boolean {

    return (
      this.gonderenIbanGecerliMi &&
      this.aliciIbanGecerliMi &&
      !this.ibanlarAyniMi &&
      this.tutarGecerliMi
    );

  }


  /*
   * İLERİ BUTONU AKTİF Mİ?
   */

  get ileriAktifMi():
    boolean {

    return (
      this.bilgiGetirilebilirMi &&
      this.transfer.gonderenHesap !==
      null &&
      this.transfer.aliciHesap !==
      null &&
      this.transfer.dovizKuru > 0 &&
      this.transfer.aliciTutar > 0 &&
      !this.transferBilgileriYukleniyorMu &&
      !this.transferYapiliyorMu
    );

  }


  /*
   * 500 MS SONRA HESAP VE
   * KUR BİLGİLERİNİ GETİR
   */

  private bilgileriGetirmeyiPlanla():
    void {

    this.bilgiGetirmeZamanlayicisiniTemizle();

    if (
      !this.bilgiGetirilebilirMi
    ) {
      return;
    }

    this.bilgiGetirmeZamanlayici =
      setTimeout(
        () => {

          this.bilgiGetirmeZamanlayici =
            null;

          this.transferBilgileriniGetir();

        },
        BILGI_GETIRME_GECIKMESI_MS
      );

  }


  /*
   * HESAP VE KUR BİLGİLERİNİ GETİR
   *
   * BU İŞLEM PARA TRANSFERİ YAPMAZ.
   */

  private transferBilgileriniGetir():
    void {

    if (
      !this.bilgiGetirilebilirMi
    ) {
      return;
    }

    const istekNo =
      ++this.bilgiIstegiNo;

    /*
     * Kullanıcının ekranda gördüğü
     * biçimlendirilmiş IBAN'ları koruyoruz.
     */
    const gonderenIban =
      this.transfer.gonderenIBAN;

    const aliciIban =
      this.transfer.aliciIBAN;

    const aciklama =
      this.transfer.aciklama;

    this.transferBilgileriYukleniyorMu =
      true;

    this.hataMesaji =
      '';

    const dto =
      this.transferIstekDtoOlustur();

    this.paraTransferApi
      .transferBilgileriniGetir(dto)
      .subscribe({

        next: (
          sonuc: ParaTransfer
        ) => {

          /*
           * Kullanıcı istek sürerken formu
           * değiştirdiyse bu yanıt kullanılmaz.
           */
          if (
            istekNo !== this.bilgiIstegiNo
          ) {
            return;
          }

          this.transfer =
          {
            ...sonuc,

            gonderenIBAN:
              gonderenIban,

            aliciIBAN:
              aliciIban,

            aciklama:
              aciklama
          };

          this.transferBilgileriYukleniyorMu =
            false;

          this.cdr.detectChanges();

        },

        error: (hata) => {

          if (
            istekNo !== this.bilgiIstegiNo
          ) {
            return;
          }

          console.error(
            'Transfer bilgileri getirme hatası:',
            hata
          );

          this.transferBilgileriYukleniyorMu =
            false;

          this.transferSonucBilgileriniSifirla();

          this.hataMesaji =
            hata?.error?.mesaj ??
            'Transfer bilgileri getirilirken bir hata oluştu.';

          this.cdr.detectChanges();

        }

      });

  }


  /*
   * ÖNİZLEME EKRANI
   */

  ileriGec(): void {

    if (
      !this.ileriAktifMi
    ) {
      return;
    }

    this.hataMesaji =
      '';

    this.ekran =
      'onizleme';

  }


  /*
   * FORMA GERİ DÖN
   */

  geriDon(): void {

    if (
      this.transferYapiliyorMu
    ) {
      return;
    }

    this.hataMesaji =
      '';

    this.ekran =
      'form';

  }


  /*
   * GERÇEK PARA TRANSFERİ
   *
   * Yalnızca onay ekranındaki
   * Onayla butonuyla çalışır.
   */

  onayla(): void {

    if (
      this.transferYapiliyorMu ||
      this.transfer.gonderenHesap ===
      null ||
      this.transfer.aliciHesap ===
      null
    ) {
      return;
    }

    this.transferYapiliyorMu =
      true;

    this.hataMesaji =
      '';

    const dto =
      this.transferIstekDtoOlustur();

    dto.recordUser =
      this.authService
        .personelSicilNoGetir();

    this.paraTransferApi
      .paraTransferiYap(dto)
      .subscribe({

        next: (
          sonuc: ParaTransfer
        ) => {

          /*
           * Transfer endpoint'i hesap ve kur
           * alanlarını null döndürse bile bilgi
           * ekranından gelen değerleri koruruz.
           */
          this.transfer =
          {
            ...sonuc,

            gonderenIBAN:
              this.transfer.gonderenIBAN,

            aliciIBAN:
              this.transfer.aliciIBAN,

            aciklama:
              this.transfer.aciklama,

            recordUser:
              dto.recordUser,

            gonderenHesap:
              sonuc.gonderenHesap ??
              this.transfer.gonderenHesap,

            aliciHesap:
              sonuc.aliciHesap ??
              this.transfer.aliciHesap,

            kurAciklamasi:
              sonuc.kurAciklamasi ??
              this.transfer.kurAciklamasi,

            kurTarihi:
              sonuc.kurTarihi ??
              this.transfer.kurTarihi
          };

          this.transferYapiliyorMu =
            false;

          this.ekran =
            'basarili';

          this.basariliZamanlayicisiniTemizle();

          this.basariliZamanlayici =
            setTimeout(
              () => {

                this.basariliZamanlayici =
                  null;

                this.formuSifirla();

                this.ekran =
                  'form';

                this.cdr.detectChanges();

              },
              3000
            );

          this.cdr.detectChanges();

        },

        error: (hata) => {

          console.error(
            'Para transferi hatası:',
            hata
          );

          this.transferYapiliyorMu =
            false;

          this.hataMesaji =
            hata?.error?.mesaj ??
            'Para transferi gerçekleştirilirken bir hata oluştu.';

          this.cdr.detectChanges();

        }

      });

  }


  /*
   * BACKEND'E GÖNDERİLECEK DTO
   *
   * Service tarafından doldurulacak alanları
   * frontend doldurmaz. Backend hesapları,
   * döviz türlerini ve kuru yeniden bulur.
   */

  private transferIstekDtoOlustur():
    ParaTransfer {

    return {
      ...this.bosTransferOlustur(),

      gonderenIBAN:
        this.ibanTemizle(
          this.transfer.gonderenIBAN
        ),

      aliciIBAN:
        this.ibanTemizle(
          this.transfer.aliciIBAN
        ),

      transferTipi:
        this.transferSekmesi ===
          'Havale'
          ? TransferTipleri.Havale
          : TransferTipleri.Virman,

      gonderenTutar:
        this.transfer.gonderenTutar,

      aciklama:
        this.transfer.aciklama,

      recordUser:
        this.transfer.recordUser
    };

  }


  /*
   * SERVICE VE PROSEDÜR SONUÇ
   * ALANLARINI SIFIRLA
   *
   * Kullanıcının girdiği IBAN, tutar ve
   * açıklama korunur.
   */

  private transferSonucBilgileriniSifirla():
    void {

    this.transfer.gonderenHesapId =
      0;

    this.transfer.aliciHesapId =
      0;

    this.transfer.gonderenDovizTipi =
      DovizCinsi.None;

    this.transfer.aliciDovizTipi =
      DovizCinsi.None;

    this.transfer.dovizKuru =
      0;

    this.transfer.gonderenHesap =
      null;

    this.transfer.aliciHesap =
      null;

    this.transfer.kurAciklamasi =
      null;

    this.transfer.kurTarihi =
      null;

    this.transfer.aliciTutar =
      0;

    this.transfer.transferId =
      0;

    this.transfer.gonderenHareketId =
      0;

    this.transfer.aliciHareketId =
      0;

    this.transfer.gonderenYeniBakiye =
      0;

    this.transfer.aliciYeniBakiye =
      0;

    this.hataMesaji =
      '';

  }


  /*
   * FORMU TAMAMEN SIFIRLA
   */

  private formuSifirla():
    void {

    this.zamanlayicilariTemizle();

    /*
     * Devam eden bilgi isteğini geçersiz kılar.
     */
    this.bilgiIstegiNo++;

    this.transfer =
      this.bosTransferOlustur();

    this.transfer.transferTipi =
      this.transferSekmesi ===
        'Havale'
        ? TransferTipleri.Havale
        : TransferTipleri.Virman;

    this.transferBilgileriYukleniyorMu =
      false;

    this.transferYapiliyorMu =
      false;

    this.hataMesaji =
      '';

  }


  /*
   * BOŞ TRANSFER NESNESİ
   */

  private bosTransferOlustur():
    ParaTransfer {

    return {
      gonderenIBAN:
        'TR',

      aliciIBAN:
        'TR',

      transferTipi:
        TransferTipleri.Havale,

      gonderenTutar:
        0,

      aciklama:
        null,

      recordUser:
        null,

      gonderenHesapId:
        0,

      aliciHesapId:
        0,

      gonderenDovizTipi:
        DovizCinsi.None,

      aliciDovizTipi:
        DovizCinsi.None,

      dovizKuru:
        0,

      gonderenHesap:
        null,

      aliciHesap:
        null,

      kurAciklamasi:
        null,

      kurTarihi:
        null,

      transferId:
        0,

      gonderenHareketId:
        0,

      aliciHareketId:
        0,

      gonderenYeniBakiye:
        0,

      aliciYeniBakiye:
        0,

      aliciTutar:
        0
    };

  }


  /*
   * ZAMANLAYICILAR
   */

  private bilgiGetirmeZamanlayicisiniTemizle():
    void {

    if (
      this.bilgiGetirmeZamanlayici ===
      null
    ) {
      return;
    }

    clearTimeout(
      this.bilgiGetirmeZamanlayici
    );

    this.bilgiGetirmeZamanlayici =
      null;

  }


  private basariliZamanlayicisiniTemizle():
    void {

    if (
      this.basariliZamanlayici ===
      null
    ) {
      return;
    }

    clearTimeout(
      this.basariliZamanlayici
    );

    this.basariliZamanlayici =
      null;

  }


  private zamanlayicilariTemizle():
    void {

    this.bilgiGetirmeZamanlayicisiniTemizle();

    this.basariliZamanlayicisiniTemizle();

  }


  /*
   * DÖVİZ KODUNU EKRANDA GÖSTER
   */

  dovizKoduGetir(
    dovizCinsi: DovizCinsi
  ): string {

    switch (dovizCinsi) {

      case DovizCinsi.TL:
        return 'TL';

      case DovizCinsi.USD:
        return 'USD';

      case DovizCinsi.EUR:
        return 'EUR';

      default:
        return '';

    }

  }

}
