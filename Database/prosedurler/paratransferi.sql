CREATE OR REPLACE PROCEDURE KB_PARA_TRANSFERI_YAP
(
    P_GONDERENHESAPID       IN  KB_HESAPBILGILERI.ID%TYPE,
    P_ALICIHESAPID          IN  KB_HESAPBILGILERI.ID%TYPE,
    P_TRANSFERTIPI          IN  KB_PARATRANSFERI.TRANSFERTIPI%TYPE,
    P_GONDERENTUTAR         IN  KB_PARATRANSFERI.GONDERENTUTAR%TYPE,
    P_DOVIZKURU             IN  KB_PARATRANSFERI.DOVIZKURU%TYPE,
    P_ACIKLAMA              IN  KB_PARATRANSFERI.ACIKLAMA%TYPE,
    P_RECORDUSER            IN  KB_PARATRANSFERI.RECORDUSER%TYPE,

    P_TRANSFERID            OUT KB_PARATRANSFERI.ID%TYPE,
    P_GONDERENHAREKETID     OUT KB_HESAPHAREKETI.ID%TYPE,
    P_ALICIHAREKETID        OUT KB_HESAPHAREKETI.ID%TYPE,
    P_GONDERENYENIBAKIYE    OUT KB_HESAPBILGILERI.BAKIYE%TYPE,
    P_ALICIYENIBAKIYE       OUT KB_HESAPBILGILERI.BAKIYE%TYPE,
    P_ALICITUTAR            OUT KB_HESAPHAREKETI.TUTAR%TYPE
)
AS
    /* Gönderen hesap bilgileri */

    V_GONDERENONCEKIBAKIYE
        KB_HESAPBILGILERI.BAKIYE%TYPE;

    V_GONDERENDOVIZTIPI
        KB_HESAPBILGILERI.DOVIZCINSI%TYPE;

    V_GONDERENDURUMKODU
        KB_HESAPBILGILERI.HESAPDURUMKODU%TYPE;

    V_GONDERENMUSTERIID
        KB_HESAPBILGILERI.MUSTERIBILGILERIID%TYPE;


    /* Alıcı hesap bilgileri */

    V_ALICIONCEKIBAKIYE
        KB_HESAPBILGILERI.BAKIYE%TYPE;

    V_ALICIDOVIZTIPI
        KB_HESAPBILGILERI.DOVIZCINSI%TYPE;

    V_ALICIDURUMKODU
        KB_HESAPBILGILERI.HESAPDURUMKODU%TYPE;

    V_ALICIMUSTERIID
        KB_HESAPBILGILERI.MUSTERIBILGILERIID%TYPE;


    /* İşlem sonucunda oluşacak değerler */

    V_GONDERENYENIBAKIYE
        KB_HESAPBILGILERI.BAKIYE%TYPE;

    V_ALICIYENIBAKIYE
        KB_HESAPBILGILERI.BAKIYE%TYPE;

    V_ALICITUTAR
        KB_HESAPHAREKETI.TUTAR%TYPE;

    V_BULUNANHESAPSAYISI NUMBER := 0;

    V_RECORDUSER KB_PARATRANSFERI.RECORDUSER%TYPE;
    V_RECORDDATE KB_PARATRANSFERI.RECORDDATE%TYPE;

