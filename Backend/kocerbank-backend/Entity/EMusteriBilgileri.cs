using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using kocerbank_backend.Models.DTOs;
using Microsoft.Extensions.Configuration;
using kocerbank_backend.Enums;

namespace kocerbank_backend.DataAccess
{
    public class MusteriRepository
    {
        private readonly string _connectionString;

        // Bağlantı dizesini appsettings.json'dan almak için IConfiguration kullanıyoruz
        public MusteriRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleConnection");
        }

        // 1. PERSONEL EKLEME
        public MusteriDTO Ekle(MusteriDTO dto)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_MUSTERIBILGILERI_EKLE", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;

                    KB.Parameters.Add("P_AD", OracleDbType.Varchar2).Value = dto.Ad;
                    KB.Parameters.Add("P_SOYAD", OracleDbType.Varchar2).Value = dto.Soyad;
                    KB.Parameters.Add("P_EPOSTA", OracleDbType.Varchar2).Value = dto.Eposta;
                    KB.Parameters.Add("P_SIFRE", OracleDbType.Varchar2).Value = dto.Sifre;
                    KB.Parameters.Add("P_DOGUMTARIHI", OracleDbType.Date).Value = dto.DogumTarihi;
                    KB.Parameters.Add("P_TELEFONNO", OracleDbType.Varchar2).Value = dto.TelefonNo;
                    KB.Parameters.Add("P_TCKN", OracleDbType.Int32).Value = dto.TCKN;
                    KB.Parameters.Add("P_CINSIYET", OracleDbType.Byte).Value = (byte)dto.Cinsiyet;
                    KB.Parameters.Add("P_VKN", OracleDbType.Int32).Value = dto.VKN;
                    KB.Parameters.Add("P_MUSTERITIPI", OracleDbType.Byte).Value = (byte)dto.MusteriTipi;
                    KB.Parameters.Add("P_SUBESUBEKODU", OracleDbType.Varchar2).Value = dto.SubeSubeKodu;
                    KB.Parameters.Add("P_DURUMKODU", OracleDbType.Byte).Value = (byte)dto.DurumKodu;
                    KB.Parameters.Add("P_UNVAN", OracleDbType.Varchar2).Value = dto.Unvan;
                    KB.Parameters.Add("P_KAYITOLUSTURMATARIHI", OracleDbType.Date).Value = dto.KayitOlusturmaTarihi;

                    // OUT Parametreleri
                    OracleParameter pId = new OracleParameter("P_ID", OracleDbType.Int64) { Direction = ParameterDirection.Output };
                    
                    KB.Parameters.Add(pId);

                    conn.Open();
                    KB.ExecuteNonQuery();

                    // Üretilen değerleri DTO'ya geri yazıyoruz
                    dto.Id = Convert.ToInt64(pId.Value.ToString());

