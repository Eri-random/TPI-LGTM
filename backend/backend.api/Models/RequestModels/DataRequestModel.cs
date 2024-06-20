using backend.data.Models;

namespace backend.api.Models.RequestModels
{
    public class DataRequestModel
    {
        public Organizacion Organizacion { get; set; }
        public List<Sede> Sedes { get; set; }
        public Usuario Usuario { get; set; }
    }
}