BEGIN

    KB_AUDIT_BILGI_HAZIRLA
    (
        P_RECORDUSER       => P_RECORDUSER,
        P_DUZENLENMIS_USER => V_RECORDUSER,
        P_RECORDDATE       => V_RECORDDATE
    );

    /* 1. TEMEL PARAMETRE KONTROLLERİ */

    IF P_GONDERENHESAPID IS NULL
       OR P_GONDERENHESAPID <= 0 THEN

        RAISE_APPLICATION_ERROR(
            -20101,
            'Geçersiz gönderen hesap ID.'
        );

    END IF;


    IF P_ALICIHESAPID IS NULL
       OR P_ALICIHESAPID <= 0 THEN

        RAISE_APPLICATION_ERROR(
            -20102,
            'Geçersiz alıcı hesap ID.'
        );

    END IF;


    IF P_GONDERENHESAPID = P_ALICIHESAPID THEN

        RAISE_APPLICATION_ERROR(
            -20103,
            'Gönderen ve alıcı hesap aynı olamaz.'
        );

    END IF;


    IF P_TRANSFERTIPI IS NULL
       OR P_TRANSFERTIPI NOT IN (1, 2) THEN

        RAISE_APPLICATION_ERROR(
            -20104,
            'Transfer tipi havale veya virman olmalıdır.'
        );

    END IF;


    IF P_GONDERENTUTAR IS NULL
       OR P_GONDERENTUTAR <= 0 THEN

        RAISE_APPLICATION_ERROR(
            -20105,
            'Gönderen tutar sıfırdan büyük olmalıdır.'
        );

    END IF;


    IF P_DOVIZKURU IS NULL
       OR P_DOVIZKURU <= 0 THEN

        RAISE_APPLICATION_ERROR(
            -20106,
            'Döviz kuru sıfırdan büyük olmalıdır.'
        );

    END IF;


    IF P_ACIKLAMA IS NOT NULL
       AND LENGTH(TRIM(P_ACIKLAMA)) > 100 THEN

        RAISE_APPLICATION_ERROR(
            -20107,
            'Açıklama en fazla 100 karakter olabilir.'
        );

    END IF;




    /*
        2. HESAPLARI KİLİTLEYEREK GETİR

        ORDER BY ID sayesinde hesaplar her zaman aynı sırada
        kilitlenir. Bu durum eş zamanlı transferlerde deadlock
        riskini azaltır.
    */

    FOR R_HESAP IN
    (
        SELECT
            ID,
            BAKIYE,
            DOVIZCINSI,
            HESAPDURUMKODU,
            MUSTERIBILGILERIID
        FROM KB_HESAPBILGILERI
        WHERE ID IN
        (
            P_GONDERENHESAPID,
            P_ALICIHESAPID
        )
        ORDER BY ID
        FOR UPDATE
    )
    LOOP

        V_BULUNANHESAPSAYISI :=
            V_BULUNANHESAPSAYISI + 1;

        IF R_HESAP.ID = P_GONDERENHESAPID THEN

            V_GONDERENONCEKIBAKIYE :=
                R_HESAP.BAKIYE;

            V_GONDERENDOVIZTIPI :=
                R_HESAP.DOVIZCINSI;

            V_GONDERENDURUMKODU :=
                R_HESAP.HESAPDURUMKODU;

            V_GONDERENMUSTERIID :=
                R_HESAP.MUSTERIBILGILERIID;

        ELSE

            V_ALICIONCEKIBAKIYE :=
                R_HESAP.BAKIYE;

            V_ALICIDOVIZTIPI :=
                R_HESAP.DOVIZCINSI;

            V_ALICIDURUMKODU :=
                R_HESAP.HESAPDURUMKODU;

            V_ALICIMUSTERIID :=
                R_HESAP.MUSTERIBILGILERIID;

        END IF;

    END LOOP;


    IF V_BULUNANHESAPSAYISI <> 2 THEN

        IF V_GONDERENONCEKIBAKIYE IS NULL THEN

            RAISE_APPLICATION_ERROR(
                -20109,
                'Gönderen hesap bulunamadı.'
            );

        ELSE

            RAISE_APPLICATION_ERROR(
                -20110,
                'Alıcı hesap bulunamadı.'
            );

        END IF;

    END IF;


    /* 3. HESAP DURUM KONTROLLERİ */

    IF V_GONDERENDURUMKODU <> 1 THEN

        RAISE_APPLICATION_ERROR(
            -20111,
            'Gönderen hesap aktif değildir.'
        );

    END IF;


    IF V_ALICIDURUMKODU <> 1 THEN

        RAISE_APPLICATION_ERROR(
            -20112,
            'Alıcı hesap aktif değildir.'
        );

    END IF;


    /* 4. BAKİYE KONTROLÜ */

    IF V_GONDERENONCEKIBAKIYE < P_GONDERENTUTAR THEN

        RAISE_APPLICATION_ERROR(
            -20113,
            'Gönderen hesap bakiyesi yetersizdir.'
        );

    END IF;


    /* 5. HAVALE KONTROLÜ */

    IF P_TRANSFERTIPI = 1
       AND V_GONDERENMUSTERIID = V_ALICIMUSTERIID THEN

        RAISE_APPLICATION_ERROR(
            -20114,
            'Aynı müşterinin hesapları arasında havale yapılamaz. Virman seçiniz.'
        );

    END IF;


    /* 6. VİRMAN KONTROLÜ */

    IF P_TRANSFERTIPI = 2
       AND V_GONDERENMUSTERIID <> V_ALICIMUSTERIID THEN

        RAISE_APPLICATION_ERROR(
            -20115,
            'Farklı müşterilerin hesapları arasında virman yapılamaz.'
        );

    END IF;


    /* 7. DÖVİZ KURU VE ALICI TUTARI */

    IF V_GONDERENDOVIZTIPI = V_ALICIDOVIZTIPI THEN

        IF P_DOVIZKURU <> 1 THEN

            RAISE_APPLICATION_ERROR(
                -20116,
                'Aynı döviz cinsleri arasındaki kur 1 olmalıdır.'
            );

        END IF;

        V_ALICITUTAR := P_GONDERENTUTAR;

    ELSE

        V_ALICITUTAR :=
            ROUND(
                P_GONDERENTUTAR * P_DOVIZKURU,
                2
            );

    END IF;


    IF V_ALICITUTAR <= 0 THEN

        RAISE_APPLICATION_ERROR(
            -20117,
            'Kur dönüşümü sonucunda alıcı tutarı sıfırdan büyük olmalıdır.'
        );

    END IF;


    /* 8. YENİ BAKİYELERİ HESAPLA */

    V_GONDERENYENIBAKIYE :=
        V_GONDERENONCEKIBAKIYE
        - P_GONDERENTUTAR;

    V_ALICIYENIBAKIYE :=
        V_ALICIONCEKIBAKIYE
        + V_ALICITUTAR;


    IF V_GONDERENYENIBAKIYE < 0 THEN

        RAISE_APPLICATION_ERROR(
            -20118,
            'Transfer sonucunda gönderen bakiyesi negatif olamaz.'
        );

    END IF;


    /* 9. GÖNDEREN BAKİYESİNİ GÜNCELLE */

    UPDATE KB_HESAPBILGILERI
    SET
        BAKIYE = V_GONDERENYENIBAKIYE
    WHERE ID = P_GONDERENHESAPID;


    /* 10. ALICI BAKİYESİNİ GÜNCELLE */

    UPDATE KB_HESAPBILGILERI
    SET
        BAKIYE = V_ALICIYENIBAKIYE
    WHERE ID = P_ALICIHESAPID;


    /* 11. PARA TRANSFERİ KAYDINI OLUŞTUR */

    INSERT INTO KB_PARATRANSFERI
    (
        GONDERENHESAPID,
        ALICIHESAPID,
        TRANSFERTIPI,
        GONDERENTUTAR,
        GONDERENDOVIZTIPI,
        ALICIDOVIZTIPI,
        DOVIZKURU,
        ACIKLAMA,
        TARIHSAAT,
        RECORDUSER,
        RECORDDATE
    )
    VALUES
    (
        P_GONDERENHESAPID,
        P_ALICIHESAPID,
        P_TRANSFERTIPI,
        P_GONDERENTUTAR,
        V_GONDERENDOVIZTIPI,
        V_ALICIDOVIZTIPI,
        P_DOVIZKURU,
        NULLIF(TRIM(P_ACIKLAMA), ''),
        V_RECORDDATE,
        V_RECORDUSER,
        V_RECORDDATE
    )
    RETURNING ID INTO P_TRANSFERID;


    /* 12. GÖNDEREN HESAP HAREKETİ */

    INSERT INTO KB_HESAPHAREKETI
    (
        HESAPBILGILERIID,
        PARATRANSFERIID,
        HAREKETTIPI,
        TUTAR,
        DOVIZCINSI,
        ONCEKIBAKIYE,
        SONRAKIBAKIYE,
        ISLEMTARIHI,
        RECORDUSER,
        RECORDDATE
    )
    VALUES
    (
        P_GONDERENHESAPID,
        P_TRANSFERID,
        4,
        P_GONDERENTUTAR,
        V_GONDERENDOVIZTIPI,
        V_GONDERENONCEKIBAKIYE,
        V_GONDERENYENIBAKIYE,
        V_RECORDDATE,
        V_RECORDUSER,
        V_RECORDDATE
    )
    RETURNING ID INTO P_GONDERENHAREKETID;


    /* 13. ALICI HESAP HAREKETİ */

    INSERT INTO KB_HESAPHAREKETI
    (
        HESAPBILGILERIID,
        PARATRANSFERIID,
        HAREKETTIPI,
        TUTAR,
        DOVIZCINSI,
        ONCEKIBAKIYE,
        SONRAKIBAKIYE,
        ISLEMTARIHI,
        RECORDUSER,
        RECORDDATE
    )
    VALUES
    (
        P_ALICIHESAPID,
        P_TRANSFERID,
        3,
        V_ALICITUTAR,
        V_ALICIDOVIZTIPI,
        V_ALICIONCEKIBAKIYE,
        V_ALICIYENIBAKIYE,
        V_RECORDDATE,
        V_RECORDUSER,
        V_RECORDDATE
    )
    RETURNING ID INTO P_ALICIHAREKETID;


    /* 14. SONUÇLARI DIŞARI AKTAR */

    P_GONDERENYENIBAKIYE :=
        V_GONDERENYENIBAKIYE;

    P_ALICIYENIBAKIYE :=
        V_ALICIYENIBAKIYE;

    P_ALICITUTAR :=
        V_ALICITUTAR;

