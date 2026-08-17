/* ============================================================================
   TABLOLARI VE KOLON YAPILARINI İNCELEME
   ============================================================================ */

/* Müşteri kayıtlarını listeler ve tablonun kolon yapısını gösterir. */
SELECT * FROM KB_MUSTERIBILGILERI;
DESC KB_MUSTERIBILGILERI;

/* Müşterilere ait iletişim kayıtlarını ve tablonun yapısını gösterir. */
SELECT * FROM KB_MUSTERIILETISIM;
DESC KB_MUSTERIILETISIM;

/* Banka hesaplarını ve hesap tablosunun kolon yapısını gösterir. */
SELECT * FROM KB_HESAPBILGILERI;
DESC KB_HESAPBILGILERI;

/* Para transferi kayıtlarını ve transfer tablosunun yapısını gösterir. */
SELECT * FROM KB_PARATRANSFERI;
DESC KB_PARATRANSFERI;

/* Banka personellerini ve personel tablosunun yapısını gösterir. */
SELECT * FROM KB_PERSONEL;
DESC KB_PERSONEL;

/* Banka şubelerini ve şube tablosunun kolon yapısını gösterir. */
SELECT * FROM KB_SUBE;
DESC KB_SUBE;

/* Sistemde kullanılan parametrik kodları ve tablo yapısını gösterir. */
SELECT * FROM KB_PARAMETRE;
DESC KB_PARAMETRE;


/* ============================================================================
   TEMEL SELECT, WHERE VE ORDER BY SORGULARI
   ============================================================================ */

/* Aktif hesapları bakiyesi en yüksek hesaptan başlayarak listeler. */
SELECT HESAPADI,
       IBAN,
       BAKIYE,
       DOVIZCINSI
FROM KB_HESAPBILGILERI
WHERE HESAPDURUMKODU = 1
ORDER BY BAKIYE DESC;


/*
   Aktif, TL cinsindeki ve bakiyesi 10.000'den büyük,
   50.000'den küçük hesapları listeler.
   10.000 ve 50.000 sınır değerleri sonuca dahil değildir.
*/
SELECT *
FROM KB_HESAPBILGILERI
WHERE HESAPDURUMKODU = 1
  AND DOVIZCINSI = 1
  AND BAKIYE > 10000
  AND BAKIYE < 50000
ORDER BY BAKIYE DESC;


/*
   Blokeli olmayan ve TL dışında bir döviz cinsine sahip hesapları listeler.
   Mevcut kodlara göre aktif/pasif durumdaki USD ve EUR hesapları getirir.
*/
SELECT *
FROM KB_HESAPBILGILERI
WHERE HESAPDURUMKODU != 3
  AND DOVIZCINSI != 1
ORDER BY BAKIYE DESC;


/*
   Hesap adında büyük-küçük harf fark etmeksizin “maaş” geçen
   aktif hesapları listeler.
*/
SELECT *
FROM KB_HESAPBILGILERI
WHERE UPPER(HESAPADI) LIKE '%MAAŞ%'
  AND HESAPDURUMKODU = 1;


/* Hesaplarda kullanılan farklı döviz kodlarını tekrarsız listeler. */
SELECT DISTINCT DOVIZCINSI
FROM KB_HESAPBILGILERI
ORDER BY DOVIZCINSI;


/*
   Hesaplarda kullanılan döviz kodlarını tekrarsız listeler
   ve CASE kullanarak kodların açıklamalarını oluşturur.
*/
SELECT DISTINCT
       DOVIZCINSI,
       CASE DOVIZCINSI
           WHEN 1 THEN 'TL'
           WHEN 2 THEN 'USD'
           WHEN 3 THEN 'EUR'
       END AS DOVIZ_ADI
FROM KB_HESAPBILGILERI
ORDER BY DOVIZCINSI;


/* Sistemde bulunan aktif hesapların toplam sayısını hesaplar. */
SELECT COUNT(*) AS TOPLAM_AKTIF_HESAP_SAYISI
FROM KB_HESAPBILGILERI
WHERE HESAPDURUMKODU = 1;


/* Aktif hesapların sayısını döviz cinslerine göre ayrı ayrı hesaplar. */
SELECT DOVIZCINSI,
       COUNT(*) AS DOVIZ_BAZINDA_AKTIF_HESAP_SAYISI
FROM KB_HESAPBILGILERI
WHERE HESAPDURUMKODU = 1
GROUP BY DOVIZCINSI;


/*
   Her döviz cinsi için aktif hesap sayısını, toplam bakiyeyi
   ve ortalama bakiyeyi hesaplar.
   Sonuçları toplam bakiyeye göre büyükten küçüğe sıralar.
*/
SELECT DOVIZCINSI,
       COUNT(HESAPDURUMKODU) AS AKTIF_SAYISI,
       SUM(BAKIYE) AS TOPLAM_BAKIYE,
       AVG(BAKIYE) AS ORTALAMA_BAKIYE
FROM KB_HESAPBILGILERI
WHERE HESAPDURUMKODU = 1
GROUP BY DOVIZCINSI
ORDER BY TOPLAM_BAKIYE DESC;


