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
            using OracleConnection conn =
                new OracleConnection(_connectionString);

            conn.Open();

            using OracleTransaction transaction =
                conn.BeginTransaction();

            try
            {
                using OracleCommand command =
                    new OracleCommand(
                        "KB_HESAPBILGILERI_EKLE",
                        conn
                    );

                command.CommandType =
                    CommandType.StoredProcedure;

                command.BindByName = true;

                command.Transaction = transaction;


                // IN PARAMETRELERİ

                command.Parameters.Add(
                    "P_HESAPADI",
                    OracleDbType.Varchar2
                ).Value = dto.HesapAdi;

                command.Parameters.Add(
                    "P_SUBESUBEKODU",
                    OracleDbType.Varchar2
                ).Value = dto.SubeSubeKodu;

                command.Parameters.Add(
                    "P_DOVIZCINSI",
                    OracleDbType.Byte
                ).Value = (byte)dto.DovizCinsi;

                command.Parameters.Add(
                    "P_HESAPDURUMKODU",
                    OracleDbType.Byte
                ).Value = (byte)dto.HesapDurumKodu;

                command.Parameters.Add(
                    "P_MUSTERIBILGILERIID",
                    OracleDbType.Int64
                ).Value = dto.MusteriBilgileriId;

                command.Parameters.Add(
                    "P_HESAPTIPI",
                    OracleDbType.Byte
                ).Value = (byte)dto.HesapTipi;

                command.Parameters.Add(
                    "P_RECORDUSER",
                    OracleDbType.Varchar2
                ).Value =
                    string.IsNullOrWhiteSpace(dto.RecordUser)
                        ? DBNull.Value
                        : dto.RecordUser;


                // OUT PARAMETRELERİ

                OracleParameter pYeniId =
                    new OracleParameter(
                        "P_YENI_ID",
                        OracleDbType.Int64
                    )
                    {
                        Direction =
                            ParameterDirection.Output
                    };

                OracleParameter pHesapNo =
                    new OracleParameter(
                        "P_HESAPNO",
                        OracleDbType.Varchar2,
                        16
                    )
                    {
                        Direction =
                            ParameterDirection.Output
                    };

                OracleParameter pIban =
                    new OracleParameter(
                        "P_IBAN",
                        OracleDbType.Varchar2,
                        26
                    )
                    {
                        Direction =
                            ParameterDirection.Output
                    };

                command.Parameters.Add(pYeniId);
                command.Parameters.Add(pHesapNo);
                command.Parameters.Add(pIban);


                // PROSEDÜRÜ ÇALIŞTIR

                command.ExecuteNonQuery();


                // ÜRETİLEN DEĞERLERİ DTO'YA YAZ

                dto.Id =
                    ((OracleDecimal)pYeniId.Value)
                    .ToInt64();

                dto.HesapNo =
                    pHesapNo.Value?.ToString()
                    ?? string.Empty;

                dto.IBAN =
                    pIban.Value?.ToString()
                    ?? string.Empty;

                dto.Bakiye = 0;

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }


            /*
            * Hesabın açılış tarihi, record date ve diğer
            * veritabanı değerlerini doğru şekilde almak için
            * eklenen hesabı tekrar getiriyoruz.
            */
            HesapDTO? olusturulanHesap =
                GetirById(dto.Id);

            return olusturulanHesap ?? dto;
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

        public HesapDTO? GetirByIBAN(string iban)
        {
            HesapDTO? hesap = null;

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_HESAPBILGILERI_GETIRBYIBAN", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;

                    KB.Parameters.Add("P_IBAN", OracleDbType.Varchar2).Value = iban;
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

        // 5. Para çek/yatır işlemi
        public HesapCekYatirDTO ParaCekYatir(HesapCekYatirDTO dto)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                conn.Open();

                using (OracleTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (OracleCommand KB = new OracleCommand("KB_HESAP_CEK_YATIR", conn))
                        {
                            KB.CommandType = CommandType.StoredProcedure;
                            KB.BindByName = true;
                            KB.Transaction = transaction;

                            KB.Parameters.Add("P_HESAPID", OracleDbType.Int64).Value = dto.HesapId;
                            KB.Parameters.Add("P_ISLEMTIPI", OracleDbType.Byte).Value = (byte)dto.IslemTipi;
                            KB.Parameters.Add("P_TUTAR", OracleDbType.Decimal).Value = dto.Tutar;
                            KB.Parameters.Add("P_RECORDUSER", OracleDbType.Varchar2).Value = (object?)dto.RecordUser ?? DBNull.Value;

                            OracleParameter pHareketId = new OracleParameter("P_HAREKETID", OracleDbType.Int64) { Direction = ParameterDirection.Output };
                            OracleParameter pYeniBakiye = new OracleParameter("P_YENIBAKIYE", OracleDbType.Decimal) { Direction = ParameterDirection.Output };

                            KB.Parameters.Add(pHareketId);
                            KB.Parameters.Add(pYeniBakiye);

                            KB.ExecuteNonQuery();

                            dto.HareketId = ((OracleDecimal)pHareketId.Value).ToInt64();
                            dto.YeniBakiye = ((OracleDecimal)pYeniBakiye.Value).Value;

                            transaction.Commit();

                            return dto;
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
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