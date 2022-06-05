using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Notifications.BL.IRepository;
using Notifications.BL.Services;
using Notifications.DAL.Models;
using Notifications.DTO.Configurations;
using Notifications.DTO.DTOs;
using Notifications.DTO.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;



namespace Notifications.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        readonly UserManager<ApplicationUser> userManager;
        readonly ILogger<AccountController> logger;
        readonly IMapper mapper;
        readonly IAuthManager authManager;
        readonly TokenValidationParameters tokenValidationParams;
        IUnitOfWork unitOfWork;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            ILogger<AccountController> logger,
            IMapper mapper,
            IAuthManager authManager,
            TokenValidationParameters tokenValidationParams, 
            IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.mapper = mapper;
            this.authManager = authManager;
            this.tokenValidationParams = tokenValidationParams;
            this.unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Route("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            logger.LogInformation($"Registration Attempt for {userDTO.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingUser = await userManager.FindByEmailAsync(userDTO.Email);

                if (existingUser != null)
                {
                    return BadRequest("Email already in use!");
                }

                var user = mapper.Map<ApplicationUser>(userDTO);
                user.UserName = userDTO.Email;
                var result = await userManager.CreateAsync(user, userDTO.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors) // TODO: possibly sensitive error info
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                var jwtToken = new AuthResult();

                if (userDTO.Email.Equals("mykola.kalinichenko@oa.edu.ua") || userDTO.Email.Equals("denys.vozniuk@oa.edu.ua") || userDTO.Email.Equals("oleksandra.kravets@oa.edu.ua"))
                {
                    await userManager.AddToRoleAsync(user, "Admin" );
                    jwtToken = await GenerateJwtToken(user);
                    return Accepted(jwtToken);
                }

                await userManager.AddToRolesAsync(user, userDTO.Roles);
                jwtToken = await GenerateJwtToken(user);
                return Accepted(jwtToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something Went Wrong in the {nameof(Register)}");
                return Problem($"Something Went Wrong in the {nameof(Register)}", statusCode: 500);
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO userDTO)
        {
            logger.LogInformation($"Login Attempt for {userDTO.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (!await authManager.ValidateUser(userDTO))
                {
                    return Unauthorized();
                }
                if (!userDTO.Email.EndsWith("oa.edu.ua"))
                {
                    return BadRequest("You must login with oa.edu.ua email");
                }

                //var props = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse", "Account") };

                //Challenge(props, GoogleDefaults.AuthenticationScheme);

                //var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                //var claims = result.Principal.Identities.FirstOrDefault()
                //    .Claims.Select(claim => new
                //    {
                //        claim.Issuer,
                //        claim.OriginalIssuer,
                //        claim.Type,
                //        claim.Value,
                //    });

                //if (claims == null)
                //{
                //    return BadRequest();
                //}

                var existingUser = await userManager.FindByEmailAsync(userDTO.Email);

                var jwtToken = await GenerateJwtToken(existingUser);

                return Accepted(jwtToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something Went Wrong in the {nameof(Login)}");
                return Problem($"Something Went Wrong in the {nameof(Login)}", statusCode: 500);
            }
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var props = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse", "Account") };

            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claims = result.Principal.Identities.FirstOrDefault()
                .Claims.Select(claim => new
                {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value,
                });

            if (claims == null)
                return BadRequest();

            var identities = result.Principal.Identities;
            string email = String.Empty;
            foreach (var id in identities)
            {
                foreach (var item in id.Claims)
                {
                    if (item.Type == ClaimTypes.Email)
                    {
                        Console.WriteLine($"Found this claim: {item.Type}");
                        email = item.Value;
                    }
                }
            }

            /*
            - подивитися, чи ця скринька співпадає з тією, яку ви визначили для адміна і чи вона у домені oa.edu.ua
            - подивитися, чи є такий користувач у вас в системі, якщо є генеруємо токени, якщо ні, створюємо для нього запис в бд і генеруємо токени
            - тоді, коли користувач буде відправляти запити, до кожного запиту в авторизаційні хедере (Bearer XXXXXX...) він буде додавати access token, коли термін дії токену закінчиться (в такому підході це зазвичай невелике число, хвилин 5) система автоматом перегенерує його (відправить refresh токен (який зберігається на юайці в локал стореджі) і отримає новий access токен)
            */

            if (email == null)
                return BadRequest();

            logger.LogInformation($"Google login Attempt for {email}");
            
            if (!email.EndsWith("@oa.edu.ua"))
                return BadRequest("Log in with OA domain!");

            //if (email.Equals("mykola.kalinichenko@oa.edu.ua") || 
            //    email.Equals("denys.vozniuk@oa.edu.ua") ||
            //    email.Equals("oleksandra.kravets@oa.edu.ua"))
            //{
            //}

            try
            {
                var existingUser = await userManager.FindByEmailAsync(email);
            
                if (existingUser == null)
                {
                    UserDTO userDTO = new UserDTO
                    {
                        Email = email,
                    };

                    var user = mapper.Map<ApplicationUser>(userDTO);
                    user.UserName = userDTO.Email;
                    var userResult = await userManager.CreateAsync(user);

                    if (!result.Succeeded)
                    {
                        foreach (var error in userResult.Errors) // TODO: possibly sensitive error info
                            ModelState.AddModelError(error.Code, error.Description);
                        
                        return BadRequest(ModelState);
                    }
                }

                existingUser = await userManager.FindByEmailAsync(email);

                var jwtToken = await GenerateJwtToken(existingUser);

                return Accepted(jwtToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something Went Wrong in the {nameof(GoogleResponse)}");
                return Problem($"Something Went Wrong in the {nameof(GoogleResponse)}", statusCode: 500);
            }
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            if(ModelState.IsValid)
            {
                var result = await VerifyAndGenerateToken(tokenRequest);

                if (result == null)
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        Errors = new List<string>()
                        {
                            "Invalid tokens"
                        },
                        Success = false
                    });
                }

                return Ok(result);
            }

            return BadRequest("Invalid payload");
        }

        private async Task<AuthResult> GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("KEY"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddSeconds(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                IsRevoked = false,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Token = RandomString(35) + Guid.NewGuid()
            };

            await unitOfWork.RefreshTokens.Insert(refreshToken);
            await unitOfWork.Save();

            return new AuthResult
            {
                Token = jwtToken,
                Success = true,
                RefreshToken = refreshToken.Token
            };
        }

        private async Task<AuthResult> VerifyAndGenerateToken(TokenRequest tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenValidationParams.ValidateLifetime = false; // TODO: Remove from prod
                // Validation 1 - Validation of JWT token format
                var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, tokenValidationParams, out var validatedToken);
                tokenValidationParams.ValidateLifetime = true;

                // Validation 2 - Validate encryption alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)
                    {
                        return null;
                    }
                }

                // Validation 3 - Validate expiry date
                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

                if (expiryDate > DateTime.UtcNow)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Token has not yet expired"
                        }
                    };
                }

                // Validation 4 - Validate existence of the token
                var storedToken = await unitOfWork.RefreshTokens.GetFirstOrDefault(x => x.Token == tokenRequest.RefreshToken);

                if (storedToken == null)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Token does not exist"
                        }
                    };
                }

                // Validation 5 - Validate of token is used
                if (storedToken.IsUsed)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Token has been used"
                        }
                    };
                }

                // Validation 6 - Validate if revoked
                if (storedToken.IsRevoked)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Token has been revoked"
                        }
                    };
                }

                // Validation 7 - Validate the ID
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                if (storedToken.JwtId != jti)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Token doesn't match"
                        }
                    };
                }

                // Validation 8 - Validate stored token expiry date
                if (storedToken.ExpiryDate < DateTime.UtcNow)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Refresh token has expired"
                        }
                    };
                }

                // Update current token
                storedToken.IsUsed = true;
                unitOfWork.RefreshTokens.Update(storedToken);
                await unitOfWork.Save();

                // Generate a new token
                var dbUser = await userManager.FindByIdAsync(storedToken.UserId);
                return await GenerateJwtToken(dbUser);
            }
            catch (Exception ex)
            {
                logger.LogInformation(ex, $"Something went wrong in the {nameof(VerifyAndGenerateToken)}");
                return null;
            }
        }

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();

            return dateTimeVal;
        }

        private string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(x => x[random.Next(x.Length)]).ToArray());
        }
    }
}
