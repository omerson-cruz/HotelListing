using HotelListing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Configurations.Entities
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(
                    new Hotel
                    {
                        Id = 1,
                        Name = "Resorts World Manila",
                        Address = "Manila",
                        CountryId = 1,
                        Rating = 5
                    },
                    new Hotel
                    {
                        Id = 2,
                        Name = "Okada",
                        Address = "Manila",
                        CountryId = 1,
                        Rating = 4

                    },
                    new Hotel
                    {
                        Id = 3,
                        Name = "Sogo Hotel",
                        Address = "China",
                        CountryId = 3,
                        Rating = 3

                    },
                    new Hotel
                    {
                        Id = 4,
                        Name = "Shangrila",
                        Address = "US",
                        CountryId = 2,
                        Rating = 2

                    }
                );
        }
    }
}
