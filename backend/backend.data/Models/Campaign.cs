namespace backend.data.Models
{
    public partial class Campaign
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int OrganizacionId { get; set; }

        public virtual Organizacion Organizacion { get; set; }

        public virtual ICollection<Necesidad> Needs { get; set; } = [];
    }
}
