namespace backend.api.Models
{
    public class UpdateDonationsStateRequest
    {
        public List<int> DonationIds { get; set; }
        public string State { get; set; }
    }
}