                    return dto;
                }
            }
        }

        // 2. ID'YE GÖRE GETİR (READ)
        public MusteriDTO GetirById(long id)
        {
            MusteriDTO musteri = null;

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_MUSTERIBILGILERI_GETIR", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;

                    KB.Parameters.Add("P_ID", OracleDbType.Int64).Value = id;
                    
                    // Oracle'daki SYS_REFCURSOR'u C# tarafında okumak için RefCursor tipi eklenir
                    KB.Parameters.Add("P_SONUC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    
                    // Cursor verisini okumak için OracleDataReader kullanıyoruz
                    using (OracleDataReader reader = KB.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            musteri = MapReaderToDTO(reader);
                        }
                    }
                }
            }
            return musteri;
        }

        // 3. KRİTERE GÖRE LİSTELE
        public List<MusteriDTO> Listele(MusteriDTO aramaKriterleri)
        {
            List<MusteriDTO> liste = new List<MusteriDTO>();

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_MUSTERIBILGILERI_LISTELE", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;

                    // Arama parametrelerinde NULL olabilme ihtimaline karşı DBNull.Value kullanıyoruz

                    KB.Parameters.Add("P_AD", OracleDbType.Varchar2).Value = (object)aramaKriterleri.Ad ?? DBNull.Value;
                    KB.Parameters.Add("P_SOYAD", OracleDbType.Varchar2).Value = (object)aramaKriterleri.Soyad ?? DBNull.Value;
                    KB.Parameters.Add("P_EPOSTA", OracleDbType.Varchar2).Value = (object)aramaKriterleri.Eposta ?? DBNull.Value;
                    KB.Parameters.Add("DOGUMTARIHI", OracleDbType.Date).Value = aramaKriterleri.DogumTarihi.HasValue ? (object)aramaKriterleri.DogumTarihi : DBNull.Value;
                    KB.Parameters.Add("P_TELEFONNO", OracleDbType.Varchar2).Value = (object)aramaKriterleri.TelefonNo ?? DBNull.Value;
                    KB.Parameters.Add("P_TCKN", OracleDbType.Int32).Value = aramaKriterleri.TCKN == 0 ? DBNull.Value : aramaKriterleri.TCKN;
                    KB.Parameters.Add("P_CINSIYET", OracleDbType.Byte).Value = aramaKriterleri.Cinsiyet == CinsiyetDurumlari.None ? DBNull.Value : aramaKriterleri.Cinsiyet;
                    KB.Parameters.Add("P_VKN", OracleDbType.Int32).Value = aramaKriterleri.VKN == 0 ? DBNull.Value : aramaKriterleri.VKN;
                    KB.Parameters.Add("P_MUSTERITIPI", OracleDbType.Byte).Value = aramaKriterleri.MusteriTipi == MusteriTipiDurumlari.None ? DBNull.Value : aramaKriterleri.MusteriTipi;
                    KB.Parameters.Add("P_SUBESUBEKODU", OracleDbType.Varchar2).Value = (object)aramaKriterleri.SubeSubeKodu ?? DBNull.Value;
                    KB.Parameters.Add("P_DURUMKODU", OracleDbType.Byte).Value = aramaKriterleri.DurumKodu == AktifPasifDurumlari.None ? DBNull.Value : aramaKriterleri.DurumKodu;
                    KB.Parameters.Add("P_UNVAN", OracleDbType.Varchar2).Value = (object)aramaKriterleri.Unvan ?? DBNull.Value;
                    KB.Parameters.Add("P_KAYITOLUSTURMATARIHI", OracleDbType.Date).Value = aramaKriterleri.KayitOlusturmaTarihi == DateTime.MinValue ? DBNull.Value : (object)aramaKriterleri.KayitOlusturmaTarihi;
                    
                    KB.Parameters.Add("P_SONUC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

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
        public void Guncelle(MusteriDTO dto)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_MUSTERIBILGILERI_GUNCELLE", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;


                    KB.Parameters.Add("P_ID", OracleDbType.Int64).Value = dto.Id;
                    KB.Parameters.Add("P_AD", OracleDbType.Varchar2).Value = dto.Ad;
                    KB.Parameters.Add("P_SOYAD", OracleDbType.Varchar2).Value = dto.Soyad;
                    KB.Parameters.Add("P_EPOSTA", OracleDbType.Varchar2).Value = dto.Eposta;
                    KB.Parameters.Add("P_SIFRE", OracleDbType.Varchar2).Value = dto.Sifre;
                    KB.Parameters.Add("P_DOGUMTARIHI", OracleDbType.Date).Value = dto.DogumTarihi;
                    KB.Parameters.Add("P_TELEFONNO", OracleDbType.Varchar2).Value = dto.TelefonNo;
                    KB.Parameters.Add("P_TCKN", OracleDbType.Int32).Value = dto.TCKN;
                    KB.Parameters.Add("P_CINSIYET", OracleDbType.Byte).Value = (byte)dto.Cinsiyet;
                    KB.Parameters.Add("P_VKN", OracleDbType.Int32).Value = dto.VKN;
                    KB.Parameters.Add("P_MUSTERITIPI", OracleDbType.Byte).Value = (byte)dto.MusteriTipi;
                    KB.Parameters.Add("P_SUBESUBEKODU", OracleDbType.Varchar2).Value = dto.SubeSubeKodu;
                    KB.Parameters.Add("P_DURUMKODU", OracleDbType.Byte).Value = (byte)dto.DurumKodu;
                    KB.Parameters.Add("P_UNVAN", OracleDbType.Varchar2).Value = dto.Unvan;

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
                using (OracleCommand KB = new OracleCommand("KB_MUSTERIBILGILERI_SIL", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;
                    KB.Parameters.Add("P_ID", OracleDbType.Int64).Value = id;

                    conn.Open();
                    KB.ExecuteNonQuery();
                }
            }
        }

        // YARDIMCI METOT: Veritabanı satırını DTO nesnesine dönüştürür (Kod tekrarını önler)
        private MusteriDTO MapReaderToDTO(OracleDataReader reader)
        {
            return new MusteriDTO
            {
                Id = Convert.ToInt64(reader["ID"]),
                Ad = reader["AD"].ToString()!,
                Soyad = reader["SOYAD"].ToString()!,
                Eposta = reader["EPOSTA"].ToString()!,
                Sifre = reader["SIFRE"].ToString()!,
                DogumTarihi = Convert.ToDateTime(reader["DOGUMTARIHI"]),
                TelefonNo = reader["TELEFONNO"].ToString()!,
                TCKN = Convert.ToInt32(reader["TCKN"]),
                Cinsiyet = (CinsiyetDurumlari)Convert.ToByte(reader["CINSIYET"]),
                VKN = Convert.ToInt32(reader["VKN"]),
                MusteriTipi = (MusteriTipiDurumlari)Convert.ToByte(reader["MUSTERITIPI"]),
                SubeSubeKodu = reader["SUBESUBEKODU"].ToString()!,
                DurumKodu = (AktifPasifDurumlari)Convert.ToByte(reader["DURUMKODU"]),
                Unvan = reader["UNVAN"].ToString()!,
                KayitOlusturmaTarihi = Convert.ToDateTime(reader["KAYITOLUSTURMATARIHI"]),
                RecordUser = reader["RECORDUSER"].ToString()!,
                RecordDate = Convert.ToDateTime(reader["RECORDDATE"])
                // Eğer SQL tablosunda RecordDate varsa o da buraya eklenebilir.
            };
        }
    }
}