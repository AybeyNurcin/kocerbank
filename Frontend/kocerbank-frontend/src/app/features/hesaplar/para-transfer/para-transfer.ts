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


  /*
   * KULLANICININ GİRECEĞİ
   * AD SOYAD BİLGİLERİ
   */

  gonderenIsimSoyisim =
    '';

  aliciIsimSoyisim =
    '';


  /*
   * BACKEND'DEN GELEN GERÇEK
   * İSİMLERİN MASKELENMİŞ HALLERİ
   */

  gonderenMaskelenmisAdSoyad =
    '';

  aliciMaskelenmisAdSoyad =
    '';


  private bilgiGetirmeZamanlayici:
    ReturnType<typeof setTimeout> | null =
    null;

  private basariliZamanlayici:
    ReturnType<typeof setTimeout> | null =
    null;

  /*
   * Eski bir API cevabının yeni form
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
     * Component kapatıldığında devam eden
     * bilgi isteğini geçersiz hale getirir.
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

    this.ekran =
      'form';

  }


  /*
   * GÖNDEREN IBAN DEĞİŞİMİ
   */

  gonderenIbanDegisti(
    deger: string
  ): void {

    this.transfer.gonderenIBAN =
      this.ibanBicimlendir(deger);

    /*
     * IBAN değiştiği için daha önce girilen
     * gönderen adı artık geçerli değildir.
     */
    this.gonderenIsimSoyisim =
      '';

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

    /*
     * IBAN değiştiği için daha önce girilen
     * alıcı adı artık geçerli değildir.
     */
    this.aliciIsimSoyisim =
      '';

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
     * Tutar değiştiğinde kur ve alıcı tutarı
     * yeniden hesaplanmalıdır.
     *
     * Girilen ad soyadlar korunur.
     */
    this.formBilgileriDegisti();

  }


  /*
   * AÇIKLAMA DEĞİŞİMİ
   */

  aciklamaDegisti(
    deger: string
  ): void {

    this.transfer.aciklama =
      deger;

  }


  /*
   * FORMDA HESAPLAMAYI ETKİLEYEN
   * BİR BİLGİ DEĞİŞTİ
   */

  private formBilgileriDegisti():
    void {

    /*
     * Önceki API isteğinin sonucunu
     * geçersiz hale getirir.
     */
    this.bilgiIstegiNo++;

    /*
     * Önceki istek devam ediyor olsa bile
     * form değiştiği için yükleniyor durumu
     * kapatılır.
     */
    this.transferBilgileriYukleniyorMu =
      false;

    this.transferSonucBilgileriniSifirla();

    this.bilgileriGetirmeyiPlanla();

  }


  /*
   * IBAN YAPIŞTIRMA
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

    let ibanGovdesi: string;

    if (
      temiz.startsWith('TR')
    ) {

      ibanGovdesi =
        temiz.slice(2);

    } else {

      ibanGovdesi =
        temiz;

    }

    const tamIban =
      ('TR' + ibanGovdesi)
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
   * IBAN GEÇERLİLİĞİ
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
   * TUTAR KONTROLÜ
   */

  get tutarGecerliMi():
    boolean {

    return (
      this.transfer.gonderenTutar > 0
    );

  }


  /*
   * AD SOYAD ALANLARI DOLU MU?
   */

  get isimlerGirildiMi():
    boolean {

    return (
      this.gonderenIsimSoyisim
        .trim().length > 0 &&
      this.aliciIsimSoyisim
        .trim().length > 0
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
      this.isimlerGirildiMi &&
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
   * BU ENDPOINT PARA TRANSFERİ YAPMAZ.
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
     * Backend temiz IBAN döndürebilir.
     * Ekrandaki biçimlendirilmiş IBAN'ları
     * koruyoruz.
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
           * Kullanıcı istek devam ederken
           * formu değiştirdiyse eski cevap
           * kullanılmaz.
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

          /*
           * Gerçek isimler inputlara yazılmaz.
           * Yalnızca maskelenmiş şekilde
           * kullanıcıya gösterilir.
           */
          this.gonderenMaskelenmisAdSoyad =
            this.adiMaskele(
              sonuc.gonderenHesap
                ?.hesapSahibi ?? ''
            );

          this.aliciMaskelenmisAdSoyad =
            this.adiMaskele(
              sonuc.aliciHesap
                ?.hesapSahibi ?? ''
            );

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
   * AD SOYAD MASKELEME
   *
   * Murat Aybey Nurçin
   * Mu*** Ay*** Nu****
   */

  private adiMaskele(
    adSoyad: string
  ): string {

    if (
      adSoyad.trim() === ''
    ) {
      return '';
    }

    return adSoyad
      .trim()
      .split(/\s+/)
      .map(
        (kelime) => {

          if (
            kelime.length <= 2
          ) {
            return kelime;
          }

          return (
            kelime.slice(0, 2) +
            '*'.repeat(
              kelime.length - 2
            )
          );

        }
      )
      .join(' ');

  }


  /*
   * KARŞILAŞTIRMA ÖNCESİ
   * METNİ NORMALLEŞTİR
   */

  private metniNormallestir(
    deger: string
  ): string {

    return deger
      .trim()
      .replace(/\s+/g, ' ')
      .toLocaleUpperCase('tr-TR');

  }


  private gonderenIsmiEslesiyorMu():
    boolean {

    const gercekAdSoyad =
      this.transfer.gonderenHesap
        ?.hesapSahibi;

    if (
      !gercekAdSoyad
    ) {
      return false;
    }

    return (
      this.metniNormallestir(
        gercekAdSoyad
      ) ===
      this.metniNormallestir(
        this.gonderenIsimSoyisim
      )
    );

  }


  private aliciIsmiEslesiyorMu():
    boolean {

    const gercekAdSoyad =
      this.transfer.aliciHesap
        ?.hesapSahibi;

    if (
      !gercekAdSoyad
    ) {
      return false;
    }

    return (
      this.metniNormallestir(
        gercekAdSoyad
      ) ===
      this.metniNormallestir(
        this.aliciIsimSoyisim
      )
    );

  }


  /*
   * ÖNİZLEME EKRANINA GEÇ
   */

  ileriGec(): void {

    if (
      !this.ileriAktifMi
    ) {
      return;
    }

    if (
      !this.gonderenIsmiEslesiyorMu()
    ) {

      this.hataMesaji =
        'Girilen gönderen ad soyadı, gönderen IBAN sahibiyle eşleşmiyor.';

      return;

    }

    if (
      !this.aliciIsmiEslesiyorMu()
    ) {

      this.hataMesaji =
        'Girilen alıcı ad soyadı, alıcı IBAN sahibiyle eşleşmiyor.';

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

  yeniTransferYap(): void {

    this.formuSifirla();

    this.ekran =
      'form';

  }


  /*
   * GERÇEK PARA TRANSFERİ
   *
   * SADECE ONAYLA BUTONUNDA ÇAĞRILIR.
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

    /*
     * Önizlemede alanlar değiştirilemiyor
     * olsa da işlem öncesinde isimleri
     * tekrar kontrol ediyoruz.
     */
    if (
      !this.gonderenIsmiEslesiyorMu() ||
      !this.aliciIsmiEslesiyorMu()
    ) {

      this.hataMesaji =
        'Hesap sahibi bilgileri doğrulanamadı. Forma dönerek bilgileri kontrol ediniz.';

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
           * alanlarını null döndürürse bilgi
           * endpoint'inden gelen değerleri korur.
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
   * IBAN, tutar, açıklama ve kullanıcının
   * girdiği ad soyadlar korunur.
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

    this.gonderenMaskelenmisAdSoyad =
      '';

    this.aliciMaskelenmisAdSoyad =
      '';

    this.hataMesaji =
      '';

  }


  /*
   * FORMU TAMAMEN SIFIRLA
   */

  private formuSifirla():
    void {

    this.zamanlayicilariTemizle();

    this.bilgiIstegiNo++;

    this.transfer =
      this.bosTransferOlustur();

    this.transfer.transferTipi =
      this.transferSekmesi ===
        'Havale'
        ? TransferTipleri.Havale
        : TransferTipleri.Virman;

    this.gonderenIsimSoyisim =
      '';

    this.aliciIsimSoyisim =
      '';

    this.gonderenMaskelenmisAdSoyad =
      '';

    this.aliciMaskelenmisAdSoyad =
      '';

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
