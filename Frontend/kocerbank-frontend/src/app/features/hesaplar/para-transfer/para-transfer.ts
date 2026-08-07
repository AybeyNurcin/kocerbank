import {
  ChangeDetectorRef,
  Component,
  OnDestroy
} from '@angular/core';

import {
  ActivatedRoute
} from '@angular/router';

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
  TransferKanallari
} from '../../../shared/enums/transfer-kanallari-enum';

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

type TransferKanaliRotasi =
  'havale-eft' | 'swift';


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

  transferKanaliRotasi: TransferKanaliRotasi;

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

  /*
   * Gönderen/alıcı için ayrı ayrı yapılan
   * tekil hesap kontrollerinde eski bir API
   * cevabının yenisini ezmesini engeller.
   */
  private gonderenHesapIstegiNo =
    0;

  private aliciHesapIstegiNo =
    0;


  constructor(
    private paraTransferApi:
      ParaTransferApi,

    private authService:
      AuthService,

    private route:
      ActivatedRoute,

    private cdr:
      ChangeDetectorRef
  ) {

    this.transferKanaliRotasi =
      this.route.snapshot.data['transferKanali'] ===
        'swift'
        ? 'swift'
        : 'havale-eft';

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

    this.gonderenBilgisiGecersizlesti();

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

    this.aliciBilgisiGecersizlesti();

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
   * TUTAR DEĞİŞTİĞİNDE KUR VE ALICI
   * TUTARI YENİDEN HESAPLANMALIDIR
   */

  private formBilgileriDegisti():
    void {

    this.kurBilgileriniSifirla();

    this.bilgileriGetirmeyiPlanla();

  }


  /*
   * GÖNDEREN IBAN'A AİT HESAP BİLGİSİ
   * ARTIK GEÇERSİZ
   *
   * Kur, her iki hesaba da bağlı olduğu
   * için o da sıfırlanır. Alıcı tarafındaki
   * bilgiler korunur.
   */

  private gonderenBilgisiGecersizlesti():
    void {

    this.gonderenHesapIstegiNo++;

    this.transfer.gonderenHesapId =
      0;

    this.transfer.gonderenDovizTipi =
      DovizCinsi.None;

    this.transfer.gonderenHesap =
      null;

    this.gonderenMaskelenmisAdSoyad =
      '';

    this.hataMesaji =
      '';

    this.kurBilgileriniSifirla();

  }


  /*
   * ALICI IBAN'A AİT HESAP BİLGİSİ
   * ARTIK GEÇERSİZ
   *
   * Gönderen tarafındaki bilgiler korunur.
   */

  private aliciBilgisiGecersizlesti():
    void {

    this.aliciHesapIstegiNo++;

    this.transfer.aliciHesapId =
      0;

    this.transfer.aliciDovizTipi =
      DovizCinsi.None;

    this.transfer.aliciHesap =
      null;

    this.aliciMaskelenmisAdSoyad =
      '';

    this.hataMesaji =
      '';

    this.kurBilgileriniSifirla();

  }


  /*
   * KUR VE ALICI TUTARI ARTIK GEÇERSİZ
   */

  private kurBilgileriniSifirla():
    void {

    this.bilgiGetirmeZamanlayicisiniTemizle();

    this.bilgiIstegiNo++;

    this.transferBilgileriYukleniyorMu =
      false;

    this.transfer.dovizKuru =
      0;

    this.transfer.kurAciklamasi =
      null;

    this.transfer.kurTarihi =
      null;

    this.transfer.aliciTutar =
      0;

  }


  /*
   * GÖNDEREN IBAN ALANI ODAK KAYBETTİ
   *
   * Alıcı IBAN'ın girilmesi beklenmeden,
   * gönderen hesabın kontrolünü hemen yapar.
   */

  gonderenIbanOdakKaybetti(): void {

    if (
      !this.gonderenIbanGecerliMi
    ) {
      return;
    }

    const istekNo =
      ++this.gonderenHesapIstegiNo;

    const iban =
      this.ibanTemizle(
        this.transfer.gonderenIBAN
      );

    this.paraTransferApi
      .tekHesapBilgisiGetir(iban)
      .subscribe({

        next: (hesap) => {

          if (
            istekNo !==
            this.gonderenHesapIstegiNo
          ) {
            return;
          }

          this.transfer.gonderenHesapId =
            hesap.id;

          this.transfer.gonderenDovizTipi =
            hesap.dovizCinsi;

          this.transfer.gonderenHesap =
            hesap;

          this.gonderenMaskelenmisAdSoyad =
            this.adiMaskele(
              hesap.hesapSahibi
            );

          this.hataMesaji =
            '';

          this.cdr.detectChanges();

          this.ikisiDeHazirsaKuruGetir();

        },

        error: (hata) => {

          if (
            istekNo !==
            this.gonderenHesapIstegiNo
          ) {
            return;
          }

          this.transfer.gonderenHesapId =
            0;

          this.transfer.gonderenDovizTipi =
            DovizCinsi.None;

          this.transfer.gonderenHesap =
            null;

          this.gonderenMaskelenmisAdSoyad =
            '';

          this.hataMesaji =
            hata?.error?.mesaj ??
            'Gönderen hesap bilgisi getirilirken bir hata oluştu.';

          this.cdr.detectChanges();

        }

      });

  }


  /*
   * ALICI IBAN ALANI ODAK KAYBETTİ
   *
   * Gönderen IBAN'ın girilmesi beklenmeden,
   * alıcı hesabın kontrolünü hemen yapar.
   */

  aliciIbanOdakKaybetti(): void {

    if (
      !this.aliciIbanGecerliMi
    ) {
      return;
    }

    const istekNo =
      ++this.aliciHesapIstegiNo;

    const iban =
      this.ibanTemizle(
        this.transfer.aliciIBAN
      );

    this.paraTransferApi
      .tekHesapBilgisiGetir(iban)
      .subscribe({

        next: (hesap) => {

          if (
            istekNo !==
            this.aliciHesapIstegiNo
          ) {
            return;
          }

          this.transfer.aliciHesapId =
            hesap.id;

          this.transfer.aliciDovizTipi =
            hesap.dovizCinsi;

          this.transfer.aliciHesap =
            hesap;

          this.aliciMaskelenmisAdSoyad =
            this.adiMaskele(
              hesap.hesapSahibi
            );

          this.hataMesaji =
            '';

          this.cdr.detectChanges();

          this.ikisiDeHazirsaKuruGetir();

        },

        error: (hata) => {

          if (
            istekNo !==
            this.aliciHesapIstegiNo
          ) {
            return;
          }

          this.transfer.aliciHesapId =
            0;

          this.transfer.aliciDovizTipi =
            DovizCinsi.None;

          this.transfer.aliciHesap =
            null;

          this.aliciMaskelenmisAdSoyad =
            '';

          this.hataMesaji =
            hata?.error?.mesaj ??
            'Alıcı hesap bilgisi getirilirken bir hata oluştu.';

          this.cdr.detectChanges();

        }

      });

  }


  /*
   * HER İKİ TARAFIN HESAP BİLGİSİ DE
   * TEK TEK DOĞRULANDIYSA KURU GETİR
   */

  private ikisiDeHazirsaKuruGetir(): void {

    if (
      !this.bilgiGetirilebilirMi
    ) {
      return;
    }

    this.bilgiGetirmeZamanlayicisiniTemizle();

    this.transferBilgileriniGetir();

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
      !this.ibanlarAyniMi
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
      !this.transferYapiliyorMu &&
      this.dovizKanalKuraliGecerliMi
    );

  }


  /*
   * TRANSFER KANALI (DTO'YA YAZILACAK ENUM)
   */

  get transferKanaliEnum():
    TransferKanallari {

    return this.transferKanaliRotasi ===
      'swift'
      ? TransferKanallari.Swift
      : TransferKanallari.HavaleEft;

  }


  /*
   * DÖVİZ CİNSİ / TRANSFER KANALI KURALI
   *
   * Havale/EFT yalnızca TL hesaplar arasında yapılabilir.
   * SWIFT ile TL hesaptan TL hesaba transfer yapılamaz.
   */

  get dovizKanalKuraliGecerliMi():
    boolean {

    if (
      this.transfer.gonderenDovizTipi ===
      DovizCinsi.None ||
      this.transfer.aliciDovizTipi ===
      DovizCinsi.None
    ) {
      return true;
    }

    const ikisiDeTL =
      this.transfer.gonderenDovizTipi ===
      DovizCinsi.TL &&
      this.transfer.aliciDovizTipi ===
      DovizCinsi.TL;

    if (
      this.transferKanaliRotasi ===
      'havale-eft'
    ) {
      return ikisiDeTL;
    }

    return !ikisiDeTL;

  }


  get dovizKanalUyariMesaji():
    string {

    if (
      this.dovizKanalKuraliGecerliMi
    ) {
      return '';
    }

    return this.transferKanaliRotasi ===
      'havale-eft'
      ? 'Havale/EFT işlemi yalnızca TL hesaplar arasında yapılabilir. Farklı döviz cinsleri için SWIFT ekranını kullanınız.'
      : 'SWIFT işleminde TL hesaptan TL hesaba transfer yapılamaz. Bu işlem için Havale/EFT ekranını kullanınız.';

  }


  /*
   * KUR BİLGİSİ GÖSTERİLSİN Mİ?
   *
   * Havale/EFT'te döviz cinsleri her zaman TL-TL
   * olacağı için kur bilgisi anlamsızdır.
   */

  get kurBilgisiGosterilsinMi():
    boolean {

    return this.transferKanaliRotasi ===
      'swift';

  }


  /*
   * SAYFA BAŞLIĞI VE AÇIKLAMASI
   */

  get sayfaBasligi():
    string {

    return this.transferKanaliRotasi ===
      'swift'
      ? 'SWIFT'
      : 'Havale/EFT';

  }


  get sayfaAciklamasi():
    string {

    return this.transferKanaliRotasi ===
      'swift'
      ? 'Farklı döviz cinsleri arasında (TL hesaptan TL hesaba hariç) transfer işlemi başlatabilirsiniz.'
      : 'TL hesaplar arasında transfer işlemi başlatabilirsiniz.';

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

      transferKanali:
        this.transferKanaliEnum,

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

      transferKanali:
        TransferKanallari.None,

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
