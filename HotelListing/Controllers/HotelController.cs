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
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HotelController> _logger;
        private readonly IMapper _imapper;

        public HotelController(IUnitOfWork unitOfWork, ILogger<HotelController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _imapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotels()
        {
            try
            {
                var hotels = await _unitOfWork.Hotels.GetAll();
                var results = _imapper.Map<List<HotelDTO>>(hotels);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(GetHotels)}");
                return StatusCode(500, "Internal Server Error. Please try again later." + ex.StackTrace + "\n" + ex.Message);
            }
        }

        [HttpGet("{id:int}", Name = "GetHotel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotel(int id)
        {
            try
            {
                var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id, new List<string> { "Country" });
                var results = _imapper.Map<HotelDTO>(hotel);
              
                return Ok(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: \n"+ ex.Message + ex.StackTrace);
                _logger.LogError(ex, $"Something went wrong in the {nameof(GetHotel)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }
        // enforce roles based authorization
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDTO hotelDTO)
        {
            // Validate incoming JSON Form Data for Creating Hotel 
            // ModelState IsValid checks if all the required data for CreateHotelDTO is in the JSON request 
            if(!ModelState.IsValid)
            {
                _logger.LogError($"Invalid POST attempt in {nameof(CreateHotel)}");
                return BadRequest(ModelState);
            }

            try
            {
                var hotel =_imapper.Map<Hotel>(hotelDTO);
                await _unitOfWork.Hotels.Insert(hotel);

                // FOR POST or any API Methods that changes the database we need to call Save or commit the changes to the database.
                await _unitOfWork.Save();

                // note that after Save() the "hotel" which is of type DTO will have
                //      the property ".id" which we can then send back to the client 
                return CreatedAtRoute("GetHotel", new { id = hotel.Id }, hotelDTO);

                // With "GetHote" in the CreatedAtRoute function we need to put a Name on the GET => GetHotel Data Annotation
                // As you see above we declared and added this Name as a nickname or identifier for the other Route here in HotelController
                // [HttpGet("{id:int}", Name = "GetHotel")]

            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(CreateHotel)}");
                return StatusCode(500, "Internal server Error. Please try again Later.");
            }

        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        
        // int id will be passed from the FOrm JSON body of the client to the CreateHotel DTO
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] CreateHotelDTO hotelDTO) 
        {
            if(!ModelState.IsValid || id < 1 )
            {
                _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateHotel)}");
                return BadRequest(ModelState);
            }

            try
            {
                var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id);
                if(hotel == null)
                {
                    _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateHotel)}");
                    return BadRequest("Submitted data is invalid");
                }

                // if not null then proceed with the update
                // so whatever is in hotelDTO put this into the "hotel" variable
                _imapper.Map(hotelDTO, hotel);
                _unitOfWork.Hotels.Update(hotel);
                await _unitOfWork.Save();

                return NoContent();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(UpdateHotel)}");
                return StatusCode(500, "Internal server Error. Please try again Later.");
            }
        }


        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            if (id < 1)
            {
                _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteHotel)}");
                return BadRequest();
            }

            try
            {
                var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id);
                if (hotel == null)
                {
                    _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteHotel)}");
                    return BadRequest("Submitted data is invalid");
                }

                await _unitOfWork.Hotels.Delete(id);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(DeleteHotel)}");
                return StatusCode(500, "Internal server Error. Please try again Later.");
            }
        }

    }
}
