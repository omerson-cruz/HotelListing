using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;

namespace HotelListing.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) :  base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Hotel> Hotels { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Country>().HasData(
                    new Country 
                    { 
                        Id = 1,
                        Name = "Philippines",
                        ShortName = "PH"
                    },
                    new Country
                    {
                        Id = 2,
                        Name = "America",
                        ShortName = "US"
                    },
                    new Country
                    {
                        Id = 3, 
                        Name = "China",
                        ShortName = "CH"
                    }
                ); 
            
                builder.Entity<Hotel>().HasData(
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
