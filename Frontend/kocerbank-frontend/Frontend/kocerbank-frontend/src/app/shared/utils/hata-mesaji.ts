/*
 * HttpErrorResponse'tan (veya benzeri bir hata nesnesinden)
 * kullanıcıya gösterilecek mesajı çıkarır. Backend hem düz
 * string hem de { mesaj } nesnesi dönebildiği için ikisini
 * de destekler.
 */
export function extractErrorMessage(
  hata: any,
  varsayilanMesaj: string
): string {

  if (typeof hata?.error === 'string') {
    return hata.error;
  }

  if (typeof hata?.error?.mesaj === 'string') {
    return hata.error.mesaj;
  }

  return varsayilanMesaj;
}
