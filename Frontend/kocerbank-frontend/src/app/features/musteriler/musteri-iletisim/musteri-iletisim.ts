import {
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';

import {
  Musteri
} from '../models/musteri-model';

@Component({
  selector: 'app-musteri-iletisim',
  standalone: false,
  templateUrl: './musteri-iletisim.html',
  styleUrl: './musteri-iletisim.css'
})
export class MusteriIletisim {

  @Input()
  musteri: Musteri | null = null;

  @Output()
  kapat = new EventEmitter<void>();

  popupKapat(): void {

    this.kapat.emit();

  }
}
