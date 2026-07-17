CREATE TABLE KB_HESAPBILGILERI(
    ID                 NUMBER(19)                                  NOT NULL,
    HESAPADI           VARCHAR2(50)                                NOT NULL,
    HESAPNO            VARCHAR2(16)                                NOT NULL,
    IBAN               VARCHAR2(26)                                NOT NULL,
    BAKIYE             NUMBER(14,2)                                NOT NULL,
    SUBESUBEKODU       VARCHAR2(20)                                NOT NULL,
    DOVIZCINSI         NUMBER(1) CHECK (DOVIZCINSI IN (1,2,3))     NOT NULL,
    HESAPACILISTARIHI  DATE DEFAULT SYSDATE                        NOT NULL,
    HESAPDURUMKODU     NUMBER(1) CHECK (HESAPDURUMKODU IN (1,2,3)) NOT NULL,
    MUSTERIBILGILERIID NUMBER(19)                                  NOT NULL,
    HESAPTIPI          NUMBER(1) CHECK (HESAPTIPI IN (1,2,3))      NOT NULL,
    RECORDUSER         VARCHAR2(10)                                NOT NULL,
    RECORDDATE         DATE DEFAULT SYSDATE                        NOT NULL,

    /* Hesap No 16 hane, IBAN 26 hane ve son 16 hanesi Hesap No */
    CONSTRAINT PK_KB_HESAPBILGILERI PRIMARY KEY (ID),
    CONSTRAINT FK_KB_HESAPBILGILERI_MUSTERIBILGILERIID FOREIGN KEY (MUSTERIBILGILERIID) REFERENCES KB_MUSTERIBILGILERI(ID) ON DELETE CASCADE

)

SELECT * FROM KB_HESAPBILGILERI;
