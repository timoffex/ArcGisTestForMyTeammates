using System;
namespace MapServerTesting
{
    public readonly struct GeoCoordinates
    {
        public readonly double latitude;
        public readonly double longitude;

        public GeoCoordinates(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }
    }
}
