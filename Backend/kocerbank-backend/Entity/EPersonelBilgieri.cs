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
            _connectionString = configuration.GetConnectionString("OracleConnection");
        }

        // 1. PERSONEL EKLEME
        public PersonelDTO Ekle(PersonelDTO dto)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_PERSONEL_EKLE", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;

                    // IN Parametreleri
                    KB.Parameters.Add("P_PERSONEL_ADI", OracleDbType.Varchar2).Value = dto.Ad;
                    KB.Parameters.Add("P_PERSONEL_SOYADI", OracleDbType.Varchar2).Value = dto.Soyad;
                    KB.Parameters.Add("P_PERSONEL_ROLU", OracleDbType.Varchar2).Value = dto.Rol;
                    KB.Parameters.Add("P_PERSONEL_SIFRESI", OracleDbType.Varchar2).Value = dto.Sifre;
                    KB.Parameters.Add("P_PERSONEL_TCKN", OracleDbType.Int32).Value = dto.TCKN;
                    KB.Parameters.Add("P_PERSONEL_TELEFON_NO", OracleDbType.Varchar2).Value = dto.TelefonNo;
                    KB.Parameters.Add("P_PERSONEL_ADRES", OracleDbType.Varchar2).Value = dto.Adres;
                    KB.Parameters.Add("P_PERSONEL_EPOSTA", OracleDbType.Varchar2).Value = dto.Email;
                    KB.Parameters.Add("P_PERSONEL_SUBEKODU", OracleDbType.Varchar2).Value = dto.SubeKodu;
                    KB.Parameters.Add("P_PERSONEL_DURUMKODU", OracleDbType.Byte).Value = (byte)dto.DurumKodu;
                    KB.Parameters.Add("P_PERSONEL_RECORDUSER", OracleDbType.Varchar2).Value = dto.RecordUser;

                    // OUT Parametreleri
                    OracleParameter pId = new OracleParameter("P_PERSONEL_ID", OracleDbType.Int64) { Direction = ParameterDirection.Output };
                    OracleParameter pSicil = new OracleParameter("P_PERSONEL_SICIL", OracleDbType.Varchar2, 50) { Direction = ParameterDirection.Output };
                    
                    KB.Parameters.Add(pId);
                    KB.Parameters.Add(pSicil);

                    conn.Open();
                    KB.ExecuteNonQuery();

                    // Üretilen değerleri DTO'ya geri yazıyoruz
                    dto.Id = Convert.ToInt64(pId.Value.ToString());
                    dto.Sicil = pSicil.Value.ToString();

                    return dto;
                }
            }
        }

        // 2. ID'YE GÖRE GETİR (READ)
        public PersonelDTO GetirById(long id)
        {
            PersonelDTO personel = null;

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_PERSONEL_READ_BY_ID", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;

                    KB.Parameters.Add("P_PERSONEL_ID", OracleDbType.Int64).Value = id;
                    
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

        // 3. KRİTERE GÖRE LİSTELE
        public List<PersonelDTO> GetirListele(PersonelDTO aramaKriterleri)
        {
            List<PersonelDTO> liste = new List<PersonelDTO>();

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_PERSONEL_READ_BY_LISTELE", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;

                    // Arama parametrelerinde NULL olabilme ihtimaline karşı DBNull.Value kullanıyoruz
                    KB.Parameters.Add("P_PERSONEL_ADI", OracleDbType.Varchar2).Value = (object)aramaKriterleri.Ad ?? DBNull.Value;
                    KB.Parameters.Add("P_PERSONEL_SOYADI", OracleDbType.Varchar2).Value = (object)aramaKriterleri.Soyad ?? DBNull.Value;
                    KB.Parameters.Add("P_PERSONEL_ROLU", OracleDbType.Varchar2).Value = (object)aramaKriterleri.Rol ?? DBNull.Value;
                    KB.Parameters.Add("P_PERSONEL_TCKN", OracleDbType.Int32).Value = aramaKriterleri.TCKN == 0 ? DBNull.Value : aramaKriterleri.TCKN;
                    KB.Parameters.Add("P_PERSONEL_TELEFON_NO", OracleDbType.Varchar2).Value = (object)aramaKriterleri.TelefonNo ?? DBNull.Value;
                    KB.Parameters.Add("P_PERSONEL_SUBEKODU", OracleDbType.Varchar2).Value = (object)aramaKriterleri.SubeKodu ?? DBNull.Value;
                    KB.Parameters.Add("P_PERSONEL_ADRES", OracleDbType.Varchar2).Value = (object)aramaKriterleri.Adres ?? DBNull.Value;
                    KB.Parameters.Add("P_PERSONEL_EPOSTA", OracleDbType.Varchar2).Value = (object)aramaKriterleri.Email ?? DBNull.Value;
                    KB.Parameters.Add("P_PERSONEL_DURUMKODU", OracleDbType.Byte).Value = aramaKriterleri.DurumKodu == AktifPasifDurumlari.None ? DBNull.Value : (byte)aramaKriterleri.DurumKodu;                    
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

                    KB.Parameters.Add("P_PERSONEL_ID", OracleDbType.Int64).Value = dto.Id;
                    KB.Parameters.Add("P_PERSONEL_ADI", OracleDbType.Varchar2).Value = dto.Ad;
                    KB.Parameters.Add("P_PERSONEL_SOYADI", OracleDbType.Varchar2).Value = dto.Soyad;
                    KB.Parameters.Add("P_PERSONEL_ROLU", OracleDbType.Varchar2).Value = dto.Rol;
                    KB.Parameters.Add("P_PERSONEL_SIFRESI", OracleDbType.Varchar2).Value = dto.Sifre;
                    KB.Parameters.Add("P_PERSONEL_TCKN", OracleDbType.Int32).Value = dto.TCKN;
                    KB.Parameters.Add("P_PERSONEL_TELEFON_NO", OracleDbType.Varchar2).Value = dto.TelefonNo;
                    KB.Parameters.Add("P_PERSONEL_ADRES", OracleDbType.Varchar2).Value = dto.Adres;
                    KB.Parameters.Add("P_PERSONEL_EPOSTA", OracleDbType.Varchar2).Value = dto.Email;
                    KB.Parameters.Add("P_PERSONEL_SUBEKODU", OracleDbType.Varchar2).Value = dto.SubeKodu;
                    KB.Parameters.Add("P_PERSONEL_DURUMKODU", OracleDbType.Byte).Value = (byte)dto.DurumKodu;
                    KB.Parameters.Add("P_PERSONEL_RECORDUSER", OracleDbType.Varchar2).Value = dto.RecordUser;

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
                    KB.Parameters.Add("P_PERSONEL_ID", OracleDbType.Int64).Value = id;

                    conn.Open();
                    KB.ExecuteNonQuery();
                }
            }
        }

        // YARDIMCI METOT: Veritabanı satırını DTO nesnesine dönüştürür (Kod tekrarını önler)
        private PersonelDTO MapReaderToDTO(OracleDataReader reader)
        {
            return new PersonelDTO
            {
                Id = Convert.ToInt64(reader["ID"]),
                Ad = reader["AD"].ToString(),
                Soyad = reader["SOYAD"].ToString(),
                Rol = reader["ROL"].ToString(),
                Sicil = reader["SICIL"].ToString(),
                Sifre = reader["SIFRE"].ToString(),
                TCKN = Convert.ToInt32(reader["TCKN"]),
                TelefonNo = reader["TELEFONNO"].ToString(),
                Adres = reader["ADRES"].ToString(),
                Email = reader["EPOSTA"].ToString(),
                SubeKodu = reader["SUBESUBEKODU"].ToString(),
                DurumKodu = (AktifPasifDurumlari)Convert.ToByte(reader["DURUMKODU"]),
                RecordUser = reader["RECORDUSER"].ToString()
                // Eğer SQL tablosunda RecordDate varsa o da buraya eklenebilir.
            };
        }
    }
}