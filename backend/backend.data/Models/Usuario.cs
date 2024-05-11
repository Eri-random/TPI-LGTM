using System.ComponentModel.DataAnnotations.Schema;

namespace backend.data.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Contrasena { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        [Column("rol_id")]
        public int RolId { get; set; }
        public Rol Rol { get; set; }
        public string Cuit {  get; set; }
    }
}
