using APIRest.Controllers.Helpers;
using APIRest.DAL;
using APIRest.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;


namespace APIRest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PermissionController : ControllerBase
    {
        private readonly ILogger<PermissionController> _logger;
        private readonly IConfiguration _configuration;

        public PermissionController(ILogger<PermissionController> logger, IConfiguration configuration)
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
                var permissions = DALFactory.GetPermissionsRepository(this._configuration).FindAll();
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult Post(Permission newPermission)
        {
            try
            {
                if (!SecurityHelper.HasAdminRole(Request))
                {
                    return BadRequest("Operacion invalida: No es Admin");
                }
                DALFactory.GetPermissionsRepository(this._configuration).Insert(newPermission);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Authorize]
        public IActionResult Put(Permission onePermission)
        {
            try
            {
                if (!SecurityHelper.HasAdminRole(Request))
                { 
                    return BadRequest("Operacion invalida: No es Admin"); 
                }
                DALFactory.GetPermissionsRepository(this._configuration).Update(onePermission);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Authorize]
        public IActionResult Delete(Permission onePermission)
        {
            try
            {
                if (!SecurityHelper.HasAdminRole(Request))
                {
                    return BadRequest("Operacion invalida: No es Admin");
                }
                DALFactory.GetPermissionsRepository(this._configuration).Delete(onePermission);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
