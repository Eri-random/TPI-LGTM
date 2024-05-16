using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using backend.servicios.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;


namespace backend.servicios.Servicios
{
    public class MapsService: IMapsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _googleMapsApiKey;

        public MapsService()
        {
        }

        public MapsService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _googleMapsApiKey = configuration["GoogleMapsApiKey"];
        }

        public async Task<(double lat, double lng)> GetCoordinates(string address, string city, string state)
        {
            var encodedAddress = Uri.EscapeDataString(address);
            var encodedCity = Uri.EscapeDataString(city);
            var encodedState = Uri.EscapeDataString(state);

            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={encodedAddress}+{encodedCity}+{encodedState}&key={_googleMapsApiKey}";

            var response = await _httpClient.GetAsync(url);
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
