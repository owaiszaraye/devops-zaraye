using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Zaraye
{
    public class JwtService
    {
        private readonly string _issuer;
        private readonly string _secret;
        private readonly string _expDate;

        public JwtService(IConfiguration config)
        {
            _issuer = config.GetSection("JwtSettings").GetSection("Issuer").Value;
            _secret = config.GetSection("JwtSettings").GetSection("Key").Value;
            _expDate = config.GetSection("JwtSettings").GetSection("ExpirationInMinutes").Value;

            if (string.IsNullOrEmpty(_secret))
                throw new Exception("JWT secret not configured");
        }

        public string GenerateSecurityToken(string email, int customerId)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("UserId", customerId.ToString()), new Claim("Email", email) }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_expDate)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public int? ValidateJwtToken(string? token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret!);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "UserId").Value);

                // return user id from JWT token if validation successful
                return userId;
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }
    }
}
