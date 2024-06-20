namespace backend.api.Models.RequestModels
{
    public class DonationRequestModel
    {

        public int Id { get; set; }
        public string Producto { get; set; }

        public int Cantidad { get; set; }

        public string Estado { get; set; }

        public int UsuarioId { get; set; }

        public int OrganizacionId { get; set; }

        public string Cuit { get; set; }

    }
}
