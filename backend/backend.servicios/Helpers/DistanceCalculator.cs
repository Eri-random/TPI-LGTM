namespace backend.servicios.Helpers
{
    public static class DistanceCalculator
    {
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371e3; // Radio de la tierra en metros
            var φ1 = lat1 * Math.PI / 180; // φ, λ en radianes
            var φ2 = lat2 * Math.PI / 180;
            var Δφ = (lat2 - lat1) * Math.PI / 180;
            var Δλ = (lon2 - lon1) * Math.PI / 180;

            var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
                    Math.Cos(φ1) * Math.Cos(φ2) *
                    Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            var d = R * c; // En metros
            return d;
        }
    }
}
