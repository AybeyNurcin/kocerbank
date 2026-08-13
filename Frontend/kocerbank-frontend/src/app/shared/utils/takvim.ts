// Paylaşılan takvim seçici bileşenleri (tarih-secici, tarih-araligi-secici)
// için ay ızgarası hesaplama ve ISO tarih (YYYY-MM-DD) dönüşüm yardımcıları.

export interface TakvimGunu {
  tarih: string;
  gun: number;
  buAyMi: boolean;
}

export const AY_ADLARI: string[] = [
  'Ocak', 'Şubat', 'Mart', 'Nisan',
  'Mayıs', 'Haziran', 'Temmuz', 'Ağustos',
  'Eylül', 'Ekim', 'Kasım', 'Aralık'
];

export const GUN_ADLARI_KISA: string[] = [
  'Pzt', 'Sal', 'Çar', 'Per', 'Cum', 'Cmt', 'Paz'
];

export function tarihiIsoyaCevir(
  yil: number,
  ay: number,
  gun: number
): string {

  const ayMetni =
    String(ay + 1).padStart(2, '0');

  const gunMetni =
    String(gun).padStart(2, '0');

  return `${yil}-${ayMetni}-${gunMetni}`;

}

export function isoTarihiAyristir(
  isoTarih: string
): { yil: number; ay: number; gun: number } | null {

  if (!isoTarih) {
    return null;
  }

  const parcalar = isoTarih.split('-');

  if (parcalar.length !== 3) {
    return null;
  }

  const yil = Number(parcalar[0]);
  const ay = Number(parcalar[1]) - 1;
  const gun = Number(parcalar[2]);

  if (
    Number.isNaN(yil) ||
    Number.isNaN(ay) ||
    Number.isNaN(gun)
  ) {
    return null;
  }

  return { yil, ay, gun };

}

export function isoTarihiGorunumeCevir(
  isoTarih: string | null | undefined
): string {

  const ayristirilmis =
    isoTarih ? isoTarihiAyristir(isoTarih) : null;

  if (!ayristirilmis) {
    return '';
  }

  const gunMetni =
    String(ayristirilmis.gun).padStart(2, '0');

  const ayMetni =
    String(ayristirilmis.ay + 1).padStart(2, '0');

  return `${gunMetni}.${ayMetni}.${ayristirilmis.yil}`;

}

// Bir ayın takvim ızgarasını (önceki/sonraki aydan taşan
// günler dahil, her zaman 6 satır x 7 sütun = 42 hücre) üretir.
// Hafta Pazartesi günü başlar.
export function ayGunleriniOlustur(
  yil: number,
  ay: number
): TakvimGunu[] {

  const ilkGun = new Date(yil, ay, 1);

  // JS: 0 = Pazar ... 6 = Cumartesi.
  // Haftayı Pazartesi'den başlatmak için kaydırılır.
  const ilkGununHaftaGunu =
    (ilkGun.getDay() + 6) % 7;

  const ayinGunSayisi =
    new Date(yil, ay + 1, 0).getDate();

  const oncekiAyinGunSayisi =
    new Date(yil, ay, 0).getDate();

  const gunler: TakvimGunu[] = [];

  for (
    let i = ilkGununHaftaGunu - 1;
    i >= 0;
    i--
  ) {

    const gun = oncekiAyinGunSayisi - i;

    let oncekiYil = yil;
    let oncekiAy = ay - 1;

    if (oncekiAy < 0) {
      oncekiAy = 11;
      oncekiYil = yil - 1;
    }

    gunler.push({
      tarih: tarihiIsoyaCevir(oncekiYil, oncekiAy, gun),
      gun,
      buAyMi: false
    });

  }

  for (
    let gun = 1;
    gun <= ayinGunSayisi;
    gun++
  ) {

    gunler.push({
      tarih: tarihiIsoyaCevir(yil, ay, gun),
      gun,
      buAyMi: true
    });

  }

  let sonrakiGun = 1;

  while (gunler.length < 42) {

    let sonrakiYil = yil;
    let sonrakiAy = ay + 1;

    if (sonrakiAy > 11) {
      sonrakiAy = 0;
      sonrakiYil = yil + 1;
    }

    gunler.push({
      tarih: tarihiIsoyaCevir(sonrakiYil, sonrakiAy, sonrakiGun),
      gun: sonrakiGun,
      buAyMi: false
    });

    sonrakiGun++;

  }

  return gunler;

}
