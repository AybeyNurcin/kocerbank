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
    public class HesapHareketiRepository
    {
        private readonly string _connectionString;

        // Bağlantı dizesini appsettings.json'dan almak için IConfiguration kullanıyoruz
        public HesapHareketiRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleConnection") ?? throw new InvalidOperationException("Connection string bulunamadı: 'OracleConnection'");
        }

        public List<HesapHareketiDTO> Listele(long hesapBilgileriId)
        {
            List<HesapHareketiDTO> liste = new List<HesapHareketiDTO>();

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_HESAPHAREKETI_LISTELE", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;

                    KB.Parameters.Add("P_HESAPBILGILERIID", OracleDbType.Int64).Value = hesapBilgileriId;
                    KB.Parameters.Add("P_SONUC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();

                    // Cursor verisini okumak için OracleDataReader kullanıyoruz
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

        private HesapHareketiDTO MapReaderToDTO(OracleDataReader reader)
        {
            return new HesapHareketiDTO
            {
                Id = Convert.ToInt64(reader["ID"]),
                HesapBilgileriId = Convert.ToInt64(reader["HESAPBILGILERIID"]),
                ParaTransferiId = reader["PARATRANSFERIID"] == DBNull.Value ? null : Convert.ToInt64(reader["PARATRANSFERIID"]),
                HesapHareketiTipi = (HesapHareketTipleri)Convert.ToByte(reader["HAREKETTIPI"]),
                Tutar = Convert.ToInt32(reader["TUTAR"]),
                DovizCinsi = (DovizCinsiDurumlari)Convert.ToByte(reader["DOVIZCINSI"]),
                OncekiBakiye = Convert.ToInt32(reader["ONCEKIBAKIYE"]),
                SonrakiBakiye = Convert.ToInt32(reader["SONRAKIBAKIYE"]),
                IslemTarihi = OracleZamanDamgasi.UtcOlarakOku(reader["ISLEMTARIHI"]),
                RecordUser = reader["RECORDUSER"] == DBNull.Value ? null : reader["RECORDUSER"].ToString(),
                RecordDate = reader["RECORDDATE"] == DBNull.Value ? null : OracleZamanDamgasi.UtcOlarakOku(reader["RECORDDATE"])
            };
        }
    }
}