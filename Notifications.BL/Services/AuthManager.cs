using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Notifications.DAL.Models;
using Notifications.DTO.DTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.BL.Services
{
    public class AuthManager : IAuthManager
    {
        readonly UserManager<ApplicationUser> userManager;
        readonly IConfiguration Configuration;
        private ApplicationUser applicationUser;
        public AuthManager(UserManager<ApplicationUser> userManager, IConfiguration Configuration)
        {
            this.userManager = userManager;
            this.Configuration = Configuration;
        }
        public async Task<string> CreateToken()
        {
            var signingCredentials = GetSignignCredentials();
            var claims = await GetClaims();
            var token = GenerateTokenOptions(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = Configuration.GetSection("Jwt");
            var expiration = DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings.GetSection("Lifetime").Value));

            var token = new JwtSecurityToken(
                issuer: jwtSettings.GetSection("Issuer").Value,
                claims: claims,
                expires: expiration,
                signingCredentials: signingCredentials
            );

            return token;
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, applicationUser.UserName)
            };

            var roles = await userManager.GetRolesAsync(applicationUser);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private SigningCredentials GetSignignCredentials()
        {
            var key = Environment.GetEnvironmentVariable("KEY");
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        public async Task<bool> ValidateUser(LoginUserDTO userDTO)
        {
            applicationUser = await userManager.FindByNameAsync(userDTO.Email);
            return (
                applicationUser != null && 
                await userManager.CheckPasswordAsync(applicationUser, userDTO.Password)
            );
        }
    }
}
