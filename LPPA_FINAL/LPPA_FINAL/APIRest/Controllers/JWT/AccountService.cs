using APIRest.Controllers.Helpers;
using APIRest.DAL;
using APIRest.DAL.Domains;
using APIRest.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace APIRest.Controllers.JWT
{
    public class AccountService
    {
        private readonly IConfiguration _configuration;
        private readonly JwtSettings _jwtSettings;
        public AccountService(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._jwtSettings = new JwtSettings();
            _configuration.Bind(nameof(JwtSettings), _jwtSettings);
        }

        public AuthenticationResponse AuthenticateSync(LoginRequest request)
        {
            var users = DALFactory.GetUsersRepository(this._configuration).FindAll();
            var currentUser = users.FirstOrDefault(item => item.Username == request.UserName && SecurityHelper.AreEqual(request.Password, item.PasswordHash, item.Salt));
            if (currentUser == null) { 
                return null;
            }

            JwtSecurityToken jwtSecurityToken = GenerateJWTToken(currentUser);
            RefreshToken refreshToken = GenerateRefreshToken();
            var oneToken = new TokenDAO
            {
                UserId = currentUser.Id,
                Token = refreshToken.Token,
                Expires = refreshToken.Expires
            };
            DALFactory.GetRefreshTokenRepository(this._configuration).Insert(oneToken);

            return new AuthenticationResponse()
            {
                UserId = currentUser.Id,
                UserName = currentUser.Username,
                JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                RefreshToken = refreshToken.Token
            };
        }

        public AuthenticationResponse RefreshTokenSync(String refreshToken)
        {
            var tokens = DALFactory.GetRefreshTokenRepository(this._configuration).FindAll();
            var currentToken = tokens.FirstOrDefault(item => item.Token == refreshToken && item.Expires > DateTime.Now);
            if (currentToken == null)
            {
                return null;
            }

            var users = DALFactory.GetUsersRepository(this._configuration).FindAll();
            var currentUser = users.FirstOrDefault(item => item.Id == currentToken.UserId);
            if (currentUser == null)
            {
                return null;
            }
            JwtSecurityToken jwtSecurityToken = GenerateJWTToken(currentUser);
            return new AuthenticationResponse()
            {
                UserId = currentUser.Id,
                UserName = currentUser.Username,
                JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                RefreshToken = refreshToken
            };
        }

        public bool RegisterSync(RegisterRequest request)
        {
            if (request.Password != request.ConfirmPassword)
            {
                throw new Exception("Ambas contraseñas deben ser iguales");
            }
            if (request.Password.Length < 5)
            {
                throw new Exception("La contraseña debe tener al menos 5 caracteres");
            }

            if (!SecurityHelper.verifyEmailFormat(request.Email))
            {
                throw new Exception("El mail no tiene un formato valido");
            }

            string oneSalt = SecurityHelper.CreateSalt(35);
            UserDAO userDao = new UserDAO();
            userDao.Id = 0;
            userDao.Username = request.UserName;
            userDao.Salt = oneSalt;
            userDao.PasswordHash = SecurityHelper.GenerateHash(request.Password, oneSalt);
            userDao.Email = request.Email;
            userDao.Permissions = new List<Permission>();
            DALFactory.GetUsersRepository(this._configuration).Insert(userDao);
            return true;
        }



        private JwtSecurityToken GenerateJWTToken(UserDAO user)
        {
            var roles = String.Join(", ", user.Permissions.Select(o => o.Name));

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("Roles", roles),
                new Claim("uid", user.Id.ToString())
            };

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.AccessTokenSecret));
            var signinCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                signingCredentials: signinCredentials);
            return jwtSecurityToken;
        }

        private JwtSecurityToken GenerateJWTToken(String refreshToken)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, ""),
                new Claim("uid", "")
            };

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.AccessTokenSecret));
            var signinCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                signingCredentials: signinCredentials);
            return jwtSecurityToken;
        }

        private RefreshToken GenerateRefreshToken()
        {
            return new RefreshToken()
            {
                Token = RandomTokenString(),
                Expires = DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                Created = DateTime.Now
            };
        }

        private string RandomTokenString()
        {
            var rngCryptoServicesProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[25];
            rngCryptoServicesProvider.GetBytes(randomBytes);
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

    }


    class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public bool IsExpired => DateTime.Now >= Expires;
        public DateTime? Revoked { get; set; }
        public string RevokedByIp { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;
    }
}