END KB_PARA_TRANSFERI_YAP;
/


/*
    KB_EFT_TRANSFERI_YAP

    Havale/EFT ekranında alıcı IBAN'ı bizim
    bankamızda kayıtlı değilse kullanılır. Alıcının
    bizim sistemimizde bir hesabı olmadığı için
    yalnızca gönderenin hesabı güncellenir; alıcı
    IBAN'ı ve kullanıcının girdiği ad-soyad bilgisi
    yalnızca KB_PARATRANSFERI kaydına metin olarak
    yazılır.
*/

CREATE OR REPLACE PROCEDURE KB_EFT_TRANSFERI_YAP
(
    P_GONDERENHESAPID       IN  KB_HESAPBILGILERI.ID%TYPE,
    P_ALICIIBAN             IN  KB_PARATRANSFERI.ALICIIBAN%TYPE,
    P_ALICIADSOYAD          IN  KB_PARATRANSFERI.ALICIADSOYAD%TYPE,
    P_GONDERENTUTAR         IN  KB_PARATRANSFERI.GONDERENTUTAR%TYPE,
    P_ACIKLAMA              IN  KB_PARATRANSFERI.ACIKLAMA%TYPE,
    P_RECORDUSER            IN  KB_PARATRANSFERI.RECORDUSER%TYPE,

    P_TRANSFERID            OUT KB_PARATRANSFERI.ID%TYPE,
    P_GONDERENHAREKETID     OUT KB_HESAPHAREKETI.ID%TYPE,
    P_GONDERENYENIBAKIYE    OUT KB_HESAPBILGILERI.BAKIYE%TYPE
)
AS
    /* Gönderen hesap bilgileri */

    V_GONDERENONCEKIBAKIYE
        KB_HESAPBILGILERI.BAKIYE%TYPE;

    V_GONDERENDOVIZTIPI
        KB_HESAPBILGILERI.DOVIZCINSI%TYPE;

    V_GONDERENDURUMKODU
        KB_HESAPBILGILERI.HESAPDURUMKODU%TYPE;

    V_GONDERENYENIBAKIYE
        KB_HESAPBILGILERI.BAKIYE%TYPE;

    V_RECORDUSER KB_PARATRANSFERI.RECORDUSER%TYPE;
    V_RECORDDATE KB_PARATRANSFERI.RECORDDATE%TYPE;

