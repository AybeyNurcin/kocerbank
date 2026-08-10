using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using kocerbank_backend.Models.DTOs;
using Microsoft.Extensions.Configuration;
using kocerbank_backend.Enums;

namespace kocerbank_backend.DataAccess
{
    public class PersonelRepository
    {
        private readonly string _connectionString;

        // Bağlantı dizesini appsettings.json'dan almak için IConfiguration kullanıyoruz
        public PersonelRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleConnection") ?? throw new InvalidOperationException("Connection string bulunamadı: 'OracleConnection'");
        }

        // 1. PERSONEL EKLEME
        public PersonelDTO Ekle(PersonelDTO dto)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_PERSONEL_EKLE", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;
                    KB.BindByName = true;

                    // IN Parametreleri
                    KB.Parameters.Add("P_AD", OracleDbType.Varchar2).Value = dto.Ad;
                    KB.Parameters.Add("P_SOYAD", OracleDbType.Varchar2).Value = dto.Soyad;
                    KB.Parameters.Add("P_ROL", OracleDbType.Varchar2).Value = dto.Rol;
                    KB.Parameters.Add("P_SIFRE", OracleDbType.Varchar2).Value = dto.Sifre;
                    KB.Parameters.Add("P_TCKN", OracleDbType.Varchar2).Value = dto.TCKN;
                    KB.Parameters.Add("P_TELEFONNO", OracleDbType.Varchar2).Value = dto.TelefonNo;
                    KB.Parameters.Add("P_ADRES", OracleDbType.Varchar2).Value = dto.Adres;
                    KB.Parameters.Add("P_EPOSTA", OracleDbType.Varchar2).Value = dto.Email;
                    KB.Parameters.Add("P_SUBEKODU", OracleDbType.Varchar2).Value = dto.SubeKodu;
                    KB.Parameters.Add("P_DURUMKODU", OracleDbType.Byte).Value = (byte)dto.DurumKodu;
                    KB.Parameters.Add("P_RECORDUSER", OracleDbType.Varchar2).Value = (object?)dto.RecordUser ?? DBNull.Value;

                    // OUT Parametreleri
                    OracleParameter pId = new OracleParameter("P_ID", OracleDbType.Int64) { Direction = ParameterDirection.Output };
                    OracleParameter pSicil = new OracleParameter("P_SICIL", OracleDbType.Varchar2, 20) { Direction = ParameterDirection.Output };
                    OracleParameter pKayitOlusturmaTarihi = new OracleParameter("P_KAYITOLUSTURMATARIHI", OracleDbType.Date) { Direction = ParameterDirection.Output };

                    KB.Parameters.Add(pId);
                    KB.Parameters.Add(pSicil);
                    KB.Parameters.Add(pKayitOlusturmaTarihi);

                    conn.Open();
                    KB.ExecuteNonQuery();

                    // Üretilen değerleri DTO'ya geri yazıyoruz
                    dto.Id = Convert.ToInt64(pId.Value.ToString());
                    dto.Sicil = pSicil.Value.ToString()!;
                    dto.KayitOlusturmaTarihi = OracleZamanDamgasi.UtcOlarakOku(pKayitOlusturmaTarihi.Value);

