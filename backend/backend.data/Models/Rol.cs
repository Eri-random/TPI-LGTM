namespace backend.data.Models
{
    public class Rol
    {
        public int RolId { get; set; }
        public string Nombre { get; set; }
        public List<Usuario> Usuarios { get; set; }
    }
}
