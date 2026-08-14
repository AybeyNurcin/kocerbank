/*
 * Backend'deki ParaTransferService.TutarKontrolEt ile
 * birebir aynı kurallar: tutar en fazla 999999999999.99
 * olabilir ve en fazla iki ondalık basamak içerebilir.
 * (Sıfırdan büyük olma şartı burada değil, her ekranın
 * kendi "tutar girildi mi" kontrolünde ayrıca yapılır.)
 */

export const TUTAR_UST_SINIRI = 999999999999.99;


function ondalikBasamakSayisiGecerliMi(
  tutar: number
): boolean {

  const parcalar =
    tutar.toString().split('.');

  if (parcalar.length < 2) {
    return true;
  }

  return parcalar[1].length <= 2;

}


export function tutarBicimGecerliMi(
  tutar: number
): boolean {

  return (
    tutar <= TUTAR_UST_SINIRI &&
    ondalikBasamakSayisiGecerliMi(tutar)
  );

}


export function tutarBicimHataMesajiGetir(
  tutar: number
): string {

  if (tutar > TUTAR_UST_SINIRI) {
    return 'Tutar izin verilen üst sınırı aşmaktadır.';
  }

  if (!ondalikBasamakSayisiGecerliMi(tutar)) {
    return 'Tutar en fazla iki ondalık basamak içerebilir.';
  }

  return '';

}
