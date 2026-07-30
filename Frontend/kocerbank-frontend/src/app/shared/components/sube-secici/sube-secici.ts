import {
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  Input,
  OnInit,
  Output
} from '@angular/core';

import { Sube } from '../../../features/subeler/models/sube-model';
import { SubeApi } from '../../../features/subeler/services/sube-api';

@Component({
  selector: 'app-sube-secici',
  standalone: false,
  templateUrl: './sube-secici.html',
  styleUrl: './sube-secici.css'
})
export class SubeSecici implements OnInit {

  @Input()
  seciliSubeKodu: string | null | undefined = '';

  @Output()
  seciliSubeKoduChange =
    new EventEmitter<string>();

  @Input()
  bosSecenekGoster: boolean = false;

  @Input()
  bosSecenekMetni: string = 'Tümü';

  @Input()
  placeholder: string = 'Şube seçiniz';

  @Input()
  devreDisiMi: boolean = false;

  subeler: Sube[] = [];

  panelAcikMi: boolean = false;
  aramaMetni: string = '';

  constructor(
    private subeApi: SubeApi,
    private elementRef: ElementRef
  ) {
  }

  ngOnInit(): void {

    this.subeApi
      .listele({})
      .subscribe({

        next: (gelenSubeler: Sube[]) => {
          this.subeler = gelenSubeler;
        },

        error: (hata) => {
          console.error(
            'Şube listesi getirme hatası:',
            hata
          );
        }

      });
  }

  get filtrelenmisSubeler(): Sube[] {

    const arananMetin =
      this.aramaMetni.trim().toLocaleUpperCase('tr-TR');

    if (arananMetin === '') {
      return this.subeler;
    }

    return this.subeler.filter(sube =>

      sube.subeAdi
        .toLocaleUpperCase('tr-TR')
        .includes(arananMetin) ||

      sube.subeKodu
        .toLocaleUpperCase('tr-TR')
        .includes(arananMetin)
    );
  }

  get seciliSubeMetni(): string {

    if (this.seciliSubeKodu === null ||
        this.seciliSubeKodu === undefined ||
        this.seciliSubeKodu === '') {

      return this.bosSecenekGoster
        ? this.bosSecenekMetni
        : this.placeholder;
    }

    const seciliSube =
      this.subeler.find(
        sube => sube.subeKodu === this.seciliSubeKodu
      );

    if (!seciliSube) {
      return this.seciliSubeKodu;
    }

    return `${seciliSube.subeKodu} - ${seciliSube.subeAdi}`;
  }

  panelAcKapat(): void {

    if (this.devreDisiMi) {
      return;
    }

    this.panelAcikMi = !this.panelAcikMi;

    if (this.panelAcikMi) {
      this.aramaMetni = '';
    }
  }

  sec(sube: Sube | null): void {

    this.seciliSubeKodu =
      sube === null ? '' : sube.subeKodu;

    this.seciliSubeKoduChange.emit(
      this.seciliSubeKodu
    );

    this.panelAcikMi = false;
  }

  @HostListener('document:click', ['$event'])
  disariTiklandi(event: MouseEvent): void {

    if (!this.elementRef.nativeElement.contains(
      event.target
    )) {
      this.panelAcikMi = false;
    }
  }
}
