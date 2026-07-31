using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using Microsoft.Extensions.Configuration;
using kocerbank_backend.Enums;
using kocerbank_backend.Models.DTOs;

namespace kocerbank_backend.DataAccess
{
    public class HesapRepository
    {
        private readonly string _connectionString;

        // Bağlantı dizesini appsettings.json'dan almak için IConfiguration kullanıyoruz
        public HesapRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleConnection")
                ?? throw new InvalidOperationException("Connection string bulunamadı: 'OracleConnection'");
        }

        // 1. HESAP EKLEME
        public HesapDTO Ekle(HesapDTO dto)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_HESAPBILGILERI_EKLE", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;

                    KB.Parameters.Add("P_HESAPADI", OracleDbType.Varchar2).Value = dto.HesapAdi;
                    KB.Parameters.Add("P_HESAPNO", OracleDbType.Varchar2).Value = dto.HesapNo;
                    KB.Parameters.Add("P_IBAN", OracleDbType.Varchar2).Value = dto.IBAN;
                    KB.Parameters.Add("P_BAKIYE", OracleDbType.Decimal).Value = dto.Bakiye;
                    KB.Parameters.Add("P_SUBESUBEKODU", OracleDbType.Varchar2).Value = dto.SubeSubeKodu;
                    KB.Parameters.Add("P_DOVIZCINSI", OracleDbType.Byte).Value = (byte)dto.DovizCinsi;
                    KB.Parameters.Add("P_HESAPDURUMKODU", OracleDbType.Byte).Value = (byte)dto.HesapDurumKodu;
                    KB.Parameters.Add("P_MUSTERIBILGILERIID", OracleDbType.Int64).Value = dto.MusteriBilgileriId;
                    KB.Parameters.Add("P_HESAPTIPI", OracleDbType.Byte).Value = (byte)dto.HesapTipi;

                    // OUT Parametreleri
                    OracleParameter pId = new OracleParameter("P_ID", OracleDbType.Int64)
                    {
                        Direction = ParameterDirection.Output
                    };

                    OracleParameter pHesapAcilisTarihi = new OracleParameter("P_HESAPACILISTARIHI", OracleDbType.Date)
                    {
                        Direction = ParameterDirection.Output
                    };

                    KB.Parameters.Add(pId);
                    KB.Parameters.Add(pHesapAcilisTarihi);

                    conn.Open();
                    KB.ExecuteNonQuery();

                    dto.Id = ((OracleDecimal)pId.Value).ToInt64();
                    dto.HesapAcilisTarihi = ((OracleDate)pHesapAcilisTarihi.Value).Value;

