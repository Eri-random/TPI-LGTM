using backend.servicios.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.servicios.Interfaces
{
    public interface IDonacionService
    {
        Task<IEnumerable<DonacionDto>> GetDonacionesByUsuarioIdAsync(int usuarioId);

        Task<IEnumerable<DonacionDto>> GetDonacionesByOrganizacionIdAsync(int organizacionId);

    }
}
