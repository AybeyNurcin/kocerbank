CREATE OR REPLACE PROCEDURE KB_HESAPBILGILERI_EKLE
(
    P_HESAPADI            IN  KB_HESAPBILGILERI.HESAPADI%TYPE,
    P_SUBESUBEKODU        IN  KB_HESAPBILGILERI.SUBESUBEKODU%TYPE,
    P_DOVIZCINSI          IN  KB_HESAPBILGILERI.DOVIZCINSI%TYPE,
    P_HESAPDURUMKODU      IN  KB_HESAPBILGILERI.HESAPDURUMKODU%TYPE,
    P_MUSTERIBILGILERIID  IN  KB_HESAPBILGILERI.MUSTERIBILGILERIID%TYPE,
    P_HESAPTIPI           IN  KB_HESAPBILGILERI.HESAPTIPI%TYPE,
    P_RECORDUSER          IN  KB_HESAPBILGILERI.RECORDUSER%TYPE,

    P_YENI_ID             OUT KB_HESAPBILGILERI.ID%TYPE,
    P_HESAPNO             OUT KB_HESAPBILGILERI.HESAPNO%TYPE,
    P_IBAN                OUT KB_HESAPBILGILERI.IBAN%TYPE
)
IS
    V_ACILIS_TARIHI  DATE := SYSDATE;
    V_SUBE_SAYISAL   VARCHAR2(4);
    V_SIRA_NO        NUMBER;
    V_HESAPNO        VARCHAR2(16);
    V_BBAN           VARCHAR2(22);
    V_MOD97_METNI    VARCHAR2(30);
    V_KALAN          NUMBER := 0;
    V_KONTROL_NO     NUMBER;
    V_KAYIT_SAYISI   NUMBER;
    V_DENEME_SAYISI  NUMBER := 0;
BEGIN
    /* Şube kodu S ve ardından dört rakam olmalıdır. */
    IF NOT REGEXP_LIKE(P_SUBESUBEKODU, '^S[0-9]{4}$') THEN
        RAISE_APPLICATION_ERROR(
            -20001,
            'Şube kodu S ve ardından dört rakam içermelidir.'
        );
    END IF;

    /* S0034 içinden 0034 alınır. */
    V_SUBE_SAYISAL := SUBSTR(P_SUBESUBEKODU, 2, 4);

    /*
       Kullanılmayan bir hesap numarası bulunana kadar
       sequence değerleri denenir.
    */
    LOOP
        V_DENEME_SAYISI := V_DENEME_SAYISI + 1;

        IF V_DENEME_SAYISI > 9999 THEN
            RAISE_APPLICATION_ERROR(
                -20002,
                'Uygun hesap numarası üretilemedi.'
            );
        END IF;

        V_SIRA_NO := KB_HESAPNO_SEQ.NEXTVAL;

        V_HESAPNO :=
              V_SUBE_SAYISAL
            || TO_CHAR(V_ACILIS_TARIHI, 'DDMMYYYY')
            || LPAD(V_SIRA_NO, 4, '0');

        SELECT COUNT(*)
          INTO V_KAYIT_SAYISI
          FROM KB_HESAPBILGILERI
         WHERE HESAPNO = V_HESAPNO;

        EXIT WHEN V_KAYIT_SAYISI = 0;
    END LOOP;

    /*
       Türkiye IBAN BBAN yapısı:

       00016  = Banka kodu
       0      = Rezerv alanı
       16 hane hesap numarası
    */
    V_BBAN := '00069' || '0' || V_HESAPNO;

    /*
       T = 29
       R = 27

       Kontrol hanesi başlangıçta 00 kabul edilir.
    */
    V_MOD97_METNI := V_BBAN || '2927' || '00';

    /*
       Büyük sayıyı doğrudan NUMBER'a çevirmeden,
       rakam rakam MOD97 hesaplanır.
    */
    V_KALAN := 0;

    FOR I IN 1 .. LENGTH(V_MOD97_METNI)
    LOOP
        V_KALAN :=
            MOD(
                V_KALAN * 10
                + TO_NUMBER(SUBSTR(V_MOD97_METNI, I, 1)),
                97
            );
    END LOOP;

    V_KONTROL_NO := 98 - V_KALAN;

    V_HESAPNO := LPAD(V_HESAPNO, 16, '0');

    P_HESAPNO := V_HESAPNO;

    P_IBAN :=
          'TR'
        || LPAD(V_KONTROL_NO, 2, '0')
        || V_BBAN;

    INSERT INTO KB_HESAPBILGILERI
    (
        HESAPADI,
        HESAPNO,
        IBAN,
        BAKIYE,
        SUBESUBEKODU,
        DOVIZCINSI,
        HESAPACILISTARIHI,
        HESAPDURUMKODU,
        MUSTERIBILGILERIID,
        HESAPTIPI,
        RECORDUSER,
        RECORDDATE
    )
    VALUES
    (
        P_HESAPADI,
        P_HESAPNO,
        P_IBAN,
        0,
        P_SUBESUBEKODU,
        P_DOVIZCINSI,
        V_ACILIS_TARIHI,
        P_HESAPDURUMKODU,
        P_MUSTERIBILGILERIID,
        P_HESAPTIPI,
        P_RECORDUSER,
        SYSDATE
    )
    RETURNING ID INTO P_YENI_ID;
END;
/

SELECT ID, AD, SOYAD, SUBESUBEKODU
FROM KB_MUSTERIBILGILERI;

DECLARE
    V_YENI_ID  NUMBER;
    V_HESAPNO  VARCHAR2(16);
    V_IBAN     VARCHAR2(26);
BEGIN
    KB_HESAPBILGILERI_EKLE
    (
        P_HESAPADI           => 'Maaş Hesabım69',
        P_SUBESUBEKODU       => 'S0034',
        P_DOVIZCINSI         => 1,
        P_HESAPDURUMKODU     => 1,
        P_MUSTERIBILGILERIID => 44,
        P_HESAPTIPI          => 1,
        P_RECORDUSER         => 'METEHAN',

        P_YENI_ID            => V_YENI_ID,
        P_HESAPNO            => V_HESAPNO,
        P_IBAN               => V_IBAN
    );

    DBMS_OUTPUT.PUT_LINE('Yeni ID: ' || V_YENI_ID);
    DBMS_OUTPUT.PUT_LINE('Hesap No: ' || V_HESAPNO);
    DBMS_OUTPUT.PUT_LINE('IBAN: ' || V_IBAN);

    COMMIT;
END;
/