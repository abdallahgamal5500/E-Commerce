using E_Commerce.Core.Helpers.Constants;
using E_Commerce.Core.Models.Database;
using E_Commerce.Core.Models.Dtos;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Helpers
{
    public static class Encryptions
    {
        public static async Task<string> EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }
        //this function Convert to Decord your Password
        public static async Task<string> DecodePasswordFromBase64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }
        public static async Task<string> GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT.GetInstance().Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim("Name", user.Name),
                new Claim("Email", user.Email),
                new Claim("Role", user.Role)
            };

            var token = new JwtSecurityToken(JWT.GetInstance().Issuer,
              JWT.GetInstance().Audience,
              claims,
              expires: DateTime.Now.AddMinutes(JWT.GetInstance().DurationInMinutes),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public static async Task<DecodedTokenDto> DecodeToken(string token)
        {
            var jwt = new JwtSecurityToken(jwtEncodedString: token);
            
            return new DecodedTokenDto
            {
                UserId = Int32.Parse(jwt.Claims.FirstOrDefault(claim => claim.Type == "UserId").Value),
                Name = jwt.Claims.FirstOrDefault(claim => claim.Type == "Name").Value,
                Email = jwt.Claims.FirstOrDefault(claim => claim.Type == "Email").Value,
                Role = jwt.Claims.FirstOrDefault(claim => claim.Type == "Role").Value,
                IsVaild = jwt.ValidTo > DateTime.UtcNow
            };
        }
    }
}
