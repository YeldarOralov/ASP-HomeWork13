using ASPHW13.DataAccess;
using ASPHW13.Models;
using ASPHW13.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ASPHW13.Services
{
    public class AuthService
    {
        private readonly UserContext context;
        private readonly string jwtSecret;

        public AuthService(UserContext context, IOptions<SecretOptions> options)
        {
            this.context = context;
            this.jwtSecret = options.Value.JWTSecret;
        }

        public async Task<string> Authenticate(string login, string password)
        {
            var user = await context.Users.FirstOrDefaultAsync(user => user.Login == login && user.Password == password);

            if (user is null) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, login)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }

        public async Task<string> Registrate(string username, string password)
        {
            var user = await context.Users.FirstOrDefaultAsync(user => user.Login == username && user.Password == password);

            if (user != null) return null;

            context.Users.Add(new User { Login = username, Password = password });

            await context.SaveChangesAsync();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }

        public string DecryptToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = handler.ReadToken(token) as JwtSecurityToken;

            var claims = tokenS.Claims;

            return (claims as List<Claim>)[0].Value;
        }
    }
}
