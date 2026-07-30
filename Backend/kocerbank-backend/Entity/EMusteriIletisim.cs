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
    public class MusteriIletisimRepository
    {
        private readonly string _connectionString;

        public MusteriIletisimRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleConnection") ?? throw new InvalidOperationException("Connection string bulunamadı: 'OracleConnection'");
        }

        public MusteriIletisimDTO? GetirById(long id)
        {
            MusteriIletisimDTO? iletisim = null;

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_MUSTERIILETISIM_GETIRBYMID", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;

                    KB.Parameters.Add("P_ID", OracleDbType.Int64).Value = id;
                    KB.Parameters.Add("P_SONUC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();

                    using (OracleDataReader reader = KB.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            iletisim = MapReaderToDTO(reader);
                        }
                    }
                }
            }
            return iletisim;
        }

        private MusteriIletisimDTO MapReaderToDTO(OracleDataReader reader)
        {
            return new MusteriIletisimDTO
            {
                TelefonNo = reader["TELEFONNO"].ToString()!,
                EvTelefonNo = GetNullableString(reader, "EVTELEFON") ?? string.Empty,
                IsTelefonNo = GetNullableString(reader, "ISTELEFON") ?? string.Empty,
                EvAdres = GetNullableString(reader, "EVADRES") ?? string.Empty,
                IsAdres = GetNullableString(reader, "ISADRES") ?? string.Empty,
                Eposta = reader["EPOSTA"].ToString()!,
                MusteriBilgileriId = Convert.ToInt64(reader["MUSTERIBILGILERIID"])
            };
        }
        private string? GetNullableString(OracleDataReader reader, string columnName)
        {
            var value = reader[columnName];
            return value == DBNull.Value ? null : value.ToString();
        }
    }
}