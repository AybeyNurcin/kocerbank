using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using kocerbank_backend.Models.DTOs;
using Microsoft.Extensions.Configuration;
using kocerbank_backend.Enums;
using Oracle.ManagedDataAccess.Types;

namespace kocerbank_backend.DataAccess
{
    public class MusteriRepository
    {
        private readonly string _connectionString;

        // Bağlantı dizesini appsettings.json'dan almak için IConfiguration kullanıyoruz
        public MusteriRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleConnection") ?? throw new InvalidOperationException("Connection string bulunamadı: 'OracleConnection'");
        }

        // 1. MÜŞTERİ EKLEME
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
                    KB.Parameters.Add("P_DOGUMTARIHI", OracleDbType.Date).Value = dto.DogumTarihi.HasValue ? dto.DogumTarihi.Value : DBNull.Value;
                    KB.Parameters.Add("P_TELEFONNO", OracleDbType.Varchar2).Value = dto.TelefonNo;
                    KB.Parameters.Add("P_TCKN", OracleDbType.Varchar2).Value  = (object?)dto.TCKN ?? DBNull.Value;
                    KB.Parameters.Add("P_CINSIYET", OracleDbType.Byte).Value = dto.Cinsiyet.HasValue ? (object)(byte)dto.Cinsiyet.Value : DBNull.Value;
                    KB.Parameters.Add("P_VKN", OracleDbType.Varchar2).Value = (object?)dto.VKN ?? DBNull.Value;
                    KB.Parameters.Add("P_MUSTERITIPI", OracleDbType.Byte).Value = (byte)dto.MusteriTipi;
                    KB.Parameters.Add("P_SUBESUBEKODU", OracleDbType.Varchar2).Value = dto.SubeSubeKodu;
                    KB.Parameters.Add("P_DURUMKODU", OracleDbType.Byte).Value = (byte)dto.DurumKodu;
                    KB.Parameters.Add("P_UNVAN", OracleDbType.Varchar2).Value = (object?)dto.Unvan ?? DBNull.Value;

                    // OUT Parametreleri
                    OracleParameter pId = new OracleParameter("P_ID", OracleDbType.Int64) { Direction = ParameterDirection.Output };
                    OracleParameter pKayitOlusturmaTarihi = new OracleParameter("P_KAYITOLUSTURMATARIHI", OracleDbType.Date) { Direction = ParameterDirection.Output };
                    
                    KB.Parameters.Add(pId);
                    KB.Parameters.Add(pKayitOlusturmaTarihi);
                    conn.Open();
                    KB.ExecuteNonQuery();

                    dto.Id = ((OracleDecimal)pId.Value).ToInt64();
                    dto.KayitOlusturmaTarihi = ((OracleDate)pKayitOlusturmaTarihi.Value).Value;

