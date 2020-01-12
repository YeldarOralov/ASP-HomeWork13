using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPHW13.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASPHW13.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class HomeController : ControllerBase
    {
        private readonly AuthService authService;
        public HomeController(AuthService authService)
        {
            this.authService = authService;
        }

        [HttpGet]
        public IActionResult GetUserToken([FromHeader]string token)
        {
            var userToken = authService.DecryptToken(token);

            return Ok(new { userToken });
        }
    }
}