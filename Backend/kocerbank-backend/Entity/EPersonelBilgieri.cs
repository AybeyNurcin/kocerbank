using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using kocerbank_backend.Models.DTOs;
using Microsoft.Extensions.Configuration;

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
                using (OracleCommand cmd = new OracleCommand("KB_PERSONEL_EKLE", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // IN Parametreleri
                    cmd.Parameters.Add("P_PERSONEL_ADI", OracleDbType.Varchar2).Value = dto.Ad;
                    cmd.Parameters.Add("P_PERSONEL_SOYADI", OracleDbType.Varchar2).Value = dto.Soyad;
                    cmd.Parameters.Add("P_PERSONEL_ROLU", OracleDbType.Varchar2).Value = dto.Rol;
                    cmd.Parameters.Add("P_PERSONEL_SIFRESI", OracleDbType.Varchar2).Value = dto.Sifre;
                    cmd.Parameters.Add("P_PERSONEL_TCKN", OracleDbType.Int32).Value = dto.TCKN;
                    cmd.Parameters.Add("P_PERSONEL_TELEFON_NO", OracleDbType.Varchar2).Value = dto.TelefonNo;
                    cmd.Parameters.Add("P_PERSONEL_ADRES", OracleDbType.Varchar2).Value = dto.Adres;
                    cmd.Parameters.Add("P_PERSONEL_EPOSTA", OracleDbType.Varchar2).Value = dto.Email;
                    cmd.Parameters.Add("P_PERSONEL_SUBEKODU", OracleDbType.Varchar2).Value = dto.SubeKodu;
                    cmd.Parameters.Add("P_PERSONEL_DURUMKODU", OracleDbType.Byte).Value = dto.DurumKodu;
                    cmd.Parameters.Add("P_PERSONEL_RECORDUSER", OracleDbType.Varchar2).Value = dto.RecordUser;

                    // OUT Parametreleri
                    OracleParameter pId = new OracleParameter("P_PERSONEL_ID", OracleDbType.Int64) { Direction = ParameterDirection.Output };
                    OracleParameter pSicil = new OracleParameter("P_PERSONEL_SICIL", OracleDbType.Varchar2, 50) { Direction = ParameterDirection.Output };
                    
                    cmd.Parameters.Add(pId);
                    cmd.Parameters.Add(pSicil);

                    conn.Open();
                    cmd.ExecuteNonQuery();

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
                using (OracleCommand cmd = new OracleCommand("KB_PERSONEL_READ_BY_ID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("P_PERSONEL_ID", OracleDbType.Int64).Value = id;
                    
                    // Oracle'daki SYS_REFCURSOR'u C# tarafında okumak için RefCursor tipi eklenir
                    cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    
                    // Cursor verisini okumak için OracleDataReader kullanıyoruz
                    using (OracleDataReader reader = cmd.ExecuteReader())
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
                using (OracleCommand cmd = new OracleCommand("KB_PERSONEL_READ_BY_LISTELE", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Arama parametrelerinde NULL olabilme ihtimaline karşı DBNull.Value kullanıyoruz
                    cmd.Parameters.Add("P_PERSONEL_ADI", OracleDbType.Varchar2).Value = (object)aramaKriterleri.Ad ?? DBNull.Value;
                    cmd.Parameters.Add("P_PERSONEL_SOYADI", OracleDbType.Varchar2).Value = (object)aramaKriterleri.Soyad ?? DBNull.Value;
                    cmd.Parameters.Add("P_PERSONEL_ROLU", OracleDbType.Varchar2).Value = (object)aramaKriterleri.Rol ?? DBNull.Value;
                    cmd.Parameters.Add("P_PERSONEL_TCKN", OracleDbType.Int32).Value = aramaKriterleri.TCKN == 0 ? DBNull.Value : aramaKriterleri.TCKN;
                    cmd.Parameters.Add("P_PERSONEL_TELEFON_NO", OracleDbType.Varchar2).Value = (object)aramaKriterleri.TelefonNo ?? DBNull.Value;
                    cmd.Parameters.Add("P_PERSONEL_SUBEKODU", OracleDbType.Varchar2).Value = (object)aramaKriterleri.SubeKodu ?? DBNull.Value;
                    cmd.Parameters.Add("P_PERSONEL_ADRES", OracleDbType.Varchar2).Value = (object)aramaKriterleri.Adres ?? DBNull.Value;
                    cmd.Parameters.Add("P_PERSONEL_EPOSTA", OracleDbType.Varchar2).Value = (object)aramaKriterleri.Email ?? DBNull.Value;
                    cmd.Parameters.Add("P_PERSONEL_DURUMKODU", OracleDbType.Byte).Value = aramaKriterleri.DurumKodu == 0 ? DBNull.Value : aramaKriterleri.DurumKodu;
                    
                    cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();

                    using (OracleDataReader reader = cmd.ExecuteReader())
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
                using (OracleCommand cmd = new OracleCommand("KB_PERSONEL_GUNCELLE", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("P_PERSONEL_ID", OracleDbType.Int64).Value = dto.Id;
                    cmd.Parameters.Add("P_PERSONEL_ADI", OracleDbType.Varchar2).Value = dto.Ad;
                    cmd.Parameters.Add("P_PERSONEL_SOYADI", OracleDbType.Varchar2).Value = dto.Soyad;
                    cmd.Parameters.Add("P_PERSONEL_ROLU", OracleDbType.Varchar2).Value = dto.Rol;
                    cmd.Parameters.Add("P_PERSONEL_SIFRESI", OracleDbType.Varchar2).Value = dto.Sifre;
                    cmd.Parameters.Add("P_PERSONEL_TCKN", OracleDbType.Int32).Value = dto.TCKN;
                    cmd.Parameters.Add("P_PERSONEL_TELEFON_NO", OracleDbType.Varchar2).Value = dto.TelefonNo;
                    cmd.Parameters.Add("P_PERSONEL_ADRES", OracleDbType.Varchar2).Value = dto.Adres;
                    cmd.Parameters.Add("P_PERSONEL_EPOSTA", OracleDbType.Varchar2).Value = dto.Email;
                    cmd.Parameters.Add("P_PERSONEL_SUBEKODU", OracleDbType.Varchar2).Value = dto.SubeKodu;
                    cmd.Parameters.Add("P_PERSONEL_DURUMKODU", OracleDbType.Byte).Value = dto.DurumKodu;
                    cmd.Parameters.Add("P_PERSONEL_RECORDUSER", OracleDbType.Varchar2).Value = dto.RecordUser;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 5. SİL
        public void Sil(long id)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("KB_PERSONEL_SIL", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("P_PERSONEL_ID", OracleDbType.Int64).Value = id;

                    conn.Open();
                    cmd.ExecuteNonQuery();
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
                DurumKodu = Convert.ToByte(reader["DURUMKODU"]),
                RecordUser = reader["RECORDUSER"].ToString()
                // Eğer SQL tablosunda RecordDate varsa o da buraya eklenebilir.
            };
        }
    }
}