using HotelListing.Data;
using HotelListing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HotelListing.Services
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _configuration;
        private ApiUser _user;

        public AuthManager(UserManager<ApiUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }


        public async Task<string> CreateToken()
        {
            // get signing credentials
            var signingCredentials = GetSigningCredentials();
            // get claims
            var claims = await GetClaims();
            // get TokenOptions
            var token = GenerateTokenOptions(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var expiration = DateTime.Now.AddHours(Convert.ToDouble(jwtSettings.GetSection("lifetime").Value));

            //var issuerFromAppSettings_json = jwtSettings.GetSection("validIssuer").Value;
            var issuerFromAppSettings_json = jwtSettings.GetSection("Issuer").Value;

            var token = new JwtSecurityToken(
                // issuer should be coming from the JWT Settings section in appsettings.json or env variables
                issuer: issuerFromAppSettings_json,
                claims: claims,
                expires: expiration,
                signingCredentials: signingCredentials
               );

            return token;
        }


        // Claims tells the App what the user or the client can access
        // is like claiming I can do this or can do that
        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                // in case the client will not use the Email as login Username 
                new Claim(ClaimTypes.Name, _user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(_user);

            // for each roles we are going to add a corresponding claims for this user 
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
            

        private SigningCredentials GetSigningCredentials()
        {
            var key = Environment.GetEnvironmentVariable("KEY",EnvironmentVariableTarget.Machine);
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }


        public async Task<bool> ValidateUser(LoginUserDTO userDTO)
        {
            // does the user exist in the system and password valid
            _user = await _userManager.FindByNameAsync(userDTO.Email);
            var validPassword = await _userManager.CheckPasswordAsync(_user, userDTO.Password);
            return (_user != null && validPassword);
        }
    }
}
