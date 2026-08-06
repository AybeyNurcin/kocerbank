export interface DovizKuru {
  alis: number;
  satis: number;
}

export interface DovizKurulari {
  kurTarihi: string;
  kurlar: { [kod: string]: DovizKuru };
}
