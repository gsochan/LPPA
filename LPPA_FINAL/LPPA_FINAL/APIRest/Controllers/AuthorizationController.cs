using APIRest.Controllers.Helpers;
using APIRest.Controllers.JWT;
using APIRest.DAL;
using APIRest.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;





namespace APIRest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly ILogger<PermissionController> _logger;
        private readonly IConfiguration _configuration;

        public AuthorizationController(ILogger<PermissionController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }


        [HttpPost("LogIn")]
        public IActionResult LogIn(LoginRequest request)
        {
            try
            {
                AccountService accountService = new AccountService(_configuration);
                var result = accountService.AuthenticateSync(request);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Usuario y/o clave incorrectos");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("RefreshToken")]
        public IActionResult RefreshToken(String refreshToken)
        {
            try
            {
                AccountService accountService = new AccountService(_configuration);
                var result = accountService.RefreshTokenSync(refreshToken);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Este token no es válido o ha expirado");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("SignUp")]
        public IActionResult SignUp(RegisterRequest request)
        {
            try
            {
                AccountService accountService = new AccountService(_configuration);
                var result = accountService.RegisterSync(request);
                if (result)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Error al registrar el usuario");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