                    return dto;
                }
            }
        }

        // 2. ID'YE GÖRE GETİR (READ)
        public PersonelDTO? GetirById(long id)
        {
            PersonelDTO? personel = null;

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_PERSONEL_GETIRBYID", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;
                    KB.BindByName = true;

                    KB.Parameters.Add("P_ID", OracleDbType.Int64).Value = id;
                    
                    // Oracle'daki SYS_REFCURSOR'u C# tarafında okumak için RefCursor tipi eklenir
                    KB.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    
                    // Cursor verisini okumak için OracleDataReader kullanıyoruz
                    using (OracleDataReader reader = KB.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            personel = MapReaderToDTO(reader);
                        }
                    }
                }
            }
            return personel;
        }

        public PersonelDTO? GetirBySicil(string sicil)
        {
            PersonelDTO? personel = null;

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                 using (OracleCommand KB = new OracleCommand("KB_PERSONEL_GETIRBYSICIL", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;
                    KB.BindByName = true;

                    KB.Parameters.Add("P_SICIL", OracleDbType.Varchar2).Value = sicil;
                    KB.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();

                    using (OracleDataReader reader = KB.ExecuteReader())
                {
                    if (reader.Read())
                {
                    personel = MapReaderToDTO(reader);
                }
                }
            }
    }

    return personel;
}

        // 3. KRİTERE GÖRE LİSTELE
        public List<PersonelDTO> GetirListele(PersonelAramaKriterleriDTO aramaKriterleri)
        {
            List<PersonelDTO> liste = new List<PersonelDTO>();

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_PERSONEL_LISTELE", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;
                    KB.BindByName = true;

                    // Arama parametrelerinde NULL olabilme ihtimaline karşı DBNull.Value kullanıyoruz
                    KB.Parameters.Add("P_ID", OracleDbType.Int64).Value = aramaKriterleri.Id.HasValue ? (object)aramaKriterleri.Id.Value : DBNull.Value;
                    KB.Parameters.Add("P_AD", OracleDbType.Varchar2).Value = (object?)aramaKriterleri.Ad ?? DBNull.Value;
                    KB.Parameters.Add("P_SOYAD", OracleDbType.Varchar2).Value = (object?)aramaKriterleri.Soyad ?? DBNull.Value;
                    KB.Parameters.Add("P_ROL", OracleDbType.Varchar2).Value = (object?)aramaKriterleri.Rol ?? DBNull.Value;
                    KB.Parameters.Add("P_TCKN", OracleDbType.Varchar2).Value = (object?)aramaKriterleri.TCKN ?? DBNull.Value;
                    KB.Parameters.Add("P_TELEFONNO", OracleDbType.Varchar2).Value = (object?)aramaKriterleri.TelefonNo ?? DBNull.Value;
                    KB.Parameters.Add("P_SUBEKODU", OracleDbType.Varchar2).Value = (object?)aramaKriterleri.SubeKodu ?? DBNull.Value;
                    KB.Parameters.Add("P_ADRES", OracleDbType.Varchar2).Value = (object?)aramaKriterleri.Adres ?? DBNull.Value;
                    KB.Parameters.Add("P_EPOSTA", OracleDbType.Varchar2).Value = (object?)aramaKriterleri.Email ?? DBNull.Value;
                    KB.Parameters.Add("P_DURUMKODU", OracleDbType.Byte).Value = aramaKriterleri.DurumKodu.HasValue ? (object)(byte)aramaKriterleri.DurumKodu.Value : DBNull.Value;                    
                    KB.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();

                    using (OracleDataReader reader = KB.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            liste.Add(MapReaderToDTO(reader));
                        }
                    }
                }
            }
            return liste;
        }

        // 4. GÜNCELLE
        public void Guncelle(PersonelDTO dto)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_PERSONEL_GUNCELLE", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;
                    KB.BindByName = true;

                    KB.Parameters.Add("P_ID", OracleDbType.Int64).Value = dto.Id;
                    KB.Parameters.Add("P_RECORDUSER", OracleDbType.Varchar2).Value = (object?)dto.RecordUser ?? DBNull.Value;
                    KB.Parameters.Add("P_AD", OracleDbType.Varchar2).Value = dto.Ad;
                    KB.Parameters.Add("P_SOYAD", OracleDbType.Varchar2).Value = dto.Soyad;
                    KB.Parameters.Add("P_ROL", OracleDbType.Varchar2).Value = dto.Rol;
                    KB.Parameters.Add("P_SIFRE", OracleDbType.Varchar2).Value = dto.Sifre;
                    KB.Parameters.Add("P_TCKN", OracleDbType.Varchar2).Value = dto.TCKN;
                    KB.Parameters.Add("P_TELEFONNO", OracleDbType.Varchar2).Value = dto.TelefonNo;
                    KB.Parameters.Add("P_ADRES", OracleDbType.Varchar2).Value = dto.Adres;
                    KB.Parameters.Add("P_EPOSTA", OracleDbType.Varchar2).Value = dto.Email;
                    KB.Parameters.Add("P_SUBEKODU", OracleDbType.Varchar2).Value = dto.SubeKodu;
                    KB.Parameters.Add("P_DURUMKODU", OracleDbType.Byte).Value = (byte)dto.DurumKodu;

                    conn.Open();
                    KB.ExecuteNonQuery();
                }
            }
        }

        // 5. SİL
        public void Sil(long id)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_PERSONEL_SIL", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;
                    KB.BindByName = true;
                    KB.Parameters.Add("P_ID", OracleDbType.Int64).Value = id;

                    conn.Open();
                    KB.ExecuteNonQuery();
                }
            }
        }

        public PersonelDashboardDTO GetirDashboardOzet(DateTime? baslangicTarihi, DateTime? bitisTarihi)
        {
            PersonelDashboardDTO ozet = new PersonelDashboardDTO();

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_PERSONELDASHBOARD", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;
                    KB.BindByName = true;

                    KB.Parameters.Add("P_BASLANGICTARIHI", OracleDbType.Date).Value = (object?)baslangicTarihi ?? DBNull.Value;
                    KB.Parameters.Add("P_BITISTARIHI", OracleDbType.Date).Value = (object?)bitisTarihi ?? DBNull.Value;
                    KB.Parameters.Add("P_SONUC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();

                    using (OracleDataReader reader = KB.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ozet.ToplamPersonel = Convert.ToInt64(reader["PERSONEL_SAYISI"]);
                            ozet.AktifSayi      = Convert.ToInt64(reader["AKTIFSAYI"]);
                            ozet.PasifSayi      = Convert.ToInt64(reader["PASIFSAYI"]);
                        }
                    }
                }
            }

            return ozet;
        }


        // YARDIMCI METOT: Veritabanı satırını DTO nesnesine dönüştürür (Kod tekrarını önler)
        private PersonelDTO MapReaderToDTO(OracleDataReader reader)
        {
            return new PersonelDTO
            {
                Id = Convert.ToInt64(reader["ID"]),
                Ad = reader["AD"].ToString()!,
                Soyad = reader["SOYAD"].ToString()!,
                Rol = reader["ROL"].ToString()!,
                Sicil = reader["SICIL"].ToString()!,
                Sifre = reader["SIFRE"].ToString()!,
                TCKN = Convert.ToString(reader["TCKN"])!,
                TelefonNo = reader["TELEFONNO"].ToString()!,
                Adres = reader["ADRES"].ToString()!,
                Email = reader["EPOSTA"].ToString()!,
                SubeKodu = reader["SUBESUBEKODU"].ToString()!,
                DurumKodu = (AktifPasifDurumlari)Convert.ToByte(reader["DURUMKODU"]),
                RecordUser = reader["RECORDUSER"] == DBNull.Value ? null : reader["RECORDUSER"].ToString(),
                RecordDate = reader["RECORDDATE"] == DBNull.Value ? null : OracleZamanDamgasi.UtcOlarakOku(reader["RECORDDATE"]),
                KayitOlusturmaTarihi = OracleZamanDamgasi.UtcOlarakOku(reader["KAYITOLUSTURMATARIHI"])
            };
        }
    }
}