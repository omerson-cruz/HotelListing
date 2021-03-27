using AutoMapper;
using HotelListing.Data;
using HotelListing.Models;
using HotelListing.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApiUser> _userManager;

        // Removing the SIGNIN MANAGER because will be using JSON WEBTOKENS instead of SIGNIN Manager that manages User Session and authentication
        //private readonly SignInManager<ApiUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper _mapper;
        private readonly IAuthManager _authManager;


        public AccountController(UserManager<ApiUser> userManager,
            //SignInManager<ApiUser> signInManager,
            ILogger<AccountController> logger,
            IMapper mapper,
            IAuthManager authManager
            )
        {
            _userManager = userManager;
            //_signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
            _authManager = authManager;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO) // The form's body content should match the UserDTO that we defined
        {
           
            _logger.LogInformation($"Registration Attempt for {userDTO.Email}");
            if(!ModelState.IsValid)
            {
                // if body form does not match the userDTO => ApiUser model
                return BadRequest(ModelState);
            }

            try
            {
                var user = _mapper.Map<ApiUser>(userDTO);

                // we're adding "UserName" and Mapping userDTO.Email into it because "ApiUser" 
                //      only contains FirstName and LastName (including the inherited properties from 
                user.UserName = userDTO.Email;

                // this is the actual registration for the user 
                var result = await _userManager.CreateAsync(user, userDTO.Password);

                if(!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }

                    return BadRequest(ModelState);
                }
                // After Adding user Roles in the RoleConfiguration.cs
                await _userManager.AddToRolesAsync(user, userDTO.Roles);
                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(Register)}");
                // this is another way of return a status code message/ status to the client 
                return Problem($"Something went wrong in the {nameof(Register)}", statusCode: 500);
            }
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO userDTO) // The form's body content should match the UserDTO that we defined
        {

            _logger.LogInformation($"Login Attempt for {userDTO.Email}");

            // if body form does not match the userDTO => ApiUser model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if(!await _authManager.ValidateUser(userDTO))
                {
                    return Unauthorized();
                }

                var tokenTest = await _authManager.CreateToken();

                //return Accepted(new { Token = await _authManager.CreateToken() });
                return Accepted(new { Token = tokenTest });
            }
            catch (Exception ex)
            {
                Console.WriteLine("testing: " + ex.StackTrace + ex.Message);
                _logger.LogError(ex, $"Something went wrong in the {nameof(Login)}");
                // this is another way of return a status code message/ status to the client 
                return Problem($"Something went wrong in the {nameof(Login)}", statusCode: 500);
            }
        }

    }
}
