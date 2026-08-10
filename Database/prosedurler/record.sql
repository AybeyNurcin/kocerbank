create or replace PROCEDURE KB_AUDIT_BILGI_HAZIRLA
(
    P_RECORDUSER       IN  VARCHAR2,
    P_DUZENLENMIS_USER OUT VARCHAR2,
    P_RECORDDATE       OUT DATE
)
AS
BEGIN
    IF TRIM(P_RECORDUSER) IS NULL THEN
        RAISE_APPLICATION_ERROR(
            -20501,
            'İşlemi yapan personel bilgisi bulunamadı.'
        );
    END IF;

    P_DUZENLENMIS_USER := TRIM(P_RECORDUSER);
    P_RECORDDATE       := SYSDATE;
END;