using AutoMapper;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Controllers
{
    [ApiVersion("2.0")]     // indicates that this is another version of the original country controller 
    //[ApiVersion("2.0", Deprecated = true)]     // indicates that this is another version of the original country controller 

    //[Route("api/country")]  // ===> this will mark as the same endpoint as the original country controller
    [Route("api/{v:apiversion}/country")]  // ===> this will mark as the same endpoint as the original country controller
    [ApiController]
    public class CountryV2Controller : ControllerBase
    {
        // for dvmo purposes only but it is not a good practice to let the controller get direct access to the Database
        private DatabaseContext _context; 

        public CountryV2Controller(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountries([FromQuery] RequestParams requestParams)
        {
            return Ok(_context.Countries);
        }
    }

}
