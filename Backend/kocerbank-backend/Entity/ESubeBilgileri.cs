using System.Data;
using Oracle.ManagedDataAccess.Client;
using kocerbank_backend.Models.DTOs;
using kocerbank_backend.Enums;

//güzel mi güzel bir yorum satırı

namespace kocerbank_backend.DataAccess
{
    public class SubeRepository
    {
        private readonly string _connectionString;

        // Bağlantı dizesini appsettings.json'dan almak için IConfiguration kullanıyoruz
        public SubeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleConnection") ?? throw new InvalidOperationException("Connection string bulunamadı: 'OracleConnection'");
        }

        // 1. SUBE EKLEME
        public SubeDTO Ekle(SubeDTO dto)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand kb = new OracleCommand("KB_SUBE_EKLE", conn))
                {
                    kb.CommandType = CommandType.StoredProcedure;
                    kb.BindByName = true;


                    // IN Parametreleri
                    kb.Parameters.Add("P_AD", OracleDbType.Varchar2).Value = dto.SubeAdi;
                    kb.Parameters.Add("P_TELEFONNO", OracleDbType.Varchar2).Value = dto.SubeTelefonNo;
                    kb.Parameters.Add("P_ADRES", OracleDbType.Varchar2).Value = dto.SubeAdres;
                    kb.Parameters.Add("P_DURUMKODU", OracleDbType.Byte).Value = (byte)dto.SubeDurumKodu;
                    kb.Parameters.Add("P_RECORDUSER", OracleDbType.Varchar2).Value = dto.RecordUser;
                    // OUT Parametreleri
                    OracleParameter sId = new OracleParameter("P_ID", OracleDbType.Int64) { Direction = ParameterDirection.Output };
                    OracleParameter sKodu = new OracleParameter("P_SUBEKODU", OracleDbType.Varchar2, 50) { Direction = ParameterDirection.Output };
                    OracleParameter sKayitOlusturmaTarihi = new OracleParameter("P_KAYITOLUSTURMATARIHI", OracleDbType.Date) { Direction = ParameterDirection.Output };

                    kb.Parameters.Add(sId);
                    kb.Parameters.Add(sKodu);
                    kb.Parameters.Add(sKayitOlusturmaTarihi);

                    conn.Open();
                    kb.ExecuteNonQuery();

                    // Üretilen değerleri DTO'ya geri yazıyoruz
                    dto.Id = Convert.ToInt64(sId.Value.ToString());
                    dto.SubeKodu = sKodu.Value.ToString()!;
                    dto.KayitOlusturmaTarihi = OracleZamanDamgasi.UtcOlarakOku(sKayitOlusturmaTarihi.Value);

                    return dto;
                }
            }
        }

        // 2. ID'YE GÖRE GETİR (READ)
        public SubeDTO? GetirById(long id)
        {
            SubeDTO? sube = null;

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand kb = new OracleCommand("KB_SUBE_GETIRBYID", conn))
                {
                    kb.CommandType = CommandType.StoredProcedure;
                    kb.BindByName = true;

                    kb.Parameters.Add("P_ID", OracleDbType.Int64).Value = id;
                    
                    // Oracle'daki SYS_REFCURSOR'u C# tarafında okumak için RefCursor tipi eklenir
                    kb.Parameters.Add("P_SONUC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    
                    // Cursor verisini okumak için OracleDataReader kullanıyoruz
                    using (OracleDataReader reader = kb.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            sube = MapReaderToDTO(reader);
                        }
                    }
                }
            }
            return sube;
        }

        // 3. KRİTERE GÖRE LİSTELE
        public List<SubeDTO> GetirListele(SubeAramaKriterleriDTO aramaKriterleri)
        {
            List<SubeDTO> liste = new List<SubeDTO>();

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand kb = new OracleCommand("KB_SUBE_LISTELE", conn))
                {
                    kb.CommandType = CommandType.StoredProcedure;
                    kb.BindByName = true;
                    // Arama parametrelerinde NULL olabilme ihtimaline karşı DBNull.Value kullanıyoruz
                    kb.Parameters.Add("P_ID",OracleDbType.Int64).Value = aramaKriterleri.Id.HasValue ? (object)aramaKriterleri.Id.Value : DBNull.Value;                    
                    kb.Parameters.Add("P_AD", OracleDbType.Varchar2).Value = (object?)aramaKriterleri.SubeAdi ?? DBNull.Value;
                    kb.Parameters.Add("P_SUBEKODU", OracleDbType.Varchar2).Value = (object?)aramaKriterleri.SubeKodu ?? DBNull.Value;
                    kb.Parameters.Add("P_TELEFONNO", OracleDbType.Varchar2).Value = (object?)aramaKriterleri.SubeTelefonNo ?? DBNull.Value;
                    kb.Parameters.Add("P_ADRES", OracleDbType.Varchar2).Value = (object?)aramaKriterleri.SubeAdres ?? DBNull.Value;
                    kb.Parameters.Add("P_DURUMKODU", OracleDbType.Byte).Value = aramaKriterleri.SubeDurumKodu.HasValue ? (object)(byte)aramaKriterleri.SubeDurumKodu.Value : DBNull.Value;
                    kb.Parameters.Add("P_ACILISTARIHIBASLANGIC", OracleDbType.Date).Value = aramaKriterleri.AcilisTarihiBaslangic.HasValue ? (object)aramaKriterleri.AcilisTarihiBaslangic.Value : DBNull.Value;
                    kb.Parameters.Add("P_ACILISTARIHIBITIS", OracleDbType.Date).Value = aramaKriterleri.AcilisTarihiBitis.HasValue ? (object)aramaKriterleri.AcilisTarihiBitis.Value : DBNull.Value;
                    kb.Parameters.Add("P_SONUC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();

                    using (OracleDataReader reader = kb.ExecuteReader())
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
        public void Guncelle(SubeDTO dto)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand kb = new OracleCommand("KB_SUBE_GUNCELLE", conn))
                {
                    kb.CommandType = CommandType.StoredProcedure;
                    kb.BindByName = true;

                    kb.Parameters.Add("P_ID", OracleDbType.Int64).Value = dto.Id;
                    kb.Parameters.Add("P_RECORDUSER", OracleDbType.Varchar2).Value = (object?)dto.RecordUser ?? DBNull.Value;
                    kb.Parameters.Add("P_AD", OracleDbType.Varchar2).Value = dto.SubeAdi;
                    kb.Parameters.Add("P_TELEFONNO", OracleDbType.Varchar2).Value = dto.SubeTelefonNo;
                    kb.Parameters.Add("P_ADRES", OracleDbType.Varchar2).Value = dto.SubeAdres;
                    kb.Parameters.Add("P_DURUMKODU", OracleDbType.Byte).Value = (byte)dto.SubeDurumKodu;
                    conn.Open();
                    kb.ExecuteNonQuery();
                }
            }
        }

        // 5. SİL
        public void Sil(long id)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand kb = new OracleCommand("KB_SUBE_SIL", conn))
                {
                    kb.CommandType = CommandType.StoredProcedure;
                    kb.BindByName = true;

                    kb.Parameters.Add("P_ID", OracleDbType.Int64).Value = id;

                    conn.Open();
                    kb.ExecuteNonQuery();
                }
            }
        }

        public SubeDashboardDTO GetirDashboardOzet(DateTime? baslangicTarihi, DateTime? bitisTarihi)
        {
            SubeDashboardDTO ozet = new SubeDashboardDTO();

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                using (OracleCommand KB = new OracleCommand("KB_SUBEDASHBOARD", conn))
                {
                    KB.CommandType = CommandType.StoredProcedure;
                    KB.BindByName = true;

                    KB.Parameters.Add("P_BASLANGICTARIHI", OracleDbType.Date).Value = (object?)baslangicTarihi ?? DBNull.Value;
                    KB.Parameters.Add("P_BITISTARIHI", OracleDbType.Date).Value = (object?)bitisTarihi ?? DBNull.Value;
                    KB.Parameters.Add("P_SONUC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();

                    using (OracleDataReader reader = KB.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ozet.ToplamSube = Convert.ToInt64(reader["SUBE_SAYISI"]);
                            ozet.AktifSayi  = Convert.ToInt64(reader["AKTIFSAYI"]);
                            ozet.PasifSayi  = Convert.ToInt64(reader["PASIFSAYI"]);

                            ozet.SonSubeAdi =
                                reader["SON_SUBE_ADI"] is DBNull
                                    ? null
                                    : reader["SON_SUBE_ADI"].ToString();
                        }
                    }
                }
            }

            return ozet;
        }

        // YARDIMCI METOT: Veritabanı satırını DTO nesnesine dönüştürür (Kod tekrarını önler)
        private SubeDTO MapReaderToDTO(OracleDataReader reader)
        {
            return new SubeDTO
            {
                Id = Convert.ToInt64(reader["ID"]),
                SubeAdi = reader["SUBEADI"].ToString()!,
                SubeKodu = reader["SUBEKODU"].ToString()!,
                SubeTelefonNo = reader["SUBETELEFONNO"].ToString()!,
                SubeAdres = reader["SUBEADRES"].ToString()!,
                SubeDurumKodu = (AktifPasifDurumlari)Convert.ToByte(reader["SUBEDURUMKODU"]),
                RecordUser = reader["RECORDUSER"].ToString()!,
                RecordDate = OracleZamanDamgasi.UtcOlarakOku(reader["RECORDDATE"]),
                KayitOlusturmaTarihi = OracleZamanDamgasi.UtcOlarakOku(reader["KAYITOLUSTURMATARIHI"])
            };
        }
    }
}