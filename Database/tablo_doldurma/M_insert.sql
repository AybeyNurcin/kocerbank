INSERT INTO KB_SUBE (SUBEADI, SUBETELEFONNO, SUBEADRES, SUBEDURUMKODU) VALUES ('Merkez Şube', '02121234567', 'İstanbul, Türkiye', 1);
INSERT INTO KB_SUBE (SUBEADI, SUBETELEFONNO, SUBEADRES, SUBEDURUMKODU) VALUES ('Ümraniye Şube','02121234568', 'İstanbul, Türkiye', 1);
COMMIT;

INSERT INTO KB_PERSONEL (AD, SOYAD, ROL, SIFRE, TCKN, TELEFONNO, ADRES, EPOSTA, SUBESUBEKODU, DURUMKODU) VALUES ('Metehan', 'Ünal', 'Admin', 'admin123', 12345678901, '05551234567', 'İstanbul, Türkiye', 'metehan.unal@kocerbank.com', 'S0008', 1);
INSERT INTO KB_PERSONEL (AD, SOYAD, ROL, SIFRE, TCKN, TELEFONNO, ADRES, EPOSTA, SUBESUBEKODU, DURUMKODU) VALUES ('Ahmet', '7abak', 'Admin', 'admin123', 12345678902, '05551234568', 'İstanbul, Türkiye', 'ahmet.7abak@kocerbank.com', 'S0034', 1);
INSERT INTO KB_PERSONEL (AD, SOYAD, ROL, SIFRE, TCKN, TELEFONNO, ADRES, EPOSTA, SUBESUBEKODU, DURUMKODU) VALUES ('Ayşegül', 'Kara', 'Admin', 'admin123', 12345678903, '05551234569', 'İstanbul, Türkiye', 'aysegul.kara@kocerbank.com', 'S0035', 1);
insert into KB_PERSONEL (AD, SOYAD, ROL, SIFRE, TCKN, TELEFONNO, ADRES, EPOSTA, SUBESUBEKODU, DURUMKODU) values ('Ayşegül', 'Kara', 'Admin', 'admin123', 12345678903, '05551234569', 'İstanbul, Türkiye', 'aysegul.kara@kocerbank.com', 'S0035', 1);
insert into KB_PERSONEL (AD, SOYAD, ROL, SIFRE, TCKN, TELEFONNO, ADRES, EPOSTA, SUBESUBEKODU, DURUMKODU) values ('Ayşe', 'Kar', 'Admin', 'admin123', 12345678905, '05551234560', 'İstanbul, Türkiye', 'ayşe.kar@kocerbank.com', 'S0037', 1);
INSERT INTO KB_PERSONEL (AD, SOYAD, ROL, SIFRE, TCKN, TELEFONNO, ADRES, EPOSTA, SUBESUBEKODU, DURUMKODU) VALUES ('Kral', 'Şakir', 'Admin', '123456', 12345678908, '05551234561', 'İstanbul, Türkiye', 'kral.sakir@kocerbank.com', 'S0038', 1);





UPDATE KB_PERSONEL SET AD = 'Metehan' WHERE ID = 3;
COMMIT;

SELECT * FROM KB_PERSONEL;
SELECT * FROM KB_SUBE;



DELETE FROM KB_personel where id=32;
