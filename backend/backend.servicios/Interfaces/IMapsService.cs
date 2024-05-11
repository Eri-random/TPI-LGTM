using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.servicios.Interfaces
{
    public interface IMapsService
    {
        Task<(double lat, double lng)> GetCoordinates(string address, string city, string state);
    }
}