BEGIN

    KB_AUDIT_BILGI_HAZIRLA
    (
        P_RECORDUSER       => P_RECORDUSER,
        P_DUZENLENMIS_USER => V_RECORDUSER,
        P_RECORDDATE       => V_RECORDDATE
    );

    /* 1. TEMEL PARAMETRE KONTROLLERİ */

    IF P_GONDERENHESAPID IS NULL
       OR P_GONDERENHESAPID <= 0 THEN

        RAISE_APPLICATION_ERROR(
            -20201,
            'Geçersiz gönderen hesap ID.'
        );

    END IF;


    IF P_ALICIIBAN IS NULL
       OR LENGTH(TRIM(P_ALICIIBAN)) = 0 THEN

        RAISE_APPLICATION_ERROR(
            -20202,
            'Alıcı IBAN girilmesi zorunludur.'
        );

    END IF;


    IF P_GONDERENTUTAR IS NULL
       OR P_GONDERENTUTAR <= 0 THEN

        RAISE_APPLICATION_ERROR(
            -20203,
            'Gönderen tutar sıfırdan büyük olmalıdır.'
        );

    END IF;


    IF P_ACIKLAMA IS NOT NULL
       AND LENGTH(TRIM(P_ACIKLAMA)) > 100 THEN

        RAISE_APPLICATION_ERROR(
            -20204,
            'Açıklama en fazla 100 karakter olabilir.'
        );

    END IF;


    /* 2. GÖNDEREN HESABI KİLİTLEYEREK GETİR */

    BEGIN

        SELECT
            BAKIYE,
            DOVIZCINSI,
            HESAPDURUMKODU
        INTO
            V_GONDERENONCEKIBAKIYE,
            V_GONDERENDOVIZTIPI,
            V_GONDERENDURUMKODU
        FROM KB_HESAPBILGILERI
        WHERE ID = P_GONDERENHESAPID
        FOR UPDATE;

    EXCEPTION

        WHEN NO_DATA_FOUND THEN

            RAISE_APPLICATION_ERROR(
                -20205,
                'Gönderen hesap bulunamadı.'
            );

    END;


    /* 3. GÖNDEREN HESAP DURUM VE DÖVİZ KONTROLÜ */

    IF V_GONDERENDURUMKODU <> 1 THEN

        RAISE_APPLICATION_ERROR(
            -20206,
            'Gönderen hesap aktif değildir.'
        );

    END IF;


    IF V_GONDERENDOVIZTIPI <> 1 THEN

        RAISE_APPLICATION_ERROR(
            -20207,
            'EFT işlemi yalnızca TL hesaplardan yapılabilir.'
        );

    END IF;


    /* 4. BAKİYE KONTROLÜ */

    IF V_GONDERENONCEKIBAKIYE < P_GONDERENTUTAR THEN

        RAISE_APPLICATION_ERROR(
            -20208,
            'Gönderen hesap bakiyesi yetersizdir.'
        );

    END IF;


    /* 5. YENİ BAKİYEYİ HESAPLA VE GÜNCELLE */

    V_GONDERENYENIBAKIYE :=
        V_GONDERENONCEKIBAKIYE
        - P_GONDERENTUTAR;


    UPDATE KB_HESAPBILGILERI
    SET
        BAKIYE = V_GONDERENYENIBAKIYE
    WHERE ID = P_GONDERENHESAPID;


    /* 6. PARA TRANSFERİ KAYDINI OLUŞTUR */

    INSERT INTO KB_PARATRANSFERI
    (
        GONDERENHESAPID,
        ALICIHESAPID,
        ALICIIBAN,
        ALICIADSOYAD,
        TRANSFERTIPI,
        GONDERENTUTAR,
        GONDERENDOVIZTIPI,
        ALICIDOVIZTIPI,
        DOVIZKURU,
        ACIKLAMA,
        TARIHSAAT,
        RECORDUSER,
        RECORDDATE
    )
    VALUES
    (
        P_GONDERENHESAPID,
        NULL,
        UPPER(TRIM(P_ALICIIBAN)),
        NULLIF(TRIM(P_ALICIADSOYAD), ''),
        3,
        P_GONDERENTUTAR,
        V_GONDERENDOVIZTIPI,
        V_GONDERENDOVIZTIPI,
        1,
        NULLIF(TRIM(P_ACIKLAMA), ''),
        V_RECORDDATE,
        V_RECORDUSER,
        V_RECORDDATE
    )
    RETURNING ID INTO P_TRANSFERID;


    /* 7. GÖNDEREN HESAP HAREKETİ */

    INSERT INTO KB_HESAPHAREKETI
    (
        HESAPBILGILERIID,
        PARATRANSFERIID,
        HAREKETTIPI,
        TUTAR,
        DOVIZCINSI,
        ONCEKIBAKIYE,
        SONRAKIBAKIYE,
        ISLEMTARIHI,
        RECORDUSER,
        RECORDDATE
    )
    VALUES
    (
        P_GONDERENHESAPID,
        P_TRANSFERID,
        4,
        P_GONDERENTUTAR,
        V_GONDERENDOVIZTIPI,
        V_GONDERENONCEKIBAKIYE,
        V_GONDERENYENIBAKIYE,
        V_RECORDDATE,
        V_RECORDUSER,
        V_RECORDDATE
    )
    RETURNING ID INTO P_GONDERENHAREKETID;


    /* 8. SONUÇLARI DIŞARI AKTAR */

    P_GONDERENYENIBAKIYE :=
        V_GONDERENYENIBAKIYE;

