using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingWebApp.Contract;
using DatingWebApp.Models;
using DatingWebApp.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository authRepository;
        private readonly IConfiguration configuration;

        public AuthController(IAuthRepository authRepository,IConfiguration configuration )
        {
            this.authRepository = authRepository;
            this.configuration = configuration;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]UserViewModel userViewModel)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userName = userViewModel.UserName.ToLower();

            if (!await authRepository.UserExists(userName))
            {
                var user = new User
                {
                    UserName = userName
                };

                var registeredUser=await authRepository.Register(user, userViewModel.Password);

                return StatusCode(201);

            }
            else
            {
                return BadRequest("User Already exists");
            }
        }


        [HttpPost("Login")]

        public async Task<IActionResult> Login([FromBody]UserViewModel userViewModel)
        {
            try
            {


                if (!ModelState.IsValid)
                    return BadRequest(ModelState);


                var userName = userViewModel.UserName.ToLower();

                if (await authRepository.UserExists(userName))
                {

                    var user = await authRepository.Login(userName, userViewModel.Password);

                    //Create JWT Toen

                    if (user != null)
                    {
                        var claims = new Claim[]
                        {
                        new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                        new Claim(ClaimTypes.Name,user.UserName)
                        };

                        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value));

                        var cred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

                        var securityTokenDescriptor = new SecurityTokenDescriptor()
                        {
                            Subject = new ClaimsIdentity(claims),
                            Expires = DateTime.Now.AddMinutes(1),
                            SigningCredentials = cred

                        };
                        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

                        SecurityToken tokenJWT = tokenHandler.CreateToken(securityTokenDescriptor);

                        return Ok(new { token = tokenHandler.WriteToken(tokenJWT) });

                    }
                    return Unauthorized();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch(Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}