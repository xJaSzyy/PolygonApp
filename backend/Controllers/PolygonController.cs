using PolygonApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace PolygonApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PolygonController : ControllerBase
    {
        [HttpPost("polygon/checkPoint")]
        public IActionResult CheckPointInPolygons([FromBody] RequestModel request)
        {
            foreach (var polygon in request.Polygons)
            {
                bool isPointInside = IsPointInPolygon(request.Point, polygon.Points);
                if (isPointInside)
                {
                    return Ok(new { point = request.Point, isPointInside });
                }
            }
            return Ok(new { point = request.Point, isPointInside = false });
        }

        private bool IsPointInPolygon(PointData point, List<PointData> polygon)
        {
            int n = polygon.Count;
            bool inside = false;
            double x = point.Longitude, y = point.Latitude;

            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                double xi = polygon[i].Longitude, yi = polygon[i].Latitude;
                double xj = polygon[j].Longitude, yj = polygon[j].Latitude;

                bool intersect = ((yi > y) != (yj > y)) &&
                                 (x < (xj - xi) * (y - yi) / (yj - yi) + xi);
                if (intersect)
                    inside = !inside;
            }
            return inside;
        }

        public class RequestModel
        {
            public PointData Point { get; set; } = new();
            public List<PolygonData> Polygons { get; set; } = new();
        }

    }
}