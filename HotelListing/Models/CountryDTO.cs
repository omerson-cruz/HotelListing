using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Models
{
    /*DTO is like the Model in MVC pattern except the term is used generally for WEB API*/
    /*The data here is looking like the Data Models for our Database Tables or Entities*/

    /*Here is where we implement some validation before reaching the actual objects or models for the Entity Models*/

    
    public class CreateCountryDTO
    {
        [Required]
        [StringLength(maximumLength: 50, ErrorMessage = "Country Name is Too Long")]
        public string Name { get; set; }
        [Required]
        [StringLength(maximumLength: 2, ErrorMessage = "Short Country Name is Too Long")]
        public string ShortName { get; set; }
    }

    public class CountryDTO : CreateCountryDTO
    {
        public int Id { get; set; }
        // for HAS MANY relationship with Hotels
        public IList<HotelDTO> Hotels { get; set; }

    }

    public class UpdateCountryDTO : CreateCountryDTO
    {
        // [2] Construct PUT Endpoint - explanation as to why were putting  public virtual IList<HotelDTO> Hotels { get; set; } 
        // in both CountryDTO and updateDTO instead of just inheriting from CreateCountryDTO


        // this allows the Update for the country to also Add/edit/remove Associated Hotels in the Country 
        public IList<CreateHotelDTO> Hotels { get; set; }

        // [2] Construct PUT Endpoint - @20:18. changing from HotelDTO to "CreateHotelDTO"

    }
}