                    return dto;
                }
            }
        }

        // 2. ID'YE GÖRE GETİR (READ)
        public MusteriDTO? GetirById(long id)
        {
            MusteriDTO? musteri = null;

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_MUSTERIBILGILERI_GETIRBYID", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;

                    KB.Parameters.Add("P_ID", OracleDbType.Int64).Value = id;
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
        public List<MusteriDTO> Listele(MusteriAramaKriterleriDTO aramaKriterleri)
        {
            List<MusteriDTO> liste = new List<MusteriDTO>();

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_MUSTERIBILGILERI_LISTELE", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;

                    // Arama parametrelerinde NULL olabilme ihtimaline karşı DBNull.Value kullanıyoruz

                    KB.Parameters.Add("P_ID", OracleDbType.Int64).Value = aramaKriterleri.Id.HasValue ? (object)aramaKriterleri.Id.Value : DBNull.Value;
                    KB.Parameters.Add("P_AD", OracleDbType.Varchar2).Value = (object?)aramaKriterleri.Ad ?? DBNull.Value;
                    KB.Parameters.Add("P_SOYAD", OracleDbType.Varchar2).Value = (object?)aramaKriterleri.Soyad ?? DBNull.Value;
                    KB.Parameters.Add("P_EPOSTA", OracleDbType.Varchar2).Value = (object?)aramaKriterleri.Eposta ?? DBNull.Value;
                    KB.Parameters.Add("P_DOGUMTARIHI", OracleDbType.Date).Value = aramaKriterleri.DogumTarihi.HasValue ? (object)aramaKriterleri.DogumTarihi.Value : DBNull.Value;
                    KB.Parameters.Add("P_TELEFONNO", OracleDbType.Varchar2).Value = (object?)aramaKriterleri.TelefonNo ?? DBNull.Value;
                    KB.Parameters.Add("P_TCKN", OracleDbType.Varchar2).Value = (object?)aramaKriterleri.TCKN ?? DBNull.Value;
                    KB.Parameters.Add("P_CINSIYET", OracleDbType.Byte).Value = aramaKriterleri.Cinsiyet.HasValue ? (object)(byte)aramaKriterleri.Cinsiyet.Value : DBNull.Value;
                    KB.Parameters.Add("P_VKN", OracleDbType.Varchar2).Value = (object?)aramaKriterleri.VKN ?? DBNull.Value;
                    KB.Parameters.Add("P_MUSTERITIPI", OracleDbType.Byte).Value = aramaKriterleri.MusteriTipi.HasValue ? (object)(byte)aramaKriterleri.MusteriTipi.Value : DBNull.Value;
                    KB.Parameters.Add("P_SUBESUBEKODU", OracleDbType.Varchar2).Value = (object?)aramaKriterleri.SubeSubeKodu ?? DBNull.Value;
                    KB.Parameters.Add("P_DURUMKODU", OracleDbType.Byte).Value = aramaKriterleri.DurumKodu.HasValue ? (object)(byte)aramaKriterleri.DurumKodu.Value : DBNull.Value;
                    KB.Parameters.Add("P_UNVAN", OracleDbType.Varchar2).Value = (object?)aramaKriterleri.Unvan ?? DBNull.Value;
                    KB.Parameters.Add("P_KAYITOLUSTURMATARIHI", OracleDbType.Date).Value = aramaKriterleri.KayitOlusturmaTarihi.HasValue ? (object)aramaKriterleri.KayitOlusturmaTarihi.Value : DBNull.Value;

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
                    KB.Parameters.Add("P_DOGUMTARIHI", OracleDbType.Date).Value = dto.DogumTarihi;
                    KB.Parameters.Add("P_TELEFONNO", OracleDbType.Varchar2).Value = dto.TelefonNo;
                    KB.Parameters.Add("P_TCKN", OracleDbType.Varchar2).Value = (object?)dto.TCKN ?? DBNull.Value;
                    KB.Parameters.Add("P_CINSIYET", OracleDbType.Byte).Value = dto.Cinsiyet.HasValue ? (object)(byte)dto.Cinsiyet.Value : DBNull.Value;
                    KB.Parameters.Add("P_VKN", OracleDbType.Varchar2).Value = (object?)dto.VKN ?? DBNull.Value;
                    KB.Parameters.Add("P_MUSTERITIPI", OracleDbType.Byte).Value = (byte)dto.MusteriTipi;
                    KB.Parameters.Add("P_SUBESUBEKODU", OracleDbType.Varchar2).Value = dto.SubeSubeKodu;
                    KB.Parameters.Add("P_DURUMKODU", OracleDbType.Byte).Value = (byte)dto.DurumKodu;
                    KB.Parameters.Add("P_UNVAN", OracleDbType.Varchar2).Value = (object?)dto.Unvan ?? DBNull.Value;

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

        public MusteriDashboardDTO GetirDashboardOzet()
        {
            MusteriDashboardDTO ozet = new MusteriDashboardDTO();

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_MUSTERIDASHBOARD", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;

                    KB.Parameters.Add("P_SONUC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();

                    using (OracleDataReader reader = KB.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ozet.ToplamMusteri = Convert.ToInt32(reader["TOPLAMMUSTERI"]);
                            ozet.AktifSayi     = Convert.ToInt32(reader["AKTIFSAYI"]);
                            ozet.PasifSayi     = Convert.ToInt32(reader["PASIFSAYI"]);
                            ozet.BireyselSayi  = Convert.ToInt32(reader["BIREYSELSAYI"]);
                            ozet.KurumsalSayi  = Convert.ToInt32(reader["KURUMSALSAYI"]);
                        }
                    }
                }
            }

            return ozet;
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
                DogumTarihi = reader["DOGUMTARIHI"] == DBNull.Value ? null : Convert.ToDateTime(reader["DOGUMTARIHI"]),
                TelefonNo = reader["TELEFONNO"].ToString()!,
                TCKN = GetNullableString(reader, "TCKN"),
                Cinsiyet = reader["CINSIYET"] == DBNull.Value ? (CinsiyetDurumlari?) null : (CinsiyetDurumlari)Convert.ToByte(reader["CINSIYET"]),
                VKN = GetNullableString(reader, "VKN"),
                MusteriTipi = (MusteriTipiDurumlari)Convert.ToByte(reader["MUSTERITIPI"]),
                SubeSubeKodu = reader["SUBESUBEKODU"].ToString()!,
                DurumKodu = (AktifPasifDurumlari)Convert.ToByte(reader["DURUMKODU"]),
                Unvan = GetNullableString(reader, "UNVAN"),
                KayitOlusturmaTarihi = Convert.ToDateTime(reader["KAYITOLUSTURMATARIHI"]),
                RecordUser = reader["RECORDUSER"].ToString()!,
                RecordDate = Convert.ToDateTime(reader["RECORDDATE"])
            };
        }
        private string? GetNullableString(OracleDataReader reader, string columnName)
        {
            var value = reader[columnName];
            return value == DBNull.Value ? null : value.ToString();
        }
    }
}