/*
   Aktif hesapları döviz cinslerine göre gruplar.
   Yalnızca 10'dan fazla aktif hesaba sahip döviz gruplarını getirir.
*/
SELECT DOVIZCINSI,
       COUNT(*) AS AKTIF_HESAP_SAYISI
FROM KB_HESAPBILGILERI
WHERE HESAPDURUMKODU = 1
GROUP BY DOVIZCINSI
HAVING COUNT(*) > 10;


/*
   Aktif hesapları döviz cinslerine göre gruplar.
   Toplam bakiyesi 100.000'den yüksek olan döviz gruplarını getirir.
*/
SELECT DOVIZCINSI,
       COUNT(*) AS HESAP_SAYISI,
       SUM(BAKIYE) AS TOPLAM_BAKIYE
FROM KB_HESAPBILGILERI
WHERE HESAPDURUMKODU = 1
GROUP BY DOVIZCINSI
HAVING SUM(BAKIYE) > 100000;


/* ============================================================================
   JOIN SORGULARI
   ============================================================================ */

/*
   Hesapları müşterilerle birleştirir.
   LEFT JOIN kullanıldığı için müşteri eşleşmesi olmayan hesaplar da korunur.
*/
SELECT H.ID AS HESAP_ID,
       H.HESAPADI,
       H.IBAN,
       H.BAKIYE,
       M.AD,
       M.SOYAD
FROM KB_HESAPBILGILERI H
LEFT JOIN KB_MUSTERIBILGILERI M
       ON H.MUSTERIBILGILERIID = M.ID;


/*
   Hesap, müşteri ve şube tablolarını birleştirir.
   Her hesabın sahibini ve hesabın bağlı olduğu şubeyi gösterir.
*/
SELECT H.ID AS HESAP_ID,
       H.HESAPADI,
       H.IBAN,
       H.BAKIYE,
       M.AD || ' ' || M.SOYAD AS MUSTERI_ADI_SOYADI,
       S.SUBEADI
FROM KB_HESAPBILGILERI H
INNER JOIN KB_MUSTERIBILGILERI M
        ON H.MUSTERIBILGILERIID = M.ID
INNER JOIN KB_SUBE S
        ON H.SUBESUBEKODU = S.SUBEKODU;


/*
   Aktif hesapları müşteri ve şube bilgileriyle birlikte gösterir.
   Önce şube adına, aynı şubedeki hesapları bakiyeye göre sıralar.
*/
SELECT H.ID AS HESAP_ID,
       H.HESAPADI AS HESAP_ADI,
       H.IBAN,
       H.BAKIYE,
       M.AD || ' ' || M.SOYAD AS MUSTERI_ADI_SOYADI,
       S.SUBEADI
FROM KB_HESAPBILGILERI H
INNER JOIN KB_MUSTERIBILGILERI M
        ON H.MUSTERIBILGILERIID = M.ID
INNER JOIN KB_SUBE S
        ON H.SUBESUBEKODU = S.SUBEKODU
WHERE H.HESAPDURUMKODU = 1
ORDER BY S.SUBEADI, H.BAKIYE DESC;


/*
   Yalnızca en az bir aktif hesabı bulunan şubeleri gösterir.
   INNER JOIN nedeniyle aktif hesabı olmayan şubeler sonuçta bulunmaz.
*/
SELECT S.SUBEKODU,
       S.SUBEADI,
       COUNT(H.ID) AS AKTIF_HESAP_SAYISI
FROM KB_SUBE S
INNER JOIN KB_HESAPBILGILERI H
        ON S.SUBEKODU = H.SUBESUBEKODU
WHERE H.HESAPDURUMKODU = 1
GROUP BY S.SUBEKODU, S.SUBEADI
ORDER BY S.SUBEADI;


/*
   Bütün şubeleri ve her şubenin aktif hesap sayısını gösterir.
   Aktif hesabı olmayan şubeler LEFT JOIN sayesinde korunur
   ve COUNT(H.ID) sonucunda hesap sayıları 0 olarak gösterilir.
*/
SELECT S.SUBEKODU,
       S.SUBEADI,
       COUNT(H.ID) AS AKTIF_HESAP_SAYISI
FROM KB_SUBE S
LEFT JOIN KB_HESAPBILGILERI H
       ON S.SUBEKODU = H.SUBESUBEKODU
      AND H.HESAPDURUMKODU = 1
GROUP BY S.SUBEKODU, S.SUBEADI
ORDER BY S.SUBEADI;

/*
Hiç hesabı bulunmayan müşterileri listele. Sonuçta müşteri ID, ad ve soyad bulunsun.
*/
SELECT M.ID AS MUSTERI_ID, M.AD AS MUSTERI_AD, M.SOYAD AS MUSTERI_SOYAD, COUNT(H.ID) AS HESAP_SAYISI
FROM KB_MUSTERIBILGILERI M
LEFT JOIN KB_HESAPBILGILERI H
       ON M.ID = H.MUSTERIBILGILERIID
GROUP BY M.ID, M.AD, M.SOYAD
HAVING COUNT(H.ID) = 0;

 
