CREATE OR REPLACE PROCEDURE KB_SUBE_EKLE
(
    p_sube_adi          IN  KB_SUBE.SUBEADI%TYPE,
    p_sube_telefon_no   IN  KB_SUBE.SUBETELEFONNO%TYPE,
    p_sube_adres        IN  KB_SUBE.SUBEADRES%TYPE,
    p_sube_durum_kodu   IN  KB_SUBE.SUBEDURUMKODU%TYPE,
    p_record_user       IN  KB_SUBE.RECORDUSER%TYPE,

    p_yeni_id           OUT KB_SUBE.ID%TYPE,
    p_yeni_sube_kodu    OUT KB_SUBE.SUBEKODU%TYPE
)
IS
BEGIN

    INSERT INTO KB_SUBE
    (
        ID,
        SUBEADI,
        SUBEKODU,
        SUBETELEFONNO,
        SUBEADRES,
        SUBEDURUMKODU,
        RECORDUSER
    )
    VALUES
    (
        p_yeni_id,
        p_sube_adi,
        p_yeni_sube_kodu,
        p_sube_telefon_no,
        p_sube_adres,
        p_sube_durum_kodu,
        p_record_user
    )
    RETURNING
        ID,
        SUBEKODU
    INTO
        p_yeni_id,
        p_yeni_sube_kodu;
END;
/

SET SERVEROUTPUT ON;

DECLARE
    v_yeni_id         KB_SUBE.ID%TYPE;
    v_yeni_sube_kodu  KB_SUBE.SUBEKODU%TYPE;
BEGIN
    KB_SUBE_EKLE
    (
        p_sube_adi        => 'Başakşehir Şube3',
        p_sube_telefon_no => '02121234469',
        p_sube_adres      => 'Başakşehir, İstanbul',
        p_sube_durum_kodu => 1,
        p_record_user     => 'METEHAN',
        p_yeni_id         => v_yeni_id,
        p_yeni_sube_kodu  => v_yeni_sube_kodu
    );

    DBMS_OUTPUT.PUT_LINE('Yeni ID: ' || v_yeni_id);
    DBMS_OUTPUT.PUT_LINE('Yeni şube kodu: ' || v_yeni_sube_kodu);

    COMMIT;
END;
/

select * from KB_SUBE;


COMMIT;
/*---------------------------------------------------------------------------------------------------------------*/

CREATE OR REPLACE PROCEDURE KB_SUBE_GETIR(
    p_id         IN KB_SUBE.ID%TYPE,

    p_sonuc      OUT SYS_REFCURSOR
)
IS
BEGIN
    OPEN p_sonuc FOR
        SELECT *
        FROM KB_SUBE
        WHERE ID = p_id;
END;
/

VARIABLE v_sonuc REFCURSOR;

EXEC KB_SUBE_GETIR(15, :v_sonuc);

PRINT v_sonuc;

/*---------------------------------------------------------------------------------------------------------------*/


CREATE OR REPLACE PROCEDURE KB_SUBE_LISTELE(
   p_sube_adi          IN KB_SUBE.SUBEADI%TYPE,
   p_sube_kodu         IN KB_SUBE.SUBEKODU%TYPE,
   p_sube_telefon_no   IN KB_SUBE.SUBETELEFONNO%TYPE,
   p_sube_adres        IN KB_SUBE.SUBEADRES%TYPE,
   p_sube_durum_kodu   IN KB_SUBE.SUBEDURUMKODU%TYPE,
   
    p_sonuc      OUT SYS_REFCURSOR
)
IS
BEGIN
    OPEN p_sonuc FOR
        SELECT *
        FROM KB_SUBE
        WHERE (p_sube_adi IS NULL OR UPPER(SUBEADI) LIKE '%' || UPPER(p_sube_adi) || '%')
          AND (p_sube_kodu IS NULL OR SUBEKODU = p_sube_kodu)
          AND (p_sube_telefon_no IS NULL OR SUBETELEFONNO = p_sube_telefon_no)
          AND (p_sube_adres IS NULL OR UPPER(SUBEADRES) LIKE '%' || UPPER(p_sube_adres) || '%')
          AND (p_sube_durum_kodu IS NULL OR SUBEDURUMKODU = p_sube_durum_kodu);
END;
/

VARIABLE v_sonuc REFCURSOR;

EXEC KB_SUBE_LISTELE(p_sube_adres=> 'ş', p_sube_durum_kodu  => 1, p_sonuc => :v_sonuc);

PRINT v_sonuc;


/*---------------------------------------------------------------------------------------------------------------*/

CREATE OR REPLACE PROCEDURE KB_SUBE_SIL(
    p_id         IN KB_SUBE.ID%TYPE

)
IS
BEGIN
    DELETE FROM KB_SUBE
    WHERE ID = p_id;

END;
/

EXEC KB_SUBE_SIL(16);

/*---------------------------------------------------------------------------------------------------------------*/

CREATE OR REPLACE PROCEDURE KB_SUBE_GUNCELLE(
    p_id                IN KB_SUBE.ID%TYPE,
    p_sube_adi          IN KB_SUBE.SUBEADI%TYPE,
    p_sube_telefon_no   IN KB_SUBE.SUBETELEFONNO%TYPE,
    p_sube_adres        IN KB_SUBE.SUBEADRES%TYPE,
    p_sube_durum_kodu   IN KB_SUBE.SUBEDURUMKODU%TYPE
)
IS
BEGIN
    UPDATE KB_SUBE
    SET SUBEADI = p_sube_adi,
        SUBETELEFONNO = p_sube_telefon_no,
        SUBEADRES = p_sube_adres,
        SUBEDURUMKODU = p_sube_durum_kodu
    WHERE ID = p_id;

END;
/

SELECT *
FROM KB_SUBE
WHERE ID = 15;


BEGIN
    KB_SUBE_GUNCELLE
    (
        p_id                => 15,
        p_sube_adi          => 'Beşiktaş Şube',
        p_sube_telefon_no   => '02121234565',
        p_sube_adres        => 'Beşiktaş, İstanbul',
        p_sube_durum_kodu   => 1
    );
END;
/

COMMIT;