END KB_EFT_TRANSFERI_YAP;
/


SELECT
    OBJECT_NAME,
    STATUS
FROM USER_OBJECTS
WHERE OBJECT_NAME IN ('KB_PARA_TRANSFERI_YAP', 'KB_EFT_TRANSFERI_YAP');

SELECT
    H.ID,
    H.HESAPADI,
    H.IBAN,
    H.BAKIYE,
    H.DOVIZCINSI,
    H.HESAPDURUMKODU,
    H.MUSTERIBILGILERIID,
    M.AD,
    M.SOYAD
FROM KB_HESAPBILGILERI H
INNER JOIN KB_MUSTERIBILGILERI M
    ON M.ID = H.MUSTERIBILGILERIID
WHERE H.HESAPDURUMKODU = 1
ORDER BY
    H.DOVIZCINSI,
    H.MUSTERIBILGILERIID,
    H.ID;

    SET SERVEROUTPUT ON;

DECLARE
    V_TRANSFERID
        KB_PARATRANSFERI.ID%TYPE;

    V_GONDERENHAREKETID
        KB_HESAPHAREKETI.ID%TYPE;

    V_ALICIHAREKETID
        KB_HESAPHAREKETI.ID%TYPE;

    V_GONDERENYENIBAKIYE
        KB_HESAPBILGILERI.BAKIYE%TYPE;

    V_ALICIYENIBAKIYE
        KB_HESAPBILGILERI.BAKIYE%TYPE;

    V_ALICITUTAR
        KB_HESAPHAREKETI.TUTAR%TYPE;

