namespace backend.data.Models
{
    public partial class Campaign
    {
        public int Id { get; set; }

        public string Title { get; set; }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set => _startDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set => _endDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public int OrganizacionId { get; set; }

        public virtual Organizacion Organizacion { get; set; }

        public virtual ICollection<Necesidad> Needs { get; set; } = [];
    }
}
