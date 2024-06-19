namespace backend.api.Models.RequestModels
{
    public class UpdateDonationsStateRequest
    {
        public List<int> DonationIds { get; set; }
        public string State { get; set; }
    }
}
