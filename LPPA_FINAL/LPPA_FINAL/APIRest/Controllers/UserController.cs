using APIRest.Controllers.Helpers;
using APIRest.DAL;
using APIRest.DAL.Domains;
using APIRest.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Mail;


namespace APIRest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;

        public UserController(ILogger<UserController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            try
            {
                if (!SecurityHelper.HasAdminRole(Request))
                {
                    return BadRequest("Operacion invalida: No es Admin");
                }
                var permissions = DALFactory.GetUsersRepository(this._configuration).FindAll();
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult Post(User newUser)
        {
            try
            {
                if (!SecurityHelper.HasAdminRole(Request))
                {
                    return BadRequest("Operacion invalida: No es Admin");
                }

                if (newUser.Password.Length < 5)
                {
                    throw new Exception("La contraseña debe tener al menos 5 caracteres");
                }
                if (!SecurityHelper.verifyEmailFormat(newUser.Email))
                {
                    throw new Exception("El mail no tiene un formato valido");
                }

                string oneSalt = SecurityHelper.CreateSalt(35);
                UserDAO userDao = new UserDAO();
                userDao.Id = 0;
                userDao.Username = newUser.Username;
                userDao.Salt = oneSalt;
                userDao.PasswordHash = SecurityHelper.GenerateHash(newUser.Password, oneSalt);
                userDao.Email = newUser.Email;
                userDao.Permissions = newUser.Permissions;
                DALFactory.GetUsersRepository(this._configuration).Insert(userDao);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Authorize]
        public IActionResult Put(User oneUser)
        {
            try
            {
                if (!SecurityHelper.HasAdminRole(Request))
                {
                    return BadRequest("Operacion invalida: No es Admin");
                }

                UserDAO userDao = new UserDAO();
                userDao.Id = oneUser.Id;
                userDao.Username = null;
                userDao.Salt = null;
                userDao.PasswordHash = null;
                userDao.Email = oneUser.Email;
                userDao.Permissions = oneUser.Permissions;
                DALFactory.GetUsersRepository(this._configuration).Update(userDao);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Authorize]
        public IActionResult Delete(UserDAO oneUser)
        {
            try
            {
                if (!SecurityHelper.HasAdminRole(Request))
                {
                    return BadRequest("Operacion invalida: No es Admin");
                }
                if (SecurityHelper.GetUserIdFromHeaders(Request) == oneUser.Id)
                {
                    return BadRequest("No puede auto-eliminarse");
                }
                DALFactory.GetUsersRepository(this._configuration).Delete(oneUser);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
