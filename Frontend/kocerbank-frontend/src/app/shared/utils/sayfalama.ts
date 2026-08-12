/*
 * Liste ekranlarında (müşteri/personel/şube) tekrarlanan
 * istemci-taraflı sayfalama hesaplarını tek yerde toplar.
 */

export function sayfadakiKayitlar<T>(
  kayitlar: T[],
  mevcutSayfa: number,
  sayfaBasinaKayit: number
): T[] {

  const baslangicIndeksi =
    (mevcutSayfa - 1) * sayfaBasinaKayit;

  const bitisIndeksi =
    baslangicIndeksi + sayfaBasinaKayit;

  return kayitlar.slice(
    baslangicIndeksi,
    bitisIndeksi
  );
}

export function toplamSayfaSayisi(
  toplamKayitSayisi: number,
  sayfaBasinaKayit: number
): number {

  return Math.ceil(
    toplamKayitSayisi / sayfaBasinaKayit
  );
}

export function ilkKayitNumarasi(
  toplamKayitSayisi: number,
  mevcutSayfa: number,
  sayfaBasinaKayit: number
): number {

  if (toplamKayitSayisi === 0) {
    return 0;
  }

  return (
    (mevcutSayfa - 1) * sayfaBasinaKayit
  ) + 1;
}

export function sonKayitNumarasi(
  toplamKayitSayisi: number,
  mevcutSayfa: number,
  sayfaBasinaKayit: number
): number {

  return Math.min(
    mevcutSayfa * sayfaBasinaKayit,
    toplamKayitSayisi
  );
}
