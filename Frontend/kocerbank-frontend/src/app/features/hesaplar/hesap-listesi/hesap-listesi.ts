import {
  Component,
  OnInit
} from '@angular/core';

import {
  ActivatedRoute
} from '@angular/router';

@Component({
  selector: 'app-hesap-listesi',
  standalone: false,
  templateUrl: './hesap-listesi.html',
  styleUrl: './hesap-listesi.css'
})
export class HesapListesi
  implements OnInit {

  musteriId: number = 0;

  constructor(
    private route: ActivatedRoute
  ) {
  }

  ngOnInit(): void {

    const routeMusteriId =
      this.route.snapshot.paramMap.get(
        'musteriId'
      );

    this.musteriId =
      Number(routeMusteriId);

  }
}
