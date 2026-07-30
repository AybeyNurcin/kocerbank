import {
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges
} from '@angular/core';

import { Sube } from '../../../features/subeler/models/sube-model';
import { SubeApi } from '../../../features/subeler/services/sube-api';

@Component({
  selector: 'app-sube-secici',
  standalone: false,
  templateUrl: './sube-secici.html',
  styleUrl: './sube-secici.css'
})
export class SubeSecici implements OnInit, OnChanges {

  @Input()
  seciliSubeKodu: string | undefined;

  @Output()
  seciliSubeKoduChange =
    new EventEmitter<string | undefined>();

  @Input()
  placeholder: string = 'Şube kodu veya adı yazınız';

  @Input()
  devreDisiMi: boolean = false;

  // ŞUBE AUTOCOMPLETE ALANLARI

  tumSubeler: Sube[] = [];

  subeAramaMetni: string = '';
  subeSecenekleriAcikMi: boolean = false;
  subelerYukleniyorMu: boolean = false;

  constructor(
    private subeApi: SubeApi,
    private elementRef: ElementRef
  ) {
  }

  ngOnInit(): void {

    this.subeSecenekleriniGetir();

  }

  ngOnChanges(
    changes: SimpleChanges
  ): void {

    if (changes['seciliSubeKodu']) {
      this.subeAramaMetniniGuncelle();
    }

  }

  private subeSecenekleriniGetir(): void {

    this.subelerYukleniyorMu = true;

    // Buradaki filtresiz listeleme şube seçim
    // seçenekleri içindir.
    this.subeApi
      .listele({})
      .subscribe({

        next: (gelenSubeler: Sube[]) => {

          this.tumSubeler = gelenSubeler;
          this.subelerYukleniyorMu = false;

          this.subeAramaMetniniGuncelle();

        },

        error: (hata) => {

          console.error(
            'Şube seçenekleri getirilirken hata oluştu:',
            hata
          );

          this.tumSubeler = [];
          this.subelerYukleniyorMu = false;

        }

      });
  }

  private subeAramaMetniniGuncelle(): void {

    if (!this.seciliSubeKodu) {
      this.subeAramaMetni = '';
      return;
    }

    const seciliSube =
      this.tumSubeler.find(
        (sube: Sube) =>
          sube.subeKodu === this.seciliSubeKodu
      );

    if (seciliSube) {
      this.subeAramaMetni =
        this.subeGorunumMetni(seciliSube);
    }

  }

  get filtrelenmisSubeler(): Sube[] {

    const aranan =
      this.subeAramaMetni
        .trim()
        .toLocaleLowerCase('tr-TR');

    // Kullanıcı henüz bir şey yazmadıysa
    // ilk 10 şubeyi seçenek olarak gösterir.
    if (aranan === '') {

      return this.tumSubeler.slice(
        0,
        10
      );

    }

    // Yazılan metni hem şube kodunda
    // hem de şube adında arar.
    return this.tumSubeler
      .filter((sube: Sube) => {

        const subeKodu =
          sube.subeKodu
            .toLocaleLowerCase('tr-TR');

        const subeAdi =
          sube.subeAdi
            .toLocaleLowerCase('tr-TR');

        return (
          subeKodu.includes(aranan) ||
          subeAdi.includes(aranan)
        );

      })
      .slice(
        0,
        10
      );
  }

  subeAramasiDegisti(): void {

    this.subeSecenekleriAcikMi = true;

    // Kullanıcı seçilmiş şubenin üzerine
    // tekrar yazarsa eski seçimi temizler.
    const tamEslesenSube =
      this.tumSubeler.find(
        (sube: Sube) =>
          this.subeGorunumMetni(sube) ===
          this.subeAramaMetni
      );

    this.seciliSubeKodu =
      tamEslesenSube?.subeKodu;

    this.seciliSubeKoduChange.emit(
      this.seciliSubeKodu
    );

  }

  subeSecenekleriniAc(): void {

    if (this.devreDisiMi) {
      return;
    }

    this.subeSecenekleriAcikMi = true;

  }

  subeSecenekleriniKapat(): void {

    // Kullanıcının seçeneğe tıklayabilmesi için
    // çok kısa süre bekleyerek kapatır.
    setTimeout(() => {

      this.subeSecenekleriAcikMi = false;

    }, 150);

  }

  kacisTuslandi(
    girdi: HTMLInputElement
  ): void {

    this.subeSecenekleriAcikMi = false;

    girdi.blur();

  }

  @HostListener('document:click', ['$event'])
  disariTiklandi(event: MouseEvent): void {

    if (!this.elementRef.nativeElement.contains(
      event.target
    )) {
      this.subeSecenekleriAcikMi = false;
    }

  }

  subeSec(
    sube: Sube
  ): void {

    this.seciliSubeKodu =
      sube.subeKodu;

    // Input içerisinde kod ve ad birlikte görünür.
    this.subeAramaMetni =
      this.subeGorunumMetni(sube);

    this.seciliSubeKoduChange.emit(
      this.seciliSubeKodu
    );

    this.subeSecenekleriAcikMi = false;

  }

  subeGorunumMetni(
    sube: Sube
  ): string {

    return (
      sube.subeKodu +
      ' - ' +
      sube.subeAdi
    );

  }
}
