/*
 * Backend'deki servislerin (SubeService, MusteriService,
 * PersonelService) TCKN/VKN/telefon alanlarında uyguladığı
 * sabit uzunluklu, yalnızca rakamlardan oluşma kuralı.
 */

export function onHaneliRakamMi(
  deger: string
): boolean {

  return /^[0-9]{10}$/.test(
    deger.trim()
  );

}


export function onbirHaneliRakamMi(
  deger: string
): boolean {

  return /^[0-9]{11}$/.test(
    deger.trim()
  );

}


/*
 * Uzunluk sınırı olmadan yalnızca rakam kontrolü
 * (ör. müşteri telefon alanları: uzunluk sınırı
 * zaten var, ama rakam zorunluluğu hiç yoktu).
 */

export function sadeceRakamMi(
  deger: string
): boolean {

  return /^[0-9]+$/.test(
    deger.trim()
  );

}


/*
 * Backend'deki MailAddress.TryCreate ile aynı
 * amaca hizmet eden, pratik bir e-posta biçim
 * kontrolü (boşluk içermeyen, @ ve en az bir
 * nokta barındıran bir alan adı).
 */

export function epostaGecerliMi(
  deger: string
): boolean {

  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(
    deger.trim()
  );

}


/*
 * TELEFON BİÇİMLENDİRME
 *
 * IBAN alanlarındaki (para-transfer.ts) biçimlendirme
 * mantığıyla aynı yaklaşım: kullanıcı yazarken alan
 * "0XXX XXX XXXX" olacak şekilde otomatik gruplanır;
 * başında 0 yoksa otomatik eklenir.
 */

export function telefonBicimlendir(
  deger: string
): string {

  const temiz =
    (deger ?? '')
      .replace(/\D/g, '');

  if (temiz.length === 0) {
    return '';
  }

  const govde =
    temiz.startsWith('0')
      ? temiz
      : '0' + temiz;

  const sinirli =
    govde.slice(0, 11);

  return [
    sinirli.slice(0, 4),
    sinirli.slice(4, 7),
    sinirli.slice(7, 11)
  ]
    .filter(
      (parca) => parca.length > 0
    )
    .join(' ');

}


/*
 * TELEFON TEMİZLEME
 *
 * Biçimlendirme sırasında eklenen boşlukları
 * kaldırıp saklama/doğrulama için saf rakam
 * dizisine döner (ibanTemizle ile aynı amaç).
 */

export function telefonTemizle(
  deger: string
): string {

  return (deger ?? '')
    .replace(/\s+/g, '');

}
