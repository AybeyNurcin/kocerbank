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
