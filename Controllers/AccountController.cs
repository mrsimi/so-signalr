using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VueSPATemplate
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login ([FromBody]LoginCredentials creds)
        {
            if(!ValidateLogin(creds))
            {
                return new JsonResult(new {error = "Login failed"});
            }
             
            var principal = GetPrincipal(creds, Startup.CookieAuthScheme);
            await HttpContext.SignInAsync(Startup.CookieAuthScheme, principal);

            return new  JsonResult(new {
                name = principal.Identity.Name,
                email = principal.FindFirstValue(ClaimTypes.Email),
                role = principal.FindFirstValue(ClaimTypes.Role)
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return StatusCode(200);
        }

        private ClaimsPrincipal GetPrincipal(LoginCredentials creds, string cookieAuthScheme)
        {
            var claims = new List<Claim>{
                new Claim(ClaimTypes.Name, creds.Email),
                new Claim(ClaimTypes.Email, creds.Email),
                new Claim(ClaimTypes.Role, "User"),
            };

            return new ClaimsPrincipal(new ClaimsIdentity(claims, cookieAuthScheme));
        }

        private bool ValidateLogin(LoginCredentials creds)
        {
            return true;
        }
    }


    public class LoginCredentials
    {
        public string Email {get; set;}
        public string Password {get; set;}
    }
}