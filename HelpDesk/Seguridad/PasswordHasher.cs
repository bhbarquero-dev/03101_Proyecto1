using System;
using System.Security.Cryptography;

namespace HelpDesk.Seguridad
{
    // Hash de contrasenas con PBKDF2-HMAC-SHA256 (OWASP Password Storage Cheat Sheet).
    // Formato almacenado: PBKDF2-SHA256$iteraciones$sal$hash (sal y hash en Base64).
    // Las iteraciones se guardan en cada valor, asi que se pueden aumentar sin invalidar los existentes.
    public static class PasswordHasher
    {
        private const string Algoritmo = "PBKDF2-SHA256";
        public const int Iteraciones = 600000;
        private const int BytesSal = 16;
        private const int BytesHash = 32;

        public static string Generar(string contrasena)
        {
            if (contrasena == null) throw new ArgumentNullException("contrasena");

            byte[] sal = new byte[BytesSal];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(sal);
            }
            byte[] hash = Derivar(contrasena, sal, Iteraciones, BytesHash);

            return string.Join("$", Algoritmo, Iteraciones, Convert.ToBase64String(sal), Convert.ToBase64String(hash));
        }

        public static bool Verificar(string contrasena, string valorAlmacenado)
        {
            if (contrasena == null || string.IsNullOrEmpty(valorAlmacenado)) return false;

            string[] partes = valorAlmacenado.Split('$');
            if (partes.Length != 4 || partes[0] != Algoritmo) return false;

            int iteraciones;
            if (!int.TryParse(partes[1], out iteraciones) || iteraciones <= 0) return false;

            byte[] sal, esperado;
            try
            {
                sal = Convert.FromBase64String(partes[2]);
                esperado = Convert.FromBase64String(partes[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            byte[] calculado = Derivar(contrasena, sal, iteraciones, esperado.Length);
            return IgualesEnTiempoConstante(calculado, esperado);
        }

        private static byte[] Derivar(string contrasena, byte[] sal, int iteraciones, int bytes)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(contrasena, sal, iteraciones, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(bytes);
            }
        }

        // Compara todos los bytes aunque haya diferencias, para no revelar informacion por el tiempo de respuesta
        private static bool IgualesEnTiempoConstante(byte[] a, byte[] b)
        {
            int diferencia = a.Length ^ b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diferencia |= a[i] ^ b[i];
            }
            return diferencia == 0;
        }
    }
}
