using MHS.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MobileHomeServiceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MapBoxController : ControllerBase
    {
        private readonly IMapboxService _mapboxService;
        public MapBoxController(IMapboxService mapboxService)
        {
            _mapboxService = mapboxService;
        }

        [HttpGet("geocode")]
        public async Task<IActionResult> Geocode([FromQuery] string address)
        {
            var result = await _mapboxService.GeocodeAsync(address);
            if (result == null) return NotFound("Address not found");
            return Ok(result);
        }

        [HttpGet("reverse-geocode")]
        public async Task<IActionResult> ReverseGeocode([FromQuery] double lat, [FromQuery] double lng)
        {
            var result = await _mapboxService.ReverseGeocodeAsync(lat, lng);
            if (result == null) return NotFound("Location not found");
            return Ok(result);
        }

        [HttpGet("directions")]
        public async Task<IActionResult> GetDirections([FromQuery] double fromLat, [FromQuery] double fromLng, [FromQuery] double toLat, [FromQuery] double toLng)
        {
            var result = await _mapboxService.GetDirectionsAsync(fromLat, fromLng, toLat, toLng);
            if (result == null) return NotFound("Route not found");
            return Ok(result);
        }
    }
}
