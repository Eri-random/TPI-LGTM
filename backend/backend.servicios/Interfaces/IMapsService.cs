namespace backend.servicios.Interfaces
{
    public interface IMapsService
    {
        Task<(double lat, double lng)> GetCoordinates(string address, string city, string state);
    }
}
