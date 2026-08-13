import {
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  Input,
  OnChanges,
  Output,
  SimpleChanges
} from '@angular/core';

import {
  AY_ADLARI,
  GUN_ADLARI_KISA,
  TakvimGunu,
  ayGunleriniOlustur,
  isoTarihiAyristir,
  isoTarihiGorunumeCevir
} from '../../utils/takvim';

@Component({
  selector: 'app-tarih-secici',
  standalone: false,
  templateUrl: './tarih-secici.html',
  styleUrl: './tarih-secici.css'
})
export class TarihSecici implements OnChanges {

  @Input()
  seciliTarih: string | null | undefined;

  @Output()
  seciliTarihChange =
    new EventEmitter<string | null>();

  @Input()
  placeholder: string = 'gg.aa.yyyy';

  @Input()
  devreDisiMi: boolean = false;

  readonly ayAdlari = AY_ADLARI;
  readonly gunAdlariKisa = GUN_ADLARI_KISA;

  takvimAcikMi: boolean = false;

  gorunenYil: number = new Date().getFullYear();
  gorunenAy: number = new Date().getMonth();

  constructor(
    private elementRef: ElementRef
  ) {
  }

  ngOnChanges(
    changes: SimpleChanges
  ): void {

    if (changes['seciliTarih']) {
      this.gorunenAyiSeciliTariheGoreAyarla();
    }

  }

  private gorunenAyiSeciliTariheGoreAyarla(): void {

    const ayristirilmis =
      this.seciliTarih
        ? isoTarihiAyristir(this.seciliTarih)
        : null;

    if (ayristirilmis) {
      this.gorunenYil = ayristirilmis.yil;
      this.gorunenAy = ayristirilmis.ay;
    }

  }

  get gorunumMetni(): string {
    return isoTarihiGorunumeCevir(this.seciliTarih);
  }

  get takvimGunleri(): TakvimGunu[] {
    return ayGunleriniOlustur(this.gorunenYil, this.gorunenAy);
  }

  takvimiAc(): void {

    if (this.devreDisiMi) {
      return;
    }

    this.gorunenAyiSeciliTariheGoreAyarla();
    this.takvimAcikMi = true;

  }

  oncekiAy(): void {

    if (this.gorunenAy === 0) {
      this.gorunenAy = 11;
      this.gorunenYil--;
    } else {
      this.gorunenAy--;
    }

  }

  sonrakiAy(): void {

    if (this.gorunenAy === 11) {
      this.gorunenAy = 0;
      this.gorunenYil++;
    } else {
      this.gorunenAy++;
    }

  }

  gunSeciliMi(
    gun: TakvimGunu
  ): boolean {

    return gun.tarih === this.seciliTarih;

  }

  gunSec(
    gun: TakvimGunu
  ): void {

    this.seciliTarih = gun.tarih;

    this.seciliTarihChange.emit(
      this.seciliTarih
    );

    this.takvimAcikMi = false;

  }

  temizle(
    event: Event
  ): void {

    event.stopPropagation();

    this.seciliTarih = null;

    this.seciliTarihChange.emit(null);

    this.takvimAcikMi = false;

  }

  escTuslandi(
    event: Event
  ): void {

    this.takvimAcikMi = false;

    (event.target as HTMLElement).blur();

  }

  @HostListener('document:mousedown', ['$event'])
  disariTiklandi(
    event: MouseEvent
  ): void {

    if (!this.elementRef.nativeElement.contains(
      event.target
    )) {
      this.takvimAcikMi = false;
    }

  }
}
