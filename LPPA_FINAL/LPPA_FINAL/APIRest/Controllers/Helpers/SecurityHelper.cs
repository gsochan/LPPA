using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;

namespace APIRest.Controllers.Helpers
{
    internal class SecurityHelper
    {
        internal static string CreateSalt(int size)
        {
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }

        internal static string GenerateHash(string input, string salt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input + salt);
            SHA256Managed sHA256ManagedString = new SHA256Managed();
            byte[] hash = sHA256ManagedString.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        internal static bool AreEqual(string plainTextInput, string hashedInput, string salt)
        {
            string newHashedPin = GenerateHash(plainTextInput, salt);
            return newHashedPin.Equals(hashedInput);
        }

        internal static String[] GetRolesFromToken(string jwtToken) {
            var fullToken = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);
            var roles = fullToken.Claims.FirstOrDefault(item => item.Type == "Roles");
            return roles.Value.Split(",");
        }
        
        internal static bool HasAdminRole(HttpRequest request) {
            var jwtToken = request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var roles = SecurityHelper.GetRolesFromToken(jwtToken);
            return roles.Contains("Admin");
        }

        internal static int GetUserIdFromToken(string jwtToken)
        {
            var fullToken = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);
            var userId = fullToken.Claims.FirstOrDefault(item => item.Type == "uid");
            return int.Parse(userId.Value);
        }
        internal static int GetUserIdFromHeaders(HttpRequest request)
        {
            var jwtToken = request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            return GetUserIdFromToken(jwtToken);
        }

        internal static bool verifyEmailFormat(string oneEmail)
        {
            try
            {
                MailAddress mailAddress = new MailAddress(oneEmail);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
