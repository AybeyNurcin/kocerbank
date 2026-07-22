using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using kocerbank_backend.Models.DTOs;
using Microsoft.Extensions.Configuration;

namespace kocerbank_backend.DataAccess
{
    public class SubeRepository
    {
        private readonly string _connectionString;

        // Bağlantı dizesini appsettings.json'dan almak için IConfiguration kullanıyoruz
        public SubeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleConnection")
            ?? throw new InvalidOperationException("OracleConnection bağlantı bilgisi bulunamadı.");
        }

        // 1. SUBE EKLEME
        public SubeDTO Ekle(SubeDTO dto)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand kb = new OracleCommand("KB_SUBE_EKLE", conn))
                {
                    kb.CommandType = CommandType.StoredProcedure;
                    kb.BindByName = true;


                    // IN Parametreleri
                    kb.Parameters.Add("P_SUBE_ADI", OracleDbType.Varchar2).Value = dto.SubeAdi;
                    kb.Parameters.Add("P_SUBE_TELEFON_NO", OracleDbType.Varchar2).Value = dto.SubeTelefonNo;
                    kb.Parameters.Add("P_SUBE_ADRES", OracleDbType.Varchar2).Value = dto.SubeAdres;
                    kb.Parameters.Add("P_SUBE_DURUM_KODU", OracleDbType.Byte).Value = dto.SubeDurumKodu;
                    kb.Parameters.Add("P_RECORD_USER", OracleDbType.Varchar2).Value = dto.RecordUser;
                    // OUT Parametreleri
                    OracleParameter sId = new OracleParameter("p_yeni_id", OracleDbType.Int64) { Direction = ParameterDirection.Output };
                    OracleParameter sKodu = new OracleParameter("p_yeni_sube_kodu", OracleDbType.Varchar2, 50) { Direction = ParameterDirection.Output };
                    
                    kb.Parameters.Add(sId);
                    kb.Parameters.Add(sKodu);

                    conn.Open();
                    kb.ExecuteNonQuery();

                    // Üretilen değerleri DTO'ya geri yazıyoruz
                    dto.Id = Convert.ToInt64(sId.Value.ToString());
                    dto.SubeKodu = sKodu.Value.ToString();

                    return dto;
                }
            }
        }

        // 2. ID'YE GÖRE GETİR (READ)
        public SubeDTO GetirById(long id)
        {
            SubeDTO sube = null;

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand kb = new OracleCommand("KB_SUBE_GETIR", conn))
                {
                    kb.CommandType = CommandType.StoredProcedure;
                    kb.BindByName = true;

                    kb.Parameters.Add("P_ID", OracleDbType.Int64).Value = id;
                    
                    // Oracle'daki SYS_REFCURSOR'u C# tarafında okumak için RefCursor tipi eklenir
                    kb.Parameters.Add("P_SONUC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    
                    // Cursor verisini okumak için OracleDataReader kullanıyoruz
                    using (OracleDataReader reader = kb.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            sube = MapReaderToDTO(reader);
                        }
                    }
                }
            }
            return sube;
        }

        // 3. KRİTERE GÖRE LİSTELE
        public List<SubeDTO> GetirListele(SubeDTO aramaKriterleri)
        {
            List<SubeDTO> liste = new List<SubeDTO>();

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand kb = new OracleCommand("KB_SUBE_LISTELE", conn))
                {
                    kb.CommandType = CommandType.StoredProcedure;
                    kb.BindByName = true;
                    // Arama parametrelerinde NULL olabilme ihtimaline karşı DBNull.Value kullanıyoruz
                    kb.Parameters.Add("P_SUBE_ADI", OracleDbType.Varchar2).Value = (object)aramaKriterleri.SubeAdi ?? DBNull.Value;
                    kb.Parameters.Add("p_sube_kodu", OracleDbType.Varchar2).Value = (object)aramaKriterleri.SubeKodu ?? DBNull.Value;
                    kb.Parameters.Add("P_SUBE_TELEFON_NO", OracleDbType.Varchar2).Value = (object)aramaKriterleri.SubeTelefonNo ?? DBNull.Value;
                    kb.Parameters.Add("P_SUBE_ADRES", OracleDbType.Varchar2).Value = (object)aramaKriterleri.SubeAdres ?? DBNull.Value;
                    kb.Parameters.Add("P_SUBE_DURUM_KODU", OracleDbType.Byte).Value = aramaKriterleri.SubeDurumKodu == 0 ? DBNull.Value : aramaKriterleri.SubeDurumKodu;

                    kb.Parameters.Add("p_sonuc", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();

                    using (OracleDataReader reader = kb.ExecuteReader())
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
        public void Guncelle(SubeDTO dto)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand kb = new OracleCommand("KB_SUBE_GUNCELLE", conn))
                {
                    kb.CommandType = CommandType.StoredProcedure;
                    kb.BindByName = true;

                    kb.Parameters.Add("P_ID", OracleDbType.Int64).Value = dto.Id;
                    kb.Parameters.Add("P_SUBE_ADI", OracleDbType.Varchar2).Value = dto.SubeAdi;
                    kb.Parameters.Add("P_SUBE_TELEFON_NO", OracleDbType.Varchar2).Value = dto.SubeTelefonNo;
                    kb.Parameters.Add("P_SUBE_ADRES", OracleDbType.Varchar2).Value = dto.SubeAdres;
                    kb.Parameters.Add("P_SUBE_DURUM_KODU", OracleDbType.Byte).Value = dto.SubeDurumKodu;

                    conn.Open();
                    kb.ExecuteNonQuery();
                }
            }
        }

        // 5. SİL
        public void Sil(long id)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand kb = new OracleCommand("KB_SUBE_SIL", conn))
                {
                    kb.CommandType = CommandType.StoredProcedure;
                    kb.BindByName = true;

                    kb.Parameters.Add("P_ID", OracleDbType.Int64).Value = id;

                    conn.Open();
                    kb.ExecuteNonQuery();
                }
            }
        }

        // YARDIMCI METOT: Veritabanı satırını DTO nesnesine dönüştürür (Kod tekrarını önler)
        private SubeDTO MapReaderToDTO(OracleDataReader reader)
        {
            return new SubeDTO
            {
                Id = Convert.ToInt64(reader["ID"]),
                SubeAdi = reader["SUBEADI"].ToString()!,
                SubeKodu = reader["SUBEKODU"].ToString()!,
                SubeTelefonNo = reader["SUBETELEFONNO"].ToString()!,
                SubeAdres = reader["SUBEADRES"].ToString()!,
                SubeDurumKodu = Convert.ToByte(reader["SUBEDURUMKODU"]),
                RecordUser = reader["RECORDUSER"].ToString()!,
                RecordDate = Convert.ToDateTime(reader["RECORDDATE"])
            };
        }
    }
}