BEGIN

    KB_PARA_TRANSFERI_YAP
    (
        P_GONDERENHESAPID       => 14,
        P_ALICIHESAPID          => 22,
        P_TRANSFERTIPI          => 1,
        P_GONDERENTUTAR         => 100,
        P_DOVIZKURU             => 1,
        P_ACIKLAMA              => 'Havale prosedür testi',
        P_RECORDUSER            => 'TEST',

        P_TRANSFERID            => V_TRANSFERID,
        P_GONDERENHAREKETID     => V_GONDERENHAREKETID,
        P_ALICIHAREKETID        => V_ALICIHAREKETID,
        P_GONDERENYENIBAKIYE    => V_GONDERENYENIBAKIYE,
        P_ALICIYENIBAKIYE       => V_ALICIYENIBAKIYE,
        P_ALICITUTAR            => V_ALICITUTAR
    );

    DBMS_OUTPUT.PUT_LINE(
        'Transfer ID: ' || V_TRANSFERID
    );

    DBMS_OUTPUT.PUT_LINE(
        'Gönderen hareket ID: ' ||
        V_GONDERENHAREKETID
    );

    DBMS_OUTPUT.PUT_LINE(
        'Alıcı hareket ID: ' ||
        V_ALICIHAREKETID
    );

    DBMS_OUTPUT.PUT_LINE(
        'Gönderen yeni bakiye: ' ||
        V_GONDERENYENIBAKIYE
    );

    DBMS_OUTPUT.PUT_LINE(
        'Alıcı yeni bakiye: ' ||
        V_ALICIYENIBAKIYE
    );

    DBMS_OUTPUT.PUT_LINE(
        'Alıcıya geçen tutar: ' ||
        V_ALICITUTAR
    );

    COMMIT;

