using System;
using System.Collections.Generic;
namespace MapServerTesting.ArcGIS.OriginDestination
{
    public class OriginDestinationResult
    {
        public Value value { get; set; }

        public class Value
        {
            public IList<ResultFeature> features { get; set; }
        }
    }

    public class ResultFeature
    {
        public Attributes attributes { get; set; }

        public class Attributes
        {
            public int OBJECTID { get; set; }
            public int DestinationRank { get; set; }
            public double Total_Time { get; set; }
            public double Total_Distance { get; set; }
            public string OriginName { get; set; }
            public int OriginOID { get; set; }
            public string DestinationName { get; set; }
            public int DestinationOID { get; set; }
        }
    }
}
