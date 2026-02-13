using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TeamProjectServer.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config) => _config = config;

        public string CreateToken(int id, string email, string name)
        {
            var jwtSetting = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Name, name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // 새로운토큰갱신
            };

            var token = new JwtSecurityToken
                (
                issuer: jwtSetting["Lssuer"],
                audience: jwtSetting["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7), // 토큰 유효기간 로그인 다시해야됨
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
