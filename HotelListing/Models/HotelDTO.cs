using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Models
{
    public class CreateHotelDTO
    {
        [Required]
        [StringLength(maximumLength: 150, ErrorMessage = "Hotel Name is too long")]
        public string Name { get; set; }
        [Required]
        [StringLength(maximumLength: 250, ErrorMessage = "Address is Too Long")]
        public string Address { get; set; }
        [Required]
        [Range(1, 5)]
        public double Rating { get; set; }

        // for referencing the related entity
        //[Required]  ===>> removing the "required" constraint here [2] COnstruct PUT Endpoint  @23:30
        public int CountryId { get; set; }
    }

    public class HotelDTO : CreateHotelDTO
    {
        public int Id { get; set; }
        public CountryDTO Country { get; set; }
    }

    public class UpdateHotelDTO : CreateHotelDTO
    {

    }

    
}
