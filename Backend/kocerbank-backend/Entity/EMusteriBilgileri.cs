using System.Data;
using kocerbank_backend.Enums;
using kocerbank_backend.Models.DTOs;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace kocerbank_backend.DataAccess
{
    public class MusteriRepository
    {
        private readonly string _connectionString;

        public MusteriRepository(
            IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString(
                    "OracleConnection")
                ?? throw new InvalidOperationException(
                    "Connection string bulunamadı: 'OracleConnection'");
        }

        // 1. MÜŞTERİ EKLEME
        public MusteriDTO Ekle(MusteriDTO dto)
        {
            using OracleConnection conn =
                new OracleConnection(_connectionString);

            using OracleCommand command =
                new OracleCommand(
                    "KB_MUSTERIBILGILERI_EKLE",
                    conn);

            command.CommandType =
                CommandType.StoredProcedure;

            command.BindByName = true;

            command.Parameters
                .Add("P_AD", OracleDbType.Varchar2)
                .Value = dto.Ad;

            command.Parameters
                .Add("P_SOYAD", OracleDbType.Varchar2)
                .Value = dto.Soyad;

            command.Parameters
                .Add("P_EPOSTA", OracleDbType.Varchar2)
                .Value = dto.Eposta;

            command.Parameters
                .Add("P_DOGUMTARIHI", OracleDbType.Date)
                .Value =
                    dto.DogumTarihi.HasValue
                        ? dto.DogumTarihi.Value
                        : DBNull.Value;

            command.Parameters
                .Add("P_TELEFONNO", OracleDbType.Varchar2)
                .Value = dto.TelefonNo;

            command.Parameters
                .Add("P_TCKN", OracleDbType.Varchar2)
                .Value =
                    (object?)dto.TCKN ??
                    DBNull.Value;

            command.Parameters
                .Add("P_CINSIYET", OracleDbType.Byte)
                .Value =
                    dto.Cinsiyet.HasValue
                        ? (byte)dto.Cinsiyet.Value
                        : DBNull.Value;

            command.Parameters
                .Add("P_VKN", OracleDbType.Varchar2)
                .Value =
                    (object?)dto.VKN ??
                    DBNull.Value;

            command.Parameters
                .Add("P_MUSTERITIPI", OracleDbType.Byte)
                .Value =
                    (byte)dto.MusteriTipi;

            command.Parameters
                .Add("P_SUBESUBEKODU", OracleDbType.Varchar2)
                .Value =
                    dto.SubeSubeKodu;

            command.Parameters
                .Add("P_DURUMKODU", OracleDbType.Byte)
                .Value =
                    (byte)dto.DurumKodu;

            command.Parameters
                .Add("P_UNVAN", OracleDbType.Varchar2)
                .Value =
                    (object?)dto.Unvan ??
                    DBNull.Value;

            command.Parameters
                .Add("P_RECORDUSER", OracleDbType.Varchar2)
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

            OracleParameter pKayitOlusturmaTarihi =
                new OracleParameter(
                    "P_KAYITOLUSTURMATARIHI",
                    OracleDbType.Date)
                {
                    Direction =
                        ParameterDirection.Output
                };

            command.Parameters.Add(pId);
            command.Parameters.Add(
                pKayitOlusturmaTarihi);

            conn.Open();
            command.ExecuteNonQuery();

            dto.Id =
                ((OracleDecimal)pId.Value)
                .ToInt64();

            dto.KayitOlusturmaTarihi =
                OracleZamanDamgasi.UtcOlarakOku(
                    pKayitOlusturmaTarihi.Value);

            return dto;
        }

        // 2. ID'YE GÖRE MÜŞTERİ GETİRME
        public MusteriDTO? GetirById(long id)
        {
            MusteriDTO? musteri = null;

            using OracleConnection conn =
                new OracleConnection(_connectionString);

            using OracleCommand command =
                new OracleCommand(
                    "KB_MUSTERIBILGILERI_GETIRBYID",
                    conn);

            command.CommandType =
                CommandType.StoredProcedure;

            command.BindByName = true;

            command.Parameters
                .Add("P_ID", OracleDbType.Int64)
                .Value = id;

            command.Parameters
                .Add("P_SONUC", OracleDbType.RefCursor)
                .Direction =
                    ParameterDirection.Output;

            conn.Open();

            using OracleDataReader reader =
                command.ExecuteReader();

            if (reader.Read())
            {
                musteri =
                    MapReaderToDTO(reader);
            }

            return musteri;
        }

        // MÜŞTERİ VE İLETİŞİM BİLGİLERİNİ
        // TEK İŞLEMDE EKLEME
        public MusteriTamKaydetSonucDTO TamKaydet(
            MusteriTamKaydetDTO dto)
        {
            using OracleConnection conn =
                new OracleConnection(_connectionString);

            using OracleCommand command =
                new OracleCommand(
                    "KB_MUSTERI_TAM_EKLE",
                    conn);

            command.CommandType =
                CommandType.StoredProcedure;

            command.BindByName = true;

            // MÜŞTERİ BİLGİLERİ

            command.Parameters
                .Add("P_AD", OracleDbType.Varchar2)
                .Value =
                    dto.Musteri.Ad;

            command.Parameters
                .Add("P_SOYAD", OracleDbType.Varchar2)
                .Value =
                    dto.Musteri.Soyad;

            command.Parameters
                .Add("P_EPOSTA", OracleDbType.Varchar2)
                .Value =
                    dto.Musteri.Eposta;

            command.Parameters
                .Add("P_DOGUMTARIHI", OracleDbType.Date)
                .Value =
                    dto.Musteri.DogumTarihi.HasValue
                        ? dto.Musteri.DogumTarihi.Value
                        : DBNull.Value;

            command.Parameters
                .Add("P_TELEFONNO", OracleDbType.Varchar2)
                .Value =
                    dto.Musteri.TelefonNo;

            command.Parameters
                .Add("P_TCKN", OracleDbType.Varchar2)
                .Value =
                    (object?)dto.Musteri.TCKN ??
                    DBNull.Value;

            command.Parameters
                .Add("P_CINSIYET", OracleDbType.Byte)
                .Value =
                    dto.Musteri.Cinsiyet.HasValue
                        ? (byte)dto.Musteri.Cinsiyet.Value
                        : DBNull.Value;

            command.Parameters
                .Add("P_VKN", OracleDbType.Varchar2)
                .Value =
                    (object?)dto.Musteri.VKN ??
                    DBNull.Value;

            command.Parameters
                .Add("P_MUSTERITIPI", OracleDbType.Byte)
                .Value =
                    (byte)dto.Musteri.MusteriTipi;

            command.Parameters
                .Add("P_SUBESUBEKODU", OracleDbType.Varchar2)
                .Value =
                    dto.Musteri.SubeSubeKodu;

            command.Parameters
                .Add("P_DURUMKODU", OracleDbType.Byte)
                .Value =
                    (byte)dto.Musteri.DurumKodu;

            command.Parameters
                .Add("P_UNVAN", OracleDbType.Varchar2)
                .Value =
                    (object?)dto.Musteri.Unvan ??
                    DBNull.Value;

            // Giriş yapan personelin sicili.
            // Üst prosedür bu değeri hem müşteri
            // hem de iletişim kaydına aktarır.
            command.Parameters
                .Add("P_RECORDUSER", OracleDbType.Varchar2)
                .Value =
                    (object?)dto.Musteri.RecordUser ??
                    DBNull.Value;

            // İLETİŞİM BİLGİLERİ

            command.Parameters
                .Add("P_EVTELEFON", OracleDbType.Varchar2)
                .Value =
                    (object?)dto.Iletisim.EvTelefonNo ??
                    DBNull.Value;

            command.Parameters
                .Add("P_ISTELEFON", OracleDbType.Varchar2)
                .Value =
                    (object?)dto.Iletisim.IsTelefonNo ??
                    DBNull.Value;

            command.Parameters
                .Add("P_EVADRES", OracleDbType.Varchar2)
                .Value =
                    (object?)dto.Iletisim.EvAdres ??
                    DBNull.Value;

            command.Parameters
                .Add("P_ISADRES", OracleDbType.Varchar2)
                .Value =
                    (object?)dto.Iletisim.IsAdres ??
                    DBNull.Value;

            // OUT PARAMETRELERİ

            OracleParameter musteriId =
                new OracleParameter(
                    "P_MUSTERI_ID",
                    OracleDbType.Int64)
                {
                    Direction =
                        ParameterDirection.Output
                };

            OracleParameter iletisimId =
                new OracleParameter(
                    "P_ILETISIM_ID",
                    OracleDbType.Int64)
                {
                    Direction =
                        ParameterDirection.Output
                };

            OracleParameter kayitTarihi =
                new OracleParameter(
                    "P_KAYITOLUSTURMATARIHI",
                    OracleDbType.Date)
                {
                    Direction =
                        ParameterDirection.Output
                };

            command.Parameters.Add(musteriId);
            command.Parameters.Add(iletisimId);
            command.Parameters.Add(kayitTarihi);

            conn.Open();
            command.ExecuteNonQuery();

            return new MusteriTamKaydetSonucDTO
            {
                MusteriId =
                    ((OracleDecimal)musteriId.Value)
                    .ToInt64(),

                IletisimId =
                    ((OracleDecimal)iletisimId.Value)
                    .ToInt64(),

                KayitOlusturmaTarihi =
                    OracleZamanDamgasi.UtcOlarakOku(
                        kayitTarihi.Value)
            };
        }

        // 3. KRİTERE GÖRE MÜŞTERİ LİSTELEME
        public List<MusteriDTO> Listele(
            MusteriAramaKriterleriDTO aramaKriterleri)
        {
            List<MusteriDTO> liste = new();

            using OracleConnection conn =
                new OracleConnection(_connectionString);

            using OracleCommand command =
                new OracleCommand(
                    "KB_MUSTERIBILGILERI_LISTELE",
                    conn);

            command.CommandType =
                CommandType.StoredProcedure;

            command.BindByName = true;

            command.Parameters
                .Add("P_ID", OracleDbType.Int64)
                .Value =
                    aramaKriterleri.Id.HasValue
                        ? aramaKriterleri.Id.Value
                        : DBNull.Value;

            command.Parameters
                .Add("P_AD", OracleDbType.Varchar2)
                .Value =
                    (object?)aramaKriterleri.Ad ??
                    DBNull.Value;

            command.Parameters
                .Add("P_SOYAD", OracleDbType.Varchar2)
                .Value =
                    (object?)aramaKriterleri.Soyad ??
                    DBNull.Value;

            command.Parameters
                .Add("P_EPOSTA", OracleDbType.Varchar2)
                .Value =
                    (object?)aramaKriterleri.Eposta ??
                    DBNull.Value;

            command.Parameters
                .Add("P_DOGUMTARIHIBASLANGIC", OracleDbType.Date)
                .Value =
                    aramaKriterleri.DogumTarihiBaslangic.HasValue
                        ? aramaKriterleri.DogumTarihiBaslangic.Value
                        : DBNull.Value;

            command.Parameters
                .Add("P_DOGUMTARIHIBITIS", OracleDbType.Date)
                .Value =
                    aramaKriterleri.DogumTarihiBitis.HasValue
                        ? aramaKriterleri.DogumTarihiBitis.Value
                        : DBNull.Value;

            command.Parameters
                .Add("P_TELEFONNO", OracleDbType.Varchar2)
                .Value =
                    (object?)aramaKriterleri.TelefonNo ??
                    DBNull.Value;

            command.Parameters
                .Add("P_TCKN", OracleDbType.Varchar2)
                .Value =
                    (object?)aramaKriterleri.TCKN ??
                    DBNull.Value;

            command.Parameters
                .Add("P_CINSIYET", OracleDbType.Byte)
                .Value =
                    aramaKriterleri.Cinsiyet.HasValue
                        ? (byte)aramaKriterleri.Cinsiyet.Value
                        : DBNull.Value;

            command.Parameters
                .Add("P_VKN", OracleDbType.Varchar2)
                .Value =
                    (object?)aramaKriterleri.VKN ??
                    DBNull.Value;

            command.Parameters
                .Add("P_MUSTERITIPI", OracleDbType.Byte)
                .Value =
                    aramaKriterleri.MusteriTipi.HasValue
                        ? (byte)aramaKriterleri.MusteriTipi.Value
                        : DBNull.Value;

            command.Parameters
                .Add("P_SUBESUBEKODU", OracleDbType.Varchar2)
                .Value =
                    (object?)aramaKriterleri.SubeSubeKodu ??
                    DBNull.Value;

            command.Parameters
                .Add("P_DURUMKODU", OracleDbType.Byte)
                .Value =
                    aramaKriterleri.DurumKodu.HasValue
                        ? (byte)aramaKriterleri.DurumKodu.Value
                        : DBNull.Value;

            command.Parameters
                .Add("P_UNVAN", OracleDbType.Varchar2)
                .Value =
                    (object?)aramaKriterleri.Unvan ??
                    DBNull.Value;

            command.Parameters
                .Add(
                    "P_KAYITOLUSTURMATARIHI",
                    OracleDbType.Date)
                .Value =
                    aramaKriterleri
                        .KayitOlusturmaTarihi
                        .HasValue
                            ? aramaKriterleri
                                .KayitOlusturmaTarihi
                                .Value
                            : DBNull.Value;

            command.Parameters
                .Add("P_SONUC", OracleDbType.RefCursor)
                .Direction =
                    ParameterDirection.Output;

            conn.Open();

            using OracleDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                liste.Add(
                    MapReaderToDTO(reader));
            }

            return liste;
        }

        // 4. MÜŞTERİ GÜNCELLEME
        public void Guncelle(MusteriDTO dto)
        {
            using OracleConnection conn =
                new OracleConnection(_connectionString);

            using OracleCommand command =
                new OracleCommand(
                    "KB_MUSTERIBILGILERI_GUNCELLE",
                    conn);

            command.CommandType =
                CommandType.StoredProcedure;

            command.BindByName = true;

            command.Parameters
                .Add("P_ID", OracleDbType.Int64)
                .Value = dto.Id;

            command.Parameters
                .Add("P_RECORDUSER", OracleDbType.Varchar2)
                .Value =
                    (object?)dto.RecordUser ??
                    DBNull.Value;

            command.Parameters
                .Add("P_AD", OracleDbType.Varchar2)
                .Value = dto.Ad;

            command.Parameters
                .Add("P_SOYAD", OracleDbType.Varchar2)
                .Value = dto.Soyad;

            command.Parameters
                .Add("P_EPOSTA", OracleDbType.Varchar2)
                .Value = dto.Eposta;

            command.Parameters
                .Add("P_DOGUMTARIHI", OracleDbType.Date)
                .Value =
                    dto.DogumTarihi.HasValue
                        ? dto.DogumTarihi.Value
                        : DBNull.Value;

            command.Parameters
                .Add("P_TELEFONNO", OracleDbType.Varchar2)
                .Value = dto.TelefonNo;

            command.Parameters
                .Add("P_TCKN", OracleDbType.Varchar2)
                .Value =
                    (object?)dto.TCKN ??
                    DBNull.Value;

            command.Parameters
                .Add("P_CINSIYET", OracleDbType.Byte)
                .Value =
                    dto.Cinsiyet.HasValue
                        ? (byte)dto.Cinsiyet.Value
                        : DBNull.Value;

            command.Parameters
                .Add("P_VKN", OracleDbType.Varchar2)
                .Value =
                    (object?)dto.VKN ??
                    DBNull.Value;

            command.Parameters
                .Add("P_MUSTERITIPI", OracleDbType.Byte)
                .Value =
                    (byte)dto.MusteriTipi;

            command.Parameters
                .Add("P_SUBESUBEKODU", OracleDbType.Varchar2)
                .Value =
                    dto.SubeSubeKodu;

            command.Parameters
                .Add("P_DURUMKODU", OracleDbType.Byte)
                .Value =
                    (byte)dto.DurumKodu;

            command.Parameters
                .Add("P_UNVAN", OracleDbType.Varchar2)
                .Value =
                    (object?)dto.Unvan ??
                    DBNull.Value;

            conn.Open();
            command.ExecuteNonQuery();
        }

        // 5. MÜŞTERİ SİLME
        public void Sil(long id)
        {
            using OracleConnection conn =
                new OracleConnection(_connectionString);

            using OracleCommand command =
                new OracleCommand(
                    "KB_MUSTERIBILGILERI_SIL",
                    conn);

            command.CommandType =
                CommandType.StoredProcedure;

            command.BindByName = true;

            command.Parameters
                .Add("P_ID", OracleDbType.Int64)
                .Value = id;

            conn.Open();
            command.ExecuteNonQuery();
        }

        // DASHBOARD ÖZETİ
        public MusteriDashboardDTO GetirDashboardOzet(DateTime? baslangicTarihi, DateTime? bitisTarihi)
        {
            MusteriDashboardDTO ozet = new();

            using OracleConnection conn =
                new OracleConnection(_connectionString);

            using OracleCommand command =
                new OracleCommand(
                    "KB_MUSTERIDASHBOARD",
                    conn);

            command.CommandType =
                CommandType.StoredProcedure;

            command.BindByName = true;

            command.Parameters
                .Add("P_BASLANGICTARIHI", OracleDbType.Date)
                .Value = (object?)baslangicTarihi ?? DBNull.Value;

            command.Parameters
                .Add("P_BITISTARIHI", OracleDbType.Date)
                .Value = (object?)bitisTarihi ?? DBNull.Value;

            command.Parameters
                .Add("P_SONUC", OracleDbType.RefCursor)
                .Direction =
                    ParameterDirection.Output;

            conn.Open();

            using OracleDataReader reader =
                command.ExecuteReader();

            if (reader.Read())
            {
                ozet.ToplamMusteri =
                    Convert.ToInt64(
                        reader["MUSTERI_SAYISI"]);

                ozet.AktifSayi =
                    Convert.ToInt64(
                        reader["AKTIFSAYI"]);

                ozet.PasifSayi =
                    Convert.ToInt64(
                        reader["PASIFSAYI"]);

                ozet.BireyselSayi =
                    Convert.ToInt64(
                        reader["BIREYSELSAYI"]);

                ozet.KurumsalSayi =
                    Convert.ToInt64(
                        reader["KURUMSALSAYI"]);
            }

            return ozet;
        }

        // VERİTABANI SATIRINI DTO'YA DÖNÜŞTÜRÜR
        private static MusteriDTO MapReaderToDTO(
            OracleDataReader reader)
        {
            return new MusteriDTO
            {
                Id =
                    Convert.ToInt64(reader["ID"]),

                Ad =
                    reader["AD"].ToString()!,

                Soyad =
                    reader["SOYAD"].ToString()!,

                Eposta =
                    reader["EPOSTA"].ToString()!,

                DogumTarihi =
                    reader["DOGUMTARIHI"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(
                            reader["DOGUMTARIHI"]),

                TelefonNo =
                    reader["TELEFONNO"].ToString()!,

                TCKN =
                    GetNullableString(
                        reader,
                        "TCKN"),

                Cinsiyet =
                    reader["CINSIYET"] == DBNull.Value
                        ? null
                        : (CinsiyetDurumlari)
                            Convert.ToByte(
                                reader["CINSIYET"]),

                VKN =
                    GetNullableString(
                        reader,
                        "VKN"),

                MusteriTipi =
                    (MusteriTipiDurumlari)
                        Convert.ToByte(
                            reader["MUSTERITIPI"]),

                SubeSubeKodu =
                    reader["SUBESUBEKODU"]
                        .ToString()!,

                DurumKodu =
                    (AktifPasifDurumlari)
                        Convert.ToByte(
                            reader["DURUMKODU"]),

                Unvan =
                    GetNullableString(
                        reader,
                        "UNVAN"),

                KayitOlusturmaTarihi =
                    OracleZamanDamgasi.UtcOlarakOku(
                        reader["KAYITOLUSTURMATARIHI"]),

                RecordUser =
                    GetNullableString(
                        reader,
                        "RECORDUSER"),

                RecordDate =
                    reader["RECORDDATE"] == DBNull.Value
                        ? null
                        : OracleZamanDamgasi.UtcOlarakOku(
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