using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace backend.servicios.Helpers
{
    public class Token
    {
        public static string CreateJwtToken(string rol, string nombre, string cuit=null,string orgNombre=null)

        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysceret.....");
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, rol),
                new Claim(ClaimTypes.Name, nombre),
                new Claim(ClaimTypes.Email, email)
            };

            if (!string.IsNullOrEmpty(cuit) && !string.IsNullOrEmpty(orgNombre))
            {
                claims.Add(new Claim("cuit", cuit));
                claims.Add(new Claim("orgName", orgNombre));
            }

            var identity = new ClaimsIdentity(claims);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
    }
}
