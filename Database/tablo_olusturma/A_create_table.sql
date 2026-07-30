CREATE TABLE KB_MUSTERIBILGILERI(
    ID                      NUMBER(19)          DEFAULT ON NULL KB_MUSTERIBILGILERI_SEQ.NEXTVAL PRIMARY KEY,
    AD                      VARCHAR2(50)                                    NOT NULL,
    SOYAD                   VARCHAR2(50)                                    NOT NULL,
    EPOSTA                  VARCHAR2(50)        UNIQUE                      NOT NULL,
    DOGUMTARIHI             DATE                DEFAULT SYSDATE             NOT NULL,
    TELEFONNO               VARCHAR2(13)                                    NOT NULL,
    TCKN                    VARCHAR2(11)        UNIQUE                              , 
    CINSIYET                NUMBER(1)           CHECK (CINSIYET IN (1,2))   NOT NULL,
    VKN                     VARCHAR2(10)        UNIQUE                              ,
    MUSTERITIPI             NUMBER(1)           CHECK(MUSTERITIPI IN (1,2)) NOT NULL,
    SUBESUBEKODU            VARCHAR2(20)                                    NOT NULL,

    CONSTRAINT FK_KB_MUSTERIBILGILERI_SUBESUBEKODU FOREIGN KEY (SUBESUBEKODU) REFERENCES KB_SUBE(SUBEKODU) ON DELETE CASCADE,

    DURUMKODU               NUMBER(1)           CHECK (DURUMKODU IN (1,2))  NOT NULL,
    UNVAN                   VARCHAR2(50)                                    NOT NULL,
    KAYITOLUSTURMATARIHI    DATE                DEFAULT SYSDATE             NOT NULL,
    RECORDUSER              VARCHAR2(10),                                
    RECORDDATE              DATE                DEFAULT SYSDATE
);
select * from KB_HESAPBILGILERI;

alter table kb_musteribilgileri add constraint chk_musteri_tckn_vkn check ((musteritipi = 1 and tckn is not null) or (musteritipi = 2 and vkn is not null));
alter table kb_musteribilgileri add constraint chk_musteri_cinsiyet check ((musteritipi = 1 and cinsiyet is not null) or (musteritipi = 2 and cinsiyet is null));
ALTER TABLE KB_MUSTERIBILGILERI MODIFY UNVAN NULL;
desc kb_musteribilgileri;


create TABLE KB_MUSTERIILETISIM(
    ID NUMBER(19) DEFAULT ON NULL KB_MUSTERIILETISIM_SEQ.NEXTVAL PRIMARY KEY,
    TELEFONNO VARCHAR2(13) NOT NULL,
    EVTELEFON VARCHAR2(11),
    ISTELEFON VARCHAR2(13),
    EVADRES VARCHAR2(100),
    ISADRES VARCHAR2(100),
    EPOSTA VARCHAR2(100),
    MUSTERIBILGILERIID NUMBER NOT NULL,
    CONSTRAINT FK_KB_MUSTERIILETISIM_MUSTERIBILGILERIID FOREIGN KEY (MUSTERIBILGILERIID) REFERENCES KB_MUSTERIBILGILERI(ID) ON DELETE CASCADE,
    RECORDUSER VARCHAR2(10),
    RECORDDATE DATE DEFAULT SYSDATE
);
/

delete KB_MUSTERIBILGILERI;

delete from KB_MUSTERIBILGILERI;

select * from KB_MUSTERIBILGILERI;

delete from KB_MUSTERIBILGILERI where id = 44;

select * from KB_SUBE where subekodu = 'S0034';
commit;
