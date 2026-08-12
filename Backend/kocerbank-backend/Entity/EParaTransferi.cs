using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using kocerbank_backend.Enums;
using kocerbank_backend.Models.DTOs;

namespace kocerbank_backend.DataAccess
{
    public class ParaTransferRepository
    {
        private readonly string _connectionString;

        public ParaTransferRepository(
            IConfiguration configuration
        )
        {
            _connectionString =
                configuration.GetConnectionString(
                    "OracleConnection"
                )
                ?? throw new InvalidOperationException(
                    "Connection string bulunamadı: 'OracleConnection'"
                );
        }


        // PARA TRANSFERİ YAPMA

        public ParaTransferDTO ParaTransferiYap(
            ParaTransferDTO dto
        )
        {
            using (
                OracleConnection conn =
                    new OracleConnection(
                        _connectionString
                    )
            )
            {
                conn.Open();

                using (
                    OracleTransaction transaction =
                        conn.BeginTransaction()
                )
                {
                    try
                    {
                        using (
                            OracleCommand KB =
                                new OracleCommand(
                                    "KB_PARA_TRANSFERI_YAP",
                                    conn
                                )
                        )
                        {
                            KB.CommandType =
                                CommandType.StoredProcedure;

                            KB.BindByName = true;

                            KB.Transaction = transaction;


                            // IN PARAMETRELERİ

                            KB.Parameters.Add(
                                "P_GONDERENHESAPID",
                                OracleDbType.Int64
                            ).Value =
                                dto.GonderenHesapId;


                            KB.Parameters.Add(
                                "P_ALICIHESAPID",
                                OracleDbType.Int64
                            ).Value =
                                dto.AliciHesapId!.Value;


                            KB.Parameters.Add(
                                "P_TRANSFERTIPI",
                                OracleDbType.Byte
                            ).Value =
                                (byte)dto.TransferTipi;


                            KB.Parameters.Add(
                                "P_GONDERENTUTAR",
                                OracleDbType.Decimal
                            ).Value =
                                dto.GonderenTutar;


                            KB.Parameters.Add(
                                "P_DOVIZKURU",
                                OracleDbType.Decimal
                            ).Value =
                                dto.DovizKuru;


                            KB.Parameters.Add(
                                "P_ACIKLAMA",
                                OracleDbType.Varchar2
                            ).Value =
                                string.IsNullOrWhiteSpace(
                                    dto.Aciklama
                                )
                                    ? DBNull.Value
                                    : dto.Aciklama.Trim();


                            KB.Parameters.Add(
                                "P_RECORDUSER",
                                OracleDbType.Varchar2
                            ).Value =
                                (object?)dto.RecordUser ?? DBNull.Value;


                            // OUT PARAMETRELERİ

                            OracleParameter pTransferId =
                                new OracleParameter(
                                    "P_TRANSFERID",
                                    OracleDbType.Int64
                                )
                                {
                                    Direction =
                                        ParameterDirection.Output
                                };


                            OracleParameter pGonderenHareketId =
                                new OracleParameter(
                                    "P_GONDERENHAREKETID",
                                    OracleDbType.Int64
                                )
                                {
                                    Direction =
                                        ParameterDirection.Output
                                };


                            OracleParameter pAliciHareketId =
                                new OracleParameter(
                                    "P_ALICIHAREKETID",
                                    OracleDbType.Int64
                                )
                                {
                                    Direction =
                                        ParameterDirection.Output
                                };


                            OracleParameter pGonderenYeniBakiye =
                                new OracleParameter(
                                    "P_GONDERENYENIBAKIYE",
                                    OracleDbType.Decimal
                                )
                                {
                                    Direction =
                                        ParameterDirection.Output
                                };


                            OracleParameter pAliciYeniBakiye =
                                new OracleParameter(
                                    "P_ALICIYENIBAKIYE",
                                    OracleDbType.Decimal
                                )
                                {
                                    Direction =
                                        ParameterDirection.Output
                                };


                            OracleParameter pAliciTutar =
                                new OracleParameter(
                                    "P_ALICITUTAR",
                                    OracleDbType.Decimal
                                )
                                {
                                    Direction =
                                        ParameterDirection.Output
                                };


                            KB.Parameters.Add(pTransferId);

                            KB.Parameters.Add(
                                pGonderenHareketId
                            );

                            KB.Parameters.Add(
                                pAliciHareketId
                            );

                            KB.Parameters.Add(
                                pGonderenYeniBakiye
                            );

                            KB.Parameters.Add(
                                pAliciYeniBakiye
                            );

                            KB.Parameters.Add(
                                pAliciTutar
                            );


                            // PROSEDÜRÜ ÇALIŞTIR

                            KB.ExecuteNonQuery();


                            // OUT DEĞERLERİNİ DTO'YA AKTAR

                            dto.TransferId =
                                ((OracleDecimal)
                                    pTransferId.Value
                                ).ToInt64();


                            dto.GonderenHareketId =
                                ((OracleDecimal)
                                    pGonderenHareketId.Value
                                ).ToInt64();


                            dto.AliciHareketId =
                                ((OracleDecimal)
                                    pAliciHareketId.Value
                                ).ToInt64();


                            dto.GonderenYeniBakiye =
                                ((OracleDecimal)
                                    pGonderenYeniBakiye.Value
                                ).Value;


                            dto.AliciYeniBakiye =
                                ((OracleDecimal)
                                    pAliciYeniBakiye.Value
                                ).Value;


                            dto.AliciTutar =
                                ((OracleDecimal)
                                    pAliciTutar.Value
                                ).Value;


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


        // EFT YAPMA (ALICININ BİZİM BANKAMIZDA HESABI YOKSA)

        public ParaTransferDTO EftTransferiYap(
            ParaTransferDTO dto
        )
        {
            using (
                OracleConnection conn =
                    new OracleConnection(
                        _connectionString
                    )
            )
            {
                conn.Open();

                using (
                    OracleTransaction transaction =
                        conn.BeginTransaction()
                )
                {
                    try
                    {
                        using (
                            OracleCommand KB =
                                new OracleCommand(
                                    "KB_EFT_TRANSFERI_YAP",
                                    conn
                                )
                        )
                        {
                            KB.CommandType =
                                CommandType.StoredProcedure;

                            KB.BindByName = true;

                            KB.Transaction = transaction;


                            // IN PARAMETRELERİ

                            KB.Parameters.Add(
                                "P_GONDERENHESAPID",
                                OracleDbType.Int64
                            ).Value =
                                dto.GonderenHesapId;


                            KB.Parameters.Add(
                                "P_ALICIIBAN",
                                OracleDbType.Varchar2
                            ).Value =
                                dto.AliciIBAN;


                            KB.Parameters.Add(
                                "P_ALICIADSOYAD",
                                OracleDbType.Varchar2
                            ).Value =
                                (object?)dto.AliciAdSoyad ?? DBNull.Value;


                            KB.Parameters.Add(
                                "P_GONDERENTUTAR",
                                OracleDbType.Decimal
                            ).Value =
                                dto.GonderenTutar;


                            KB.Parameters.Add(
                                "P_ACIKLAMA",
                                OracleDbType.Varchar2
                            ).Value =
                                string.IsNullOrWhiteSpace(
                                    dto.Aciklama
                                )
                                    ? DBNull.Value
                                    : dto.Aciklama.Trim();


                            KB.Parameters.Add(
                                "P_RECORDUSER",
                                OracleDbType.Varchar2
                            ).Value =
                                (object?)dto.RecordUser ?? DBNull.Value;


                            // OUT PARAMETRELERİ

                            OracleParameter pTransferId =
                                new OracleParameter(
                                    "P_TRANSFERID",
                                    OracleDbType.Int64
                                )
                                {
                                    Direction =
                                        ParameterDirection.Output
                                };


                            OracleParameter pGonderenHareketId =
                                new OracleParameter(
                                    "P_GONDERENHAREKETID",
                                    OracleDbType.Int64
                                )
                                {
                                    Direction =
                                        ParameterDirection.Output
                                };


                            OracleParameter pGonderenYeniBakiye =
                                new OracleParameter(
                                    "P_GONDERENYENIBAKIYE",
                                    OracleDbType.Decimal
                                )
                                {
                                    Direction =
                                        ParameterDirection.Output
                                };


                            KB.Parameters.Add(pTransferId);

                            KB.Parameters.Add(
                                pGonderenHareketId
                            );

                            KB.Parameters.Add(
                                pGonderenYeniBakiye
                            );


                            // PROSEDÜRÜ ÇALIŞTIR

                            KB.ExecuteNonQuery();


                            // OUT DEĞERLERİNİ DTO'YA AKTAR

                            dto.TransferId =
                                ((OracleDecimal)
                                    pTransferId.Value
                                ).ToInt64();


                            dto.GonderenHareketId =
                                ((OracleDecimal)
                                    pGonderenHareketId.Value
                                ).ToInt64();


                            dto.GonderenYeniBakiye =
                                ((OracleDecimal)
                                    pGonderenYeniBakiye.Value
                                ).Value;


                            // EFT'de alıcı tarafında iç hesap
                            // güncellenmediği için alıcı tutarı
                            // gönderilen tutarla aynıdır (kur 1).
                            dto.AliciTutar =
                                dto.GonderenTutar;

                            dto.AliciYeniBakiye =
                                0;

                            dto.AliciHareketId =
                                0;


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


        // ID'YE GÖRE GETİR (TRANSFER DETAYI İÇİN)

        public ParaTransferDTO? GetirById(
            long id
        )
        {
            ParaTransferDTO? transfer = null;

            using (
                OracleConnection conn =
                    new OracleConnection(
                        _connectionString
                    )
            )
            {
                using (
                    OracleCommand KB =
                        new OracleCommand(
                            "KB_PARATRANSFERI_GETIRBYID",
                            conn
                        )
                )
                {
                    KB.CommandType =
                        CommandType.StoredProcedure;

                    KB.Parameters.Add(
                        "P_ID",
                        OracleDbType.Int64
                    ).Value = id;

                    KB.Parameters.Add(
                        "P_SONUC",
                        OracleDbType.RefCursor
                    ).Direction =
                        ParameterDirection.Output;

                    conn.Open();

                    using (
                        OracleDataReader reader =
                            KB.ExecuteReader()
                    )
                    {
                        if (reader.Read())
                        {
                            transfer =
                                MapReaderToDTO(reader);
                        }
                    }
                }
            }

            return transfer;
        }


        private ParaTransferDTO MapReaderToDTO(
            OracleDataReader reader
        )
        {
            return new ParaTransferDTO
            {
                TransferId =
                    Convert.ToInt64(reader["ID"]),

                GonderenHesapId =
                    Convert.ToInt64(reader["GONDERENHESAPID"]),

                // EFT kayıtlarında iç alıcı hesabı yoktur.
                AliciHesapId =
                    reader["ALICIHESAPID"] == DBNull.Value
                        ? null
                        : Convert.ToInt64(reader["ALICIHESAPID"]),

                AliciIBAN =
                    reader["ALICIIBAN"] == DBNull.Value
                        ? string.Empty
                        : reader["ALICIIBAN"].ToString() ?? string.Empty,

                AliciAdSoyad =
                    reader["ALICIADSOYAD"] == DBNull.Value
                        ? null
                        : reader["ALICIADSOYAD"].ToString(),

                TransferTipi =
                    (TransferTipleri)
                        Convert.ToByte(reader["TRANSFERTIPI"]),

                GonderenTutar =
                    Convert.ToDecimal(reader["GONDERENTUTAR"]),

                GonderenDovizTipi =
                    (DovizCinsiDurumlari)
                        Convert.ToByte(reader["GONDERENDOVIZTIPI"]),

                AliciDovizTipi =
                    (DovizCinsiDurumlari)
                        Convert.ToByte(reader["ALICIDOVIZTIPI"]),

                DovizKuru =
                    Convert.ToDecimal(reader["DOVIZKURU"]),

                Aciklama =
                    reader["ACIKLAMA"] == DBNull.Value
                        ? null
                        : reader["ACIKLAMA"].ToString(),

                RecordUser =
                    reader["RECORDUSER"] == DBNull.Value
                        ? null
                        : reader["RECORDUSER"].ToString()
            };
        }
    }
}