                    return dto;
                }
            }
        }

                // 2. ID'YE GÖRE GETİR (READ)
        public HesapDTO? GetirById(long id)
        {
            HesapDTO? hesap = null;

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_HESAPBILGILERI_GETIRBYID", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;

                    KB.Parameters.Add("P_ID", OracleDbType.Int64).Value = id;
                    KB.Parameters.Add("P_SONUC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();

                    using (OracleDataReader reader = KB.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            hesap = MapReaderToDTO(reader);
                        }
                    }
                }
            }

            return hesap;
        }

        // 3. KRİTERE GÖRE LİSTELE
        public List<HesapDTO> Listele(HesapAramaKriterleriDTO aramaKriterleri)
        {
            List<HesapDTO> liste = new List<HesapDTO>();

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_HESAPBILGILERI_LISTELE", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;

                    KB.Parameters.Add("P_ID", OracleDbType.Int64).Value =
                        aramaKriterleri.Id.HasValue ? (object)aramaKriterleri.Id.Value : DBNull.Value;

                    KB.Parameters.Add("P_HESAPADI", OracleDbType.Varchar2).Value =
                        (object?)aramaKriterleri.HesapAdi ?? DBNull.Value;

                    KB.Parameters.Add("P_HESAPNO", OracleDbType.Varchar2).Value =
                        (object?)aramaKriterleri.HesapNo ?? DBNull.Value;

                    KB.Parameters.Add("P_IBAN", OracleDbType.Varchar2).Value =
                        (object?)aramaKriterleri.IBAN ?? DBNull.Value;

                    KB.Parameters.Add("P_BAKIYE", OracleDbType.Decimal).Value =
                        aramaKriterleri.Bakiye.HasValue ? (object)aramaKriterleri.Bakiye.Value : DBNull.Value;

                    KB.Parameters.Add("P_SUBESUBEKODU", OracleDbType.Varchar2).Value =
                        (object?)aramaKriterleri.SubeSubeKodu ?? DBNull.Value;

                    KB.Parameters.Add("P_DOVIZCINSI", OracleDbType.Byte).Value =
                        aramaKriterleri.DovizCinsi.HasValue
                            ? (object)(byte)aramaKriterleri.DovizCinsi.Value
                            : DBNull.Value;

                    KB.Parameters.Add("P_HESAPACILISTARIHI", OracleDbType.Date).Value =
                        aramaKriterleri.HesapAcilisTarihi.HasValue
                            ? (object)aramaKriterleri.HesapAcilisTarihi.Value
                            : DBNull.Value;

                    KB.Parameters.Add("P_HESAPDURUMKODU", OracleDbType.Byte).Value =
                        aramaKriterleri.HesapDurumKodu.HasValue
                            ? (object)(byte)aramaKriterleri.HesapDurumKodu.Value
                            : DBNull.Value;

                    KB.Parameters.Add("P_MUSTERIBILGILERIID", OracleDbType.Int64).Value =
                        aramaKriterleri.MusteriBilgileriId.HasValue
                            ? (object)aramaKriterleri.MusteriBilgileriId.Value
                            : DBNull.Value;

                    KB.Parameters.Add("P_HESAPTIPI", OracleDbType.Byte).Value =
                        aramaKriterleri.HesapTipi.HasValue
                            ? (object)(byte)aramaKriterleri.HesapTipi.Value
                            : DBNull.Value;

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
        public void Guncelle(HesapDTO dto)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_HESAPBILGILERI_GUNCELLE", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;

                    KB.Parameters.Add("P_ID", OracleDbType.Int64).Value = dto.Id;
                    KB.Parameters.Add("P_HESAPADI", OracleDbType.Varchar2).Value = dto.HesapAdi;
                    KB.Parameters.Add("P_HESAPNO", OracleDbType.Varchar2).Value = dto.HesapNo;
                    KB.Parameters.Add("P_IBAN", OracleDbType.Varchar2).Value = dto.IBAN;
                    KB.Parameters.Add("P_BAKIYE", OracleDbType.Decimal).Value = dto.Bakiye;
                    KB.Parameters.Add("P_SUBESUBEKODU", OracleDbType.Varchar2).Value = dto.SubeSubeKodu;
                    KB.Parameters.Add("P_DOVIZCINSI", OracleDbType.Byte).Value = (byte)dto.DovizCinsi;
                    KB.Parameters.Add("P_HESAPDURUMKODU", OracleDbType.Byte).Value = (byte)dto.HesapDurumKodu;
                    KB.Parameters.Add("P_MUSTERIBILGILERIID", OracleDbType.Int64).Value = dto.MusteriBilgileriId;
                    KB.Parameters.Add("P_HESAPTIPI", OracleDbType.Byte).Value = (byte)dto.HesapTipi;

                    conn.Open();
                    KB.ExecuteNonQuery();
                }
            }
        }

        // YARDIMCI METOT: Veritabanı satırını DTO nesnesine dönüştürür (Kod tekrarını önler)
        private HesapDTO MapReaderToDTO(OracleDataReader reader)
        {
            return new HesapDTO
            {
                Id = Convert.ToInt64(reader["ID"]),
                HesapAdi = reader["HESAPADI"].ToString()!,
                HesapNo = reader["HESAPNO"].ToString()!,
                IBAN = reader["IBAN"].ToString()!,
                Bakiye = Convert.ToDecimal(reader["BAKIYE"]),
                SubeSubeKodu = reader["SUBESUBEKODU"].ToString()!,
                DovizCinsi = (DovizCinsiDurumlari)Convert.ToByte(reader["DOVIZCINSI"]),
                HesapAcilisTarihi = Convert.ToDateTime(reader["HESAPACILISTARIHI"]),
                HesapDurumKodu = (HesapDurumKodlari)Convert.ToByte(reader["HESAPDURUMKODU"]),
                MusteriBilgileriId = Convert.ToInt64(reader["MUSTERIBILGILERIID"]),
                HesapTipi = (HesapTipiDurumlari)Convert.ToByte(reader["HESAPTIPI"]),
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