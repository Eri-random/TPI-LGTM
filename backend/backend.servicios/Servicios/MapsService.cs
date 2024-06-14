using backend.servicios.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace backend.servicios.Servicios
{
    public class MapsService(IHttpClientFactory factory, IConfiguration configuration) : IMapsService
    {
        private readonly IHttpClientFactory _httpClientFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        private readonly string _googleMapsApiKey = configuration["GoogleMapsApiKey"];

        public async Task<(double lat, double lng)> GetCoordinates(string address, string city, string state)
        {
            var encodedAddress = Uri.EscapeDataString(address);
            var encodedCity = Uri.EscapeDataString(city);
            var encodedState = Uri.EscapeDataString(state);

            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={encodedAddress}+{encodedCity}+{encodedState}&key={_googleMapsApiKey}";

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);

            if (result.status == "OK")
            {
                double lat = result.results[0].geometry.location.lat;
                double lng = result.results[0].geometry.location.lng;
                return (lat, lng);
            }
            else
            {
                // Manejar errores de geocodificación
                throw new Exception("Error geocodificando la dirección");
            }
        }
    }
}
