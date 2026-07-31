using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using kocerbank_backend.Models.DTOs;
using Microsoft.Extensions.Configuration;
using kocerbank_backend.Enums;

namespace kocerbank_backend.DataAccess
{
    public class ParaTransferiRepository
    {
        private readonly string _connectionString;

        // Bağlantı dizesini appsettings.json'dan almak için IConfiguration kullanıyoruz
        public ParaTransferiRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleConnection")
                ?? throw new InvalidOperationException("Connection string bulunamadı: 'OracleConnection'");
        }

        // 1. PARA TRANSFERİ EKLEME
        public ParaTransferiDTO Ekle(ParaTransferiDTO dto)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_PARATRANSFERI_EKLE", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;

                    // IN Parametreleri
                    KB.Parameters.Add("P_GONDERENIBAN", OracleDbType.Varchar2).Value = dto.GonderenIBAN;
                    KB.Parameters.Add("P_ALICIIBAN", OracleDbType.Varchar2).Value = dto.AliciIBAN;
                    KB.Parameters.Add("P_TRANSFERTIPI", OracleDbType.Byte).Value = (byte)dto.TransferTipi;
                    KB.Parameters.Add("P_MIKTAR", OracleDbType.Decimal).Value = dto.Miktar;
                    KB.Parameters.Add("P_ACIKLAMA", OracleDbType.Varchar2).Value = dto.Aciklama;
                    KB.Parameters.Add("P_GONDERENDOVIZTIPI", OracleDbType.Byte).Value = (byte)dto.GonderenDovizTipi;
                    KB.Parameters.Add("P_ALICIDOVIZTIPI", OracleDbType.Byte).Value = (byte)dto.AliciDovizTipi;
                    KB.Parameters.Add("P_RECORDUSER", OracleDbType.Varchar2).Value = dto.RecordUser;

                    // OUT Parametresi
                    OracleParameter pId = new OracleParameter("P_ID", OracleDbType.Int64)
                    {
                        Direction = ParameterDirection.Output
                    };

                    KB.Parameters.Add(pId);

                    conn.Open();
                    KB.ExecuteNonQuery();

                    dto.Id = Convert.ToInt64(pId.Value.ToString());

                    return dto;
                }
            }
        }

        // 2. ID'YE GÖRE GETİR
        public ParaTransferiDTO? GetirById(long id)
        {
            ParaTransferiDTO? transfer = null;

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_PARATRANSFERI_GETIRBYID", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;

                    KB.Parameters.Add("P_ID", OracleDbType.Int64).Value = id;
                    KB.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();

                    using (OracleDataReader reader = KB.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            transfer = MapReaderToDTO(reader);
                        }
                    }
                }
            }

            return transfer;
        }

        // 3. GÜNCELLE
        public void Guncelle(ParaTransferiDTO dto)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_PARATRANSFERI_GUNCELLE", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;

                    KB.Parameters.Add("P_ID", OracleDbType.Int64).Value = dto.Id;
                    KB.Parameters.Add("P_GONDERENIBAN", OracleDbType.Varchar2).Value = dto.GonderenIBAN;
                    KB.Parameters.Add("P_ALICIIBAN", OracleDbType.Varchar2).Value = dto.AliciIBAN;
                    KB.Parameters.Add("P_TRANSFERTIPI", OracleDbType.Byte).Value = (byte)dto.TransferTipi;
                    KB.Parameters.Add("P_MIKTAR", OracleDbType.Decimal).Value = dto.Miktar;
                    KB.Parameters.Add("P_ACIKLAMA", OracleDbType.Varchar2).Value = dto.Aciklama;
                    KB.Parameters.Add("P_GONDERENDOVIZTIPI", OracleDbType.Byte).Value = (byte)dto.GonderenDovizTipi;
                    KB.Parameters.Add("P_ALICIDOVIZTIPI", OracleDbType.Byte).Value = (byte)dto.AliciDovizTipi;
                    KB.Parameters.Add("P_RECORDUSER", OracleDbType.Varchar2).Value = dto.RecordUser;

                    conn.Open();
                    KB.ExecuteNonQuery();
                }
            }
        }

        // YARDIMCI METOT: Veritabanı satırını DTO nesnesine dönüştürür
        private ParaTransferiDTO MapReaderToDTO(OracleDataReader reader)
        {
            return new ParaTransferiDTO
            {
                Id = Convert.ToInt64(reader["ID"]),
                GonderenIBAN = reader["GONDERENIBAN"].ToString()!,
                AliciIBAN = reader["ALICIIBAN"].ToString()!,
                TransferTipi = (TransferTipleri)Convert.ToByte(reader["TRANSFERTIPI"]),
                Miktar = Convert.ToDecimal(reader["MIKTAR"]),
                Aciklama = reader["ACIKLAMA"].ToString()!,
                GonderenDovizTipi = (DovizTipleri)Convert.ToByte(reader["GONDERENDOVIZTIPI"]),
                AliciDovizTipi = (DovizTipleri)Convert.ToByte(reader["ALICIDOVIZTIPI"]),
                TarihSaat = Convert.ToDateTime(reader["TARIHSAAT"]),
                RecordUser = reader["RECORDUSER"].ToString()
            };
        }
    }
}