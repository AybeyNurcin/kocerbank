using System;

namespace kocerbank_backend.DataAccess
{
    /*
     * Oracle sunucusunun saat dilimi UTC olduğu için
     * SYSDATE/SYSTIMESTAMP ile üretilen zaman damgaları
     * okunurken UTC olarak işaretlenir.
     *
     * Böylece JSON'a UTC olarak yazılır (ISO 8601 sonunda
     * 'Z' ile) ve frontend bunu tarayıcının yerel saatine
     * göre otomatik dönüştürür. İşaretlenmezse değer
     * tarayıcıda zaten yerel saatmiş gibi yorumlanır ve
     * Türkiye'de 3 saat geride görünür.
     *
     * Kullanıcının girdiği saf tarihler (ör. doğum tarihi)
     * burada KULLANILMAZ; yalnızca sunucu tarafından
     * SYSDATE/SYSTIMESTAMP ile üretilen zaman damgaları
     * için kullanılır.
     */
    internal static class OracleZamanDamgasi
    {
        public static DateTime UtcOlarakOku(object deger)
        {
            return DateTime.SpecifyKind(
                Convert.ToDateTime(deger),
                DateTimeKind.Utc
            );
        }
    }
}
