import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HesapListesi } from './hesap-listesi';

describe('HesapListesi', () => {
  let component: HesapListesi;
  let fixture: ComponentFixture<HesapListesi>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [HesapListesi],
    }).compileComponents();

    fixture = TestBed.createComponent(HesapListesi);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
