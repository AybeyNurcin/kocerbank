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
  isoTarihiGorunumeCevir,
  yilBlokuOlustur
} from '../../utils/takvim';

type TakvimGorunumu = 'gun' | 'ay' | 'yil';

@Component({
  selector: 'app-tarih-araligi-secici',
  standalone: false,
  templateUrl: './tarih-araligi-secici.html',
  styleUrl: './tarih-araligi-secici.css'
})
export class TarihAraligiSecici implements OnChanges {

  @Input()
  baslangicTarihi: string | null | undefined;

  @Output()
  baslangicTarihiChange =
    new EventEmitter<string | null>();

  @Input()
  bitisTarihi: string | null | undefined;

  @Output()
  bitisTarihiChange =
    new EventEmitter<string | null>();

  @Input()
  placeholder: string = 'gg.aa.yyyy – gg.aa.yyyy';

  @Input()
  devreDisiMi: boolean = false;

  readonly ayAdlari = AY_ADLARI;
  readonly gunAdlariKisa = GUN_ADLARI_KISA;

  takvimAcikMi: boolean = false;
  gorunumModu: TakvimGorunumu = 'gun';

  gorunenYil: number = new Date().getFullYear();
  gorunenAy: number = new Date().getMonth();

  // Başlangıç seçilip bitiş henüz seçilmemişken,
  // fare ile üzerine gelinen günü aradaki aralığı
  // önizlemek için tutar.
  private hoverTarih: string | null = null;

  constructor(
    private elementRef: ElementRef
  ) {
  }

  ngOnChanges(
    changes: SimpleChanges
  ): void {

    if (
      changes['baslangicTarihi'] ||
      changes['bitisTarihi']
    ) {
      this.gorunenAyiSeciliTariheGoreAyarla();
    }

  }

  private gorunenAyiSeciliTariheGoreAyarla(): void {

    const referansTarih =
      this.baslangicTarihi ??
      this.bitisTarihi;

    const ayristirilmis =
      referansTarih
        ? isoTarihiAyristir(referansTarih)
        : null;

    if (ayristirilmis) {
      this.gorunenYil = ayristirilmis.yil;
      this.gorunenAy = ayristirilmis.ay;
    }

  }

  get gorunumMetni(): string {

    const baslangic =
      isoTarihiGorunumeCevir(this.baslangicTarihi);

    const bitis =
      isoTarihiGorunumeCevir(this.bitisTarihi);

    if (!baslangic && !bitis) {
      return '';
    }

    if (baslangic && !bitis) {
      return `${baslangic} – …`;
    }

    return `${baslangic} – ${bitis}`;

  }

  get takvimGunleri(): TakvimGunu[] {
    return ayGunleriniOlustur(this.gorunenYil, this.gorunenAy);
  }

  get yilListesi(): number[] {
    return yilBlokuOlustur(this.gorunenYil);
  }

  takvimiAc(): void {

    if (this.devreDisiMi) {
      return;
    }

    this.gorunenAyiSeciliTariheGoreAyarla();
    this.gorunumModu = 'gun';
    this.takvimAcikMi = true;

  }

  baslikTiklandi(): void {

    if (this.gorunumModu === 'gun') {
      this.gorunumModu = 'ay';
    } else if (this.gorunumModu === 'ay') {
      this.gorunumModu = 'yil';
    }

  }

  geriGit(): void {

    if (this.gorunumModu === 'gun') {
      this.oncekiAy();
    } else if (this.gorunumModu === 'ay') {
      this.gorunenYil--;
    } else {
      this.gorunenYil -= 12;
    }

  }

  ileriGit(): void {

    if (this.gorunumModu === 'gun') {
      this.sonrakiAy();
    } else if (this.gorunumModu === 'ay') {
      this.gorunenYil++;
    } else {
      this.gorunenYil += 12;
    }

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

  yilSec(
    yil: number
  ): void {

    this.gorunenYil = yil;
    this.gorunumModu = 'ay';

  }

  aySec(
    ayIndex: number
  ): void {

    this.gorunenAy = ayIndex;
    this.gorunumModu = 'gun';

  }

  gunUzerineGelindi(
    gun: TakvimGunu
  ): void {

    this.hoverTarih = gun.tarih;

  }

  gunHoveriTemizle(): void {

    this.hoverTarih = null;

  }

  // Yeni bir aralık seçimi başlar: hem başlangıç
  // hem bitiş doluysa veya hiçbiri seçili değilse
  // tıklanan gün yeni başlangıç olur. Yalnızca
  // başlangıç seçiliyse tıklanan gün, başlangıçtan
  // önceyse yeni başlangıç, sonraysa bitiş olur.
  gunSec(
    gun: TakvimGunu
  ): void {

    const yeniAralikBaslatilmali =
      !this.baslangicTarihi ||
      (this.baslangicTarihi && this.bitisTarihi);

    if (yeniAralikBaslatilmali) {

      this.baslangicTarihi = gun.tarih;
      this.bitisTarihi = null;

      this.baslangicTarihiChange.emit(
        this.baslangicTarihi
      );

      this.bitisTarihiChange.emit(null);

      return;
    }

    if (gun.tarih < this.baslangicTarihi!) {

      this.baslangicTarihi = gun.tarih;

      this.baslangicTarihiChange.emit(
        this.baslangicTarihi
      );

      return;
    }

    this.bitisTarihi = gun.tarih;

    this.bitisTarihiChange.emit(
      this.bitisTarihi
    );

    this.takvimAcikMi = false;
    this.gorunumModu = 'gun';

  }

  gunBaslangicMi(
    gun: TakvimGunu
  ): boolean {

    return gun.tarih === this.baslangicTarihi;

  }

  gunBitisMi(
    gun: TakvimGunu
  ): boolean {

    return gun.tarih === this.bitisTarihi;

  }

  // Başlangıç ile bitiş (veya bitiş seçilmemişse
  // fare önizlemesi) arasındaki günleri vurgular.
  gunAralikIcindeMi(
    gun: TakvimGunu
  ): boolean {

    if (!this.baslangicTarihi) {
      return false;
    }

    const digerUc =
      this.bitisTarihi ?? this.hoverTarih;

    if (!digerUc) {
      return false;
    }

    const erkenTarih =
      this.baslangicTarihi <= digerUc
        ? this.baslangicTarihi
        : digerUc;

    const gecTarih =
      this.baslangicTarihi <= digerUc
        ? digerUc
        : this.baslangicTarihi;

    return (
      gun.tarih >= erkenTarih &&
      gun.tarih <= gecTarih
    );

  }

  temizle(
    event: Event
  ): void {

    event.stopPropagation();

    this.baslangicTarihi = null;
    this.bitisTarihi = null;
    this.hoverTarih = null;

    this.baslangicTarihiChange.emit(null);
    this.bitisTarihiChange.emit(null);

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
