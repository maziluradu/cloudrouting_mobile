using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace CLOUDROUTING
{
    public class TripsMap : Map
    {
        public List<Position> RouteCoordinates { get; set; }
        public List<TripsPin> TripPins { get; set; }
        
        public TripsMap()
        {
            RouteCoordinates = new List<Position>();
            //TripPins = new List<TripsPin>();
        }
    }
}
