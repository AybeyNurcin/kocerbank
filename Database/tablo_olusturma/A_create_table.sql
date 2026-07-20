CREATE TABLE KB_MUSTERIBILGILERI(
    ID                      NUMBER(19)          DEFAULT ON NULL KB_MUSTERIBILGILERI_SEQ.NEXTVAL PRIMARY KEY,
    AD                      VARCHAR2(50)                                    NOT NULL,
    SOYAD                   VARCHAR2(50)                                    NOT NULL,
    EPOSTA                  VARCHAR2(50)        UNIQUE                      NOT NULL,
    SIFRE                   VARCHAR2 (20)                                   NOT NULL,
    DOGUMTARIHI             DATE                DEFAULT SYSDATE             NOT NULL,
    TELEFONNO               VARCHAR2(13)                                    NOT NULL,
    TCKN                    NUMBER(11)          UNIQUE                      NOT NULL, 
    CINSIYET                NUMBER(1)           CHECK (CINSIYET IN (1,2))   NOT NULL,
    VKN                     NUMBER(10)          UNIQUE                      NOT NULL,
    MUSTERITIPI             NUMBER(1)           CHECK(MUSTERITIPI IN (1,2)) NOT NULL,
    SUBESUBEKODU            VARCHAR2(20)                                    NOT NULL,
    DURUMKODU               NUMBER(1)           CHECK (DURUMKODU IN (1,2))  NOT NULL,
    UNVAN                   VARCHAR2(50)                                    NOT NULL,
    KAYITOLUSTURMATARIHI    DATE                                            NOT NULL,
    RECORDUSER              VARCHAR2(10),                                
    RECORDDATE              DATE                DEFAULT SYSDATE
);

create TABLE KB_MUSTERIILETISIM(
    ID NUMBER(19) DEFAULT ON NULL KB_MUSTERIILETISIM_SEQ.NEXTVAL PRIMARY KEY,
    ILETISIMTIPI NUMBER(1) CHECK (ILETISIMTIPI IN (1, 2, 3, 4, 5, 6)) NOT NULL,
    ILETISIM VARCHAR2(100) NOT NULL,
    MUSTERIBILGILERIID NUMBER NOT NULL,
    CONSTRAINT FK_KB_MUSTERIILETISIM_MUSTERIBILGILERIID FOREIGN KEY (MUSTERIBILGILERIID) REFERENCES KB_MUSTERIBILGILERI(ID) ON DELETE CASCADE,
    RECORDUSER VARCHAR2(10),
    RECORDDATE DATE DEFAULT SYSDATE
);