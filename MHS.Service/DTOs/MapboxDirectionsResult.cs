using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHS.Service.DTOs
{
    public class MapboxDirectionsResult
    {
        public double Distance { get; set; } 
        public double Duration { get; set; } 
        public string? Geometry { get; set; } 
        public string? Summary { get; set; } 
    }
}
