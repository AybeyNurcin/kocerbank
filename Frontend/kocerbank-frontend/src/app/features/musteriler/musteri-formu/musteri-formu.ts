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
  selector: 'app-musteri-formu',
  standalone: false,
  templateUrl: './musteri-formu.html',
  styleUrl: './musteri-formu.css'
})
export class MusteriFormu {

  @Input()
  musteri: Musteri | null = null;

  @Output()
  kapat = new EventEmitter<void>();

  @Output()
  kaydedildi = new EventEmitter<void>();

  formuKapat(): void {

    this.kapat.emit();

  }
}

