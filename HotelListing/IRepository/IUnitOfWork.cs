using HotelListing.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.IRepository
{
    public interface IUnitOfWork: IDisposable
    {
       /* this will act like a register for every variation of the generic repository relative to the class T*/

        IGenericRepository<Country> Countries { get; } 
        IGenericRepository<Hotel> Hotels { get; }
        Task Save();
    }
}
