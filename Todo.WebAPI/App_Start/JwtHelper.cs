using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Todo.WebAPI.App_Start
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateToken()
        {
            // 1. 定义需要使用到的Claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "u_admin"),
                new Claim(ClaimTypes.Role, "r_admin"),
                new Claim(JwtRegisteredClaimNames.Jti, "admin"),
                new Claim("Username", "Admin"),
                new Claim("Name", "超级管理员")
            };

            // 2. 从 appsettings.json 中读取SecretKey
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            // 3. 选择加密算法
            var algorithm = SecurityAlgorithms.HmacSha256;

            // 4. 生成Credentials
            var signingCredentials = new SigningCredentials(secretKey, algorithm);

            // 5. 根据以上，生成token
            var jwtToken = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, DateTime.Now, DateTime.Now.AddSeconds(30), signingCredentials);

            // 6. 将token变为string
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return token;
        }
    }
}
