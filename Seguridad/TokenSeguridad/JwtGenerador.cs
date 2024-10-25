using Aplicacion.Contratos;
using Dominio;
using System.Security.Claims;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace Seguridad.TokenSeguridad
{
    public class JwtGenerador : IJwtGenerador
    {
        public string CrearToken(Usuario usuario)
        {
            // claims es la data del usuario que se va compartir con el cliente
            //var claims = new List<Claim>(); // Forma dinamica 
            var claims = new List<Claim> 
            { 
                // Se instalo el paquete System.IdentityModel.Tokens.Jwt
                new Claim(JwtRegisteredClaimNames.NameId, usuario.UserName)
            };

            // Crear credenciales de acceso 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("unaClaveMuyLargaParaEjemploHMACSHA512ConAlMenos512BitsDeLongitudParaQueElAlgoritmoFuncioneCorrectamente"));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Descripcion del token
            var tokenDescripcion = new SecurityTokenDescriptor 
            { 
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credenciales

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescripcion);


            return tokenHandler.WriteToken(token);
        }
    }
}
