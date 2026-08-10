-- KB_PARATRANSFERI ve KB_HESAPHAREKETI test verisi
-- Üretilen kayıtlar:
--   100 transfer dışı hesap hareketi (50 yatırma + 50 çekme)
--   50 havale + 50 virman = 100 para transferi
--   Her transfer için 2 hareket = 200 transfer hareketi
--   Toplam yeni hesap hareketi = 300
--
-- Bu dosya, aşağıdaki prosedürleri kullanır:
--   KB_HESAP_CEK_YATIR
--   KB_PARA_TRANSFERI_YAP
--
-- Önce hesap_bilgileri_test_100.sql ve hesap_bilgileri_ek_50.sql çalıştırılmalıdır.

SET SERVEROUTPUT ON;

DECLARE
    V_AKTIF_HESAP_SAYISI       NUMBER;
    V_VIRMAN_MUSTERI_SAYISI    NUMBER;
    V_HESAP_ID                 KB_HESAPBILGILERI.ID%TYPE;
    V_GONDEREN_ID              KB_HESAPBILGILERI.ID%TYPE;
    V_ALICI_ID                 KB_HESAPBILGILERI.ID%TYPE;
    V_GONDEREN_MUSTERI_ID      KB_HESAPBILGILERI.MUSTERIBILGILERIID%TYPE;
    V_ALICI_MUSTERI_ID         KB_HESAPBILGILERI.MUSTERIBILGILERIID%TYPE;
    V_GONDEREN_DOVIZ           KB_HESAPBILGILERI.DOVIZCINSI%TYPE;
    V_ALICI_DOVIZ              KB_HESAPBILGILERI.DOVIZCINSI%TYPE;
    V_GONDEREN_BAKIYE          KB_HESAPBILGILERI.BAKIYE%TYPE;
    V_ISLEM_TIPI               NUMBER;
    V_TUTAR                    NUMBER(14,2);
    V_KUR                      NUMBER(18,8);
    V_HAREKET_ID               NUMBER;
    V_YENI_BAKIYE              NUMBER;
    V_TRANSFER_ID              NUMBER;
    V_GONDEREN_HAREKET_ID      NUMBER;
    V_ALICI_HAREKET_ID         NUMBER;
    V_GONDEREN_YENI_BAKIYE     NUMBER;
    V_ALICI_YENI_BAKIYE        NUMBER;
    V_ALICI_TUTAR              NUMBER;
    V_BASLANGIC_TRANSFER       NUMBER;
    V_BASLANGIC_HAREKET        NUMBER;
    V_BITIS_TRANSFER           NUMBER;
    V_BITIS_HAREKET            NUMBER;
    V_ALICI_SIRA               NUMBER;

    FUNCTION KUR_GETIR(P_GONDEREN NUMBER, P_ALICI NUMBER)
        RETURN NUMBER
    IS
    BEGIN
        IF P_GONDEREN = P_ALICI THEN RETURN 1;
        ELSIF P_GONDEREN = 1 AND P_ALICI = 2 THEN RETURN 0.03000000;
        ELSIF P_GONDEREN = 1 AND P_ALICI = 3 THEN RETURN 0.02500000;
        ELSIF P_GONDEREN = 2 AND P_ALICI = 1 THEN RETURN 33.33333333;
        ELSIF P_GONDEREN = 2 AND P_ALICI = 3 THEN RETURN 0.83333333;
        ELSIF P_GONDEREN = 3 AND P_ALICI = 1 THEN RETURN 40.00000000;
        ELSIF P_GONDEREN = 3 AND P_ALICI = 2 THEN RETURN 1.20000000;
        ELSE
            RAISE_APPLICATION_ERROR(-20200, 'Desteklenmeyen döviz çifti.');
        END IF;
    END;

    PROCEDURE AKTIF_HESAP_GETIR(
        P_SIRA       IN NUMBER,
        P_HESAP_ID   OUT NUMBER,
        P_MUSTERI_ID OUT NUMBER,
        P_DOVIZ      OUT NUMBER,
        P_BAKIYE     OUT NUMBER
    )
    IS
    BEGIN
        SELECT ID, MUSTERIBILGILERIID, DOVIZCINSI, BAKIYE
          INTO P_HESAP_ID, P_MUSTERI_ID, P_DOVIZ, P_BAKIYE
          FROM (
                SELECT H.ID,
                       H.MUSTERIBILGILERIID,
                       H.DOVIZCINSI,
                       H.BAKIYE,
                       ROW_NUMBER() OVER (ORDER BY H.ID) AS RN
                  FROM KB_HESAPBILGILERI H
                 WHERE H.HESAPDURUMKODU = 1
               )
         WHERE RN = MOD(P_SIRA - 1, V_AKTIF_HESAP_SAYISI) + 1;
    END;