EXCEPTION

    WHEN OTHERS THEN

        ROLLBACK;

        DBMS_OUTPUT.PUT_LINE(
            'Hata kodu: ' || SQLCODE
        );

        DBMS_OUTPUT.PUT_LINE(
            'Hata mesajı: ' || SQLERRM
        );

        RAISE;

END;
/

select * from kb_hesapbilgileri;

SET SERVEROUTPUT ON;

DECLARE
    V_TRANSFERID
        KB_PARATRANSFERI.ID%TYPE;

    V_GONDERENHAREKETID
        KB_HESAPHAREKETI.ID%TYPE;

    V_ALICIHAREKETID
        KB_HESAPHAREKETI.ID%TYPE;

    V_GONDERENYENIBAKIYE
        KB_HESAPBILGILERI.BAKIYE%TYPE;

    V_ALICIYENIBAKIYE
        KB_HESAPBILGILERI.BAKIYE%TYPE;

    V_ALICITUTAR
        KB_HESAPHAREKETI.TUTAR%TYPE;

BEGIN

    KB_PARA_TRANSFERI_YAP
    (
        P_GONDERENHESAPID       => 14,
        P_ALICIHESAPID          => 25,
        P_TRANSFERTIPI          => 1,
        P_GONDERENTUTAR         => 10000,
        P_DOVIZKURU             => 0.021,
        P_ACIKLAMA              => 'TL EUR havale testi',
        P_RECORDUSER            => 'TEST',

        P_TRANSFERID            => V_TRANSFERID,
        P_GONDERENHAREKETID     => V_GONDERENHAREKETID,
        P_ALICIHAREKETID        => V_ALICIHAREKETID,
        P_GONDERENYENIBAKIYE    => V_GONDERENYENIBAKIYE,
        P_ALICIYENIBAKIYE       => V_ALICIYENIBAKIYE,
        P_ALICITUTAR            => V_ALICITUTAR
    );

    DBMS_OUTPUT.PUT_LINE(
        'Transfer ID: ' || V_TRANSFERID
    );

    DBMS_OUTPUT.PUT_LINE(
        'Gönderen hareket ID: ' ||
        V_GONDERENHAREKETID
    );

    DBMS_OUTPUT.PUT_LINE(
        'Alıcı hareket ID: ' ||
        V_ALICIHAREKETID
    );

    DBMS_OUTPUT.PUT_LINE(
        'Gönderen yeni bakiye: ' ||
        V_GONDERENYENIBAKIYE
    );

    DBMS_OUTPUT.PUT_LINE(
        'Alıcı yeni bakiye: ' ||
        V_ALICIYENIBAKIYE
    );

    DBMS_OUTPUT.PUT_LINE(
        'Alıcıya geçen tutar: ' ||
        V_ALICITUTAR
    );

    COMMIT;

EXCEPTION

    WHEN OTHERS THEN

        ROLLBACK;

        DBMS_OUTPUT.PUT_LINE(
            'Hata kodu: ' || SQLCODE
        );

        DBMS_OUTPUT.PUT_LINE(
            'Hata mesajı: ' || SQLERRM
        );

        RAISE;

END;
/


CREATE OR REPLACE PROCEDURE KB_PARATRANSFERI_GETIRBYID
(
    P_ID     IN  KB_PARATRANSFERI.ID%TYPE,
    P_SONUC  OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN P_SONUC FOR
    SELECT *
    FROM KB_PARATRANSFERI
    WHERE ID = P_ID;
END KB_PARATRANSFERI_GETIRBYID;
/
