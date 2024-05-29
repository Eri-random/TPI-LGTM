using backend.servicios.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.servicios.Interfaces
{
    public interface ISedeService
    {
        Task createSedeAsync(List<SedeDto> sedesDtos);

        Task<IEnumerable<SedeDto>> GetAllSedesAsync();

        Task<IEnumerable<SedeDto>> GetSedesByOrganizacionIdAsync(int organizacionId);

        Task updateSedeAsync(SedeDto sedeDto);

        Task deleteSedeAsync(int sedeId);

        Task<SedeDto> GetSedeByIdAsync(int sedeId);

        double CalculateDistance(double lat1, double lon1, double lat2, double lon2);
    }
}