BEGIN
    SELECT COUNT(*) INTO V_BASLANGIC_TRANSFER FROM KB_PARATRANSFERI;
    SELECT COUNT(*) INTO V_BASLANGIC_HAREKET FROM KB_HESAPHAREKETI;

    SELECT COUNT(*)
      INTO V_AKTIF_HESAP_SAYISI
      FROM KB_HESAPBILGILERI
     WHERE HESAPDURUMKODU = 1;

    IF V_AKTIF_HESAP_SAYISI < 2 THEN
        RAISE_APPLICATION_ERROR(-20201, 'En az iki aktif hesap gereklidir.');
    END IF;

    SELECT COUNT(*)
      INTO V_VIRMAN_MUSTERI_SAYISI
      FROM (
            SELECT MUSTERIBILGILERIID
              FROM KB_HESAPBILGILERI
             WHERE HESAPDURUMKODU = 1
             GROUP BY MUSTERIBILGILERIID
            HAVING COUNT(*) >= 2
           );

    IF V_VIRMAN_MUSTERI_SAYISI < 50 THEN
        RAISE_APPLICATION_ERROR(
            -20202,
            '50 virman için ikişer aktif hesabı olan 50 müşteri bulunmalıdır.'
        );
    END IF;

    /* 1) 100 TRANSFER DIŞI HAREKET */
    FOR I IN 1..100 LOOP
        AKTIF_HESAP_GETIR(
            P_SIRA       => I,
            P_HESAP_ID   => V_HESAP_ID,
            P_MUSTERI_ID => V_GONDEREN_MUSTERI_ID,
            P_DOVIZ      => V_GONDEREN_DOVIZ,
            P_BAKIYE     => V_GONDEREN_BAKIYE
        );

        V_ISLEM_TIPI := CASE WHEN MOD(I, 2) = 1 THEN 1 ELSE 2 END;
        V_TUTAR := ROUND(25 + I * 3.25, 2);

        IF V_ISLEM_TIPI = 2 AND V_GONDEREN_BAKIYE < V_TUTAR THEN
            V_TUTAR := ROUND(V_GONDEREN_BAKIYE / 10, 2);
        END IF;

        KB_HESAP_CEK_YATIR(
            P_HESAPID    => V_HESAP_ID,
            P_ISLEMTIPI  => V_ISLEM_TIPI,
            P_TUTAR      => V_TUTAR,
            P_RECORDUSER => 'SEED',
            P_HAREKETID  => V_HAREKET_ID,
            P_YENIBAKIYE => V_YENI_BAKIYE
        );
    END LOOP;

    /* 2) 50 HAVALE: FARKLI MÜŞTERİLERİN AKTİF HESAPLARI */
    FOR I IN 1..50 LOOP
        AKTIF_HESAP_GETIR(
            P_SIRA       => I,
            P_HESAP_ID   => V_GONDEREN_ID,
            P_MUSTERI_ID => V_GONDEREN_MUSTERI_ID,
            P_DOVIZ      => V_GONDEREN_DOVIZ,
            P_BAKIYE     => V_GONDEREN_BAKIYE
        );

        V_ALICI_SIRA := I + 67;

        AKTIF_HESAP_GETIR(
            P_SIRA       => V_ALICI_SIRA,
            P_HESAP_ID   => V_ALICI_ID,
            P_MUSTERI_ID => V_ALICI_MUSTERI_ID,
            P_DOVIZ      => V_ALICI_DOVIZ,
            P_BAKIYE     => V_YENI_BAKIYE
        );

        WHILE V_ALICI_ID = V_GONDEREN_ID
           OR V_ALICI_MUSTERI_ID = V_GONDEREN_MUSTERI_ID LOOP
            V_ALICI_SIRA := V_ALICI_SIRA + 1;

            AKTIF_HESAP_GETIR(
                P_SIRA       => V_ALICI_SIRA,
                P_HESAP_ID   => V_ALICI_ID,
                P_MUSTERI_ID => V_ALICI_MUSTERI_ID,
                P_DOVIZ      => V_ALICI_DOVIZ,
                P_BAKIYE     => V_YENI_BAKIYE
            );
        END LOOP;

        V_TUTAR := ROUND(40 + I * 4.50, 2);
        IF V_GONDEREN_BAKIYE < V_TUTAR THEN
            V_TUTAR := ROUND(V_GONDEREN_BAKIYE / 10, 2);
        END IF;
        V_KUR := KUR_GETIR(V_GONDEREN_DOVIZ, V_ALICI_DOVIZ);

        KB_PARA_TRANSFERI_YAP(
            P_GONDERENHESAPID    => V_GONDEREN_ID,
            P_ALICIHESAPID       => V_ALICI_ID,
            P_TRANSFERTIPI       => 1,
            P_GONDERENTUTAR      => V_TUTAR,
            P_DOVIZKURU          => V_KUR,
            P_ACIKLAMA           => 'Test havalesi ' || LPAD(I, 3, '0'),
            P_RECORDUSER         => 'SEED',
            P_TRANSFERID         => V_TRANSFER_ID,
            P_GONDERENHAREKETID  => V_GONDEREN_HAREKET_ID,
            P_ALICIHAREKETID     => V_ALICI_HAREKET_ID,
            P_GONDERENYENIBAKIYE => V_GONDEREN_YENI_BAKIYE,
            P_ALICIYENIBAKIYE    => V_ALICI_YENI_BAKIYE,
            P_ALICITUTAR         => V_ALICI_TUTAR
        );
    END LOOP;

    /* 3) 50 VİRMAN: AYNI MÜŞTERİNİN İKİ AKTİF HESABI */
    FOR R IN (
        SELECT MUSTERIBILGILERIID
          FROM (
                SELECT MUSTERIBILGILERIID,
                       MIN(ID) AS SIRALAMA_ID
                  FROM KB_HESAPBILGILERI
                 WHERE HESAPDURUMKODU = 1
                 GROUP BY MUSTERIBILGILERIID
                HAVING COUNT(*) >= 2
                 ORDER BY SIRALAMA_ID
               )
         WHERE ROWNUM <= 50
    ) LOOP
        SELECT ID, DOVIZCINSI, BAKIYE
          INTO V_GONDEREN_ID, V_GONDEREN_DOVIZ, V_GONDEREN_BAKIYE
          FROM (
                SELECT ID, DOVIZCINSI, BAKIYE,
                       ROW_NUMBER() OVER (ORDER BY ID) AS RN
                  FROM KB_HESAPBILGILERI
                 WHERE MUSTERIBILGILERIID = R.MUSTERIBILGILERIID
                   AND HESAPDURUMKODU = 1
               )
         WHERE RN = 1;

        SELECT ID, DOVIZCINSI
          INTO V_ALICI_ID, V_ALICI_DOVIZ
          FROM (
                SELECT ID, DOVIZCINSI,
                       ROW_NUMBER() OVER (ORDER BY ID) AS RN
                  FROM KB_HESAPBILGILERI
                 WHERE MUSTERIBILGILERIID = R.MUSTERIBILGILERIID
                   AND HESAPDURUMKODU = 1
               )
         WHERE RN = 2;

        V_TUTAR := 125.00;
        IF V_GONDEREN_BAKIYE < V_TUTAR THEN
            V_TUTAR := ROUND(V_GONDEREN_BAKIYE / 10, 2);
        END IF;
        V_KUR := KUR_GETIR(V_GONDEREN_DOVIZ, V_ALICI_DOVIZ);

        KB_PARA_TRANSFERI_YAP(
            P_GONDERENHESAPID    => V_GONDEREN_ID,
            P_ALICIHESAPID       => V_ALICI_ID,
            P_TRANSFERTIPI       => 2,
            P_GONDERENTUTAR      => V_TUTAR,
            P_DOVIZKURU          => V_KUR,
            P_ACIKLAMA           => 'Test virmanı ' || R.MUSTERIBILGILERIID,
            P_RECORDUSER         => 'SEED',
            P_TRANSFERID         => V_TRANSFER_ID,
            P_GONDERENHAREKETID  => V_GONDEREN_HAREKET_ID,
            P_ALICIHAREKETID     => V_ALICI_HAREKET_ID,
            P_GONDERENYENIBAKIYE => V_GONDEREN_YENI_BAKIYE,
            P_ALICIYENIBAKIYE    => V_ALICI_YENI_BAKIYE,
            P_ALICITUTAR         => V_ALICI_TUTAR
        );
    END LOOP;

    SELECT COUNT(*) INTO V_BITIS_TRANSFER FROM KB_PARATRANSFERI;
    SELECT COUNT(*) INTO V_BITIS_HAREKET FROM KB_HESAPHAREKETI;

    IF V_BITIS_TRANSFER - V_BASLANGIC_TRANSFER <> 100 THEN
        RAISE_APPLICATION_ERROR(-20203, 'Beklenen 100 transfer oluşmadı.');
    END IF;

    IF V_BITIS_HAREKET - V_BASLANGIC_HAREKET <> 300 THEN
        RAISE_APPLICATION_ERROR(-20204, 'Beklenen 300 hesap hareketi oluşmadı.');
    END IF;

    COMMIT;

    DBMS_OUTPUT.PUT_LINE('Eklenen transfer: '
        || (V_BITIS_TRANSFER - V_BASLANGIC_TRANSFER));
    DBMS_OUTPUT.PUT_LINE('Eklenen hesap hareketi: '
        || (V_BITIS_HAREKET - V_BASLANGIC_HAREKET));
EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        RAISE;
END;
/

-- Sonuç kontrolü
SELECT TRANSFERTIPI, COUNT(*) AS TRANSFER_SAYISI
FROM KB_PARATRANSFERI
WHERE RECORDUSER = 'SEED'
GROUP BY TRANSFERTIPI
ORDER BY TRANSFERTIPI;

SELECT
    HAREKETTIPI,
    CASE WHEN PARATRANSFERIID IS NULL
         THEN 'TRANSFER_DISI'
         ELSE 'TRANSFER_BAGLI'
    END AS BAGLANTI,
    COUNT(*) AS HAREKET_SAYISI
FROM KB_HESAPHAREKETI
WHERE RECORDUSER = 'SEED'
GROUP BY
    HAREKETTIPI,
    CASE WHEN PARATRANSFERIID IS NULL
         THEN 'TRANSFER_DISI'
         ELSE 'TRANSFER_BAGLI'
    END
ORDER BY HAREKETTIPI;

select * from kb_paratransferi;

select * from kb_hesapbilgileri where id = 123;

select * from kb_musteribilgileri where id = 122;
