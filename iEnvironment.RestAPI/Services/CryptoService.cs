using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using iEnvironment.Domain.Models;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace iEnvironment.RestAPI.Services
{
    public class CryptoService : BaseService<RefreshToken>
    {
        public CryptoService() : base("tokens")
        {

        }
        private string GenerateSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt(Settings.WorkFactor);
        }
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, GenerateSalt());
        }

        public bool ValidatePassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }


        public string GenerateJWT(User User)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
               {
                    new Claim(ClaimTypes.Name, User.Name.ToString()),
                    new Claim(ClaimTypes.Role, User.GetClaimAttribute())
               }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<RefreshToken> GenerateRefreshToken(User user)
        {
            var token = new RefreshToken(TimeSpan.FromHours(Settings.RefreshTokerValidationHours), user.Id);

            await Collection.InsertOneAsync(token);
            return token;
        }

        public async Task<RefreshToken> RetrieveRefreshToken(string token)
        {
            return await Collection.Find(x => x.Value == token).FirstAsync();
        }

        public async Task<bool> RevokeTokens(string userid)
        {
            var tokens = await Collection.Find(x => x.UserID == userid).ToListAsync();

            foreach (var item in tokens)
            {
                await Collection.DeleteOneAsync(x => x.Id == item.Id);
            }

            return true;
        }
    }
}
