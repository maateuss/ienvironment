using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iEnvironment.RestAPI.Services
{
    public class CryptoService
    {
        private static string GenerateSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt(Settings.WorkFactor);
        }
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, GenerateSalt());
        }

        public static bool ValidatePassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
