using System.Data;
using kocerbank_backend.Models.DTOs;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace kocerbank_backend.DataAccess
{
    public class MusteriIletisimRepository
    {
        private readonly string _connectionString;

        public MusteriIletisimRepository(
            IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString(
                    "OracleConnection")
                ?? throw new InvalidOperationException(
                    "Connection string bulunamadı: 'OracleConnection'");
        }

        // 1. İLETİŞİM BİLGİSİ EKLEME
        public MusteriIletisimDTO Ekle(
            MusteriIletisimDTO dto)
        {
            using OracleConnection conn =
                new OracleConnection(_connectionString);

            using OracleCommand command =
                new OracleCommand(
                    "KB_MUSTERIILETISIM_EKLE",
                    conn);

            command.CommandType =
                CommandType.StoredProcedure;

            command.BindByName = true;

            command.Parameters
                .Add(
                    "P_MUSTERIBILGILERIID",
                    OracleDbType.Int64)
                .Value =
                    dto.MusteriBilgileriId;

            command.Parameters
                .Add(
                    "P_TELEFONNO",
                    OracleDbType.Varchar2)
                .Value =
                    dto.TelefonNo;

            command.Parameters
                .Add(
                    "P_EVTELEFON",
                    OracleDbType.Varchar2)
                .Value =
                    (object?)dto.EvTelefonNo ??
                    DBNull.Value;

            command.Parameters
                .Add(
                    "P_ISTELEFON",
                    OracleDbType.Varchar2)
                .Value =
                    (object?)dto.IsTelefonNo ??
                    DBNull.Value;

            command.Parameters
                .Add(
                    "P_EPOSTA",
                    OracleDbType.Varchar2)
                .Value =
                    dto.Eposta;

            command.Parameters
                .Add(
                    "P_EVADRES",
                    OracleDbType.Varchar2)
                .Value =
                    (object?)dto.EvAdres ??
                    DBNull.Value;

            command.Parameters
                .Add(
                    "P_ISADRES",
                    OracleDbType.Varchar2)
                .Value =
                    (object?)dto.IsAdres ??
                    DBNull.Value;

            // Giriş yapan personelin sicili
            command.Parameters
                .Add(
                    "P_RECORDUSER",
                    OracleDbType.Varchar2)
                .Value =
                    (object?)dto.RecordUser ??
                    DBNull.Value;

            OracleParameter pId =
                new OracleParameter(
                    "P_ID",
                    OracleDbType.Int64)
                {
                    Direction =
                        ParameterDirection.Output
                };

            command.Parameters.Add(pId);

            conn.Open();
            command.ExecuteNonQuery();

            dto.Id =
                ((OracleDecimal)pId.Value)
                .ToInt64();

            return dto;
        }

        // 2. MÜŞTERİ ID'SİNE GÖRE
        // İLETİŞİM BİLGİSİ GETİRME
        public MusteriIletisimDTO? GetirById(long id)
        {
            MusteriIletisimDTO? iletisim = null;

            using OracleConnection conn =
                new OracleConnection(_connectionString);

            using OracleCommand command =
                new OracleCommand(
                    "KB_MUSTERIILETISIM_GETIRBYMID",
                    conn);

            command.CommandType =
                CommandType.StoredProcedure;

            command.BindByName = true;

            command.Parameters
                .Add(
                    "P_ID",
                    OracleDbType.Int64)
                .Value = id;

            command.Parameters
                .Add(
                    "P_SONUC",
                    OracleDbType.RefCursor)
                .Direction =
                    ParameterDirection.Output;

            conn.Open();

            using OracleDataReader reader =
                command.ExecuteReader();

            if (reader.Read())
            {
                iletisim =
                    MapReaderToDTO(reader);
            }

            return iletisim;
        }

        // 3. MÜŞTERİ VE İLETİŞİM TABLOLARINDAKİ
        // İLETİŞİM BİLGİLERİNİ GÜNCELLEME
        public void Guncelle(
            MusteriIletisimAramaKriterleriDTO dto)
        {
            using OracleConnection conn =
                new OracleConnection(_connectionString);

            using OracleCommand command =
                new OracleCommand(
                    "KB_MUSTERI_ILETISIM_TAM_GUNCELLE",
                    conn);

            command.CommandType =
                CommandType.StoredProcedure;

            command.BindByName = true;

            command.Parameters
                .Add(
                    "P_MUSTERIBILGILERIID",
                    OracleDbType.Int64)
                .Value =
                    dto.MusteriBilgileriId;

            // Güncellemeyi yapan personelin sicili
            command.Parameters
                .Add(
                    "P_RECORDUSER",
                    OracleDbType.Varchar2)
                .Value =
                    (object?)dto.RecordUser ??
                    DBNull.Value;

            command.Parameters
                .Add(
                    "P_TELEFONNO",
                    OracleDbType.Varchar2)
                .Value =
                    (object?)dto.TelefonNo ??
                    DBNull.Value;

            command.Parameters
                .Add(
                    "P_EPOSTA",
                    OracleDbType.Varchar2)
                .Value =
                    (object?)dto.Eposta ??
                    DBNull.Value;

            command.Parameters
                .Add(
                    "P_EVTELEFON",
                    OracleDbType.Varchar2)
                .Value =
                    (object?)dto.EvTelefonNo ??
                    DBNull.Value;

            command.Parameters
                .Add(
                    "P_ISTELEFON",
                    OracleDbType.Varchar2)
                .Value =
                    (object?)dto.IsTelefonNo ??
                    DBNull.Value;

            command.Parameters
                .Add(
                    "P_EVADRES",
                    OracleDbType.Varchar2)
                .Value =
                    (object?)dto.EvAdres ??
                    DBNull.Value;

            command.Parameters
                .Add(
                    "P_ISADRES",
                    OracleDbType.Varchar2)
                .Value =
                    (object?)dto.IsAdres ??
                    DBNull.Value;

            conn.Open();
            command.ExecuteNonQuery();
        }

        // VERİTABANI SATIRINI DTO'YA DÖNÜŞTÜRÜR
        private static MusteriIletisimDTO MapReaderToDTO(
            OracleDataReader reader)
        {
            return new MusteriIletisimDTO
            {
                Id =
                    Convert.ToInt64(
                        reader["ID"]),

                MusteriBilgileriId =
                    Convert.ToInt64(
                        reader["MUSTERIBILGILERIID"]),

                TelefonNo =
                    reader["TELEFONNO"]
                        .ToString()!,

                EvTelefonNo =
                    GetNullableString(
                        reader,
                        "EVTELEFON"),

                IsTelefonNo =
                    GetNullableString(
                        reader,
                        "ISTELEFON"),

                Eposta =
                    reader["EPOSTA"]
                        .ToString()!,

                EvAdres =
                    GetNullableString(
                        reader,
                        "EVADRES"),

                IsAdres =
                    GetNullableString(
                        reader,
                        "ISADRES"),

                RecordUser =
                    GetNullableString(
                        reader,
                        "RECORDUSER"),

                RecordDate =
                    reader["RECORDDATE"] ==
                    DBNull.Value
                        ? null
                        : Convert.ToDateTime(
                            reader["RECORDDATE"])
            };
        }

        private static string? GetNullableString(
            OracleDataReader reader,
            string columnName)
        {
            object value =
                reader[columnName];

            return value == DBNull.Value
                ? null
                : value.ToString();
        }
    }
}