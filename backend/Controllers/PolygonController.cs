using PolygonApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolygonApp.Controllers
{
    [ApiController]
    [Route("api/polygon")]
    public class PolygonController : ControllerBase
    {
        private const string FilePath = "polygons.json";

        [HttpPost("contains")]
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

        [HttpPost("save")]
        public async Task<IActionResult> SavePolygons([FromBody] RequestModel request)
        {
            if (request.Polygons == null || request.Polygons.Any(p => p.Points == null || p.Points.Count < 3))
            {
                return BadRequest("Некорректные данные о полигонах");
            }

            var existingPolygons = await LoadPolygonsFromFile();

            existingPolygons.AddRange(request.Polygons);
            await SavePolygonsToFile(existingPolygons);

            return Ok(new { message = "Полигоны успешно сохранены", polygons = request.Polygons });
        }


        [HttpGet("load")]
        public async Task<IActionResult> LoadPolygons()
        {
            var polygons = await LoadPolygonsFromFile();
            return Ok(polygons);
        }

        private async Task SavePolygonsToFile(List<PolygonData> polygons)
        {
            var json = JsonSerializer.Serialize(polygons, new JsonSerializerOptions { WriteIndented = true });
            await System.IO.File.WriteAllTextAsync(FilePath, json);
        }

        private async Task<List<PolygonData>> LoadPolygonsFromFile()
        {
            if (!System.IO.File.Exists(FilePath))
            {
                return new List<PolygonData>();
            }

            var json = await System.IO.File.ReadAllTextAsync(FilePath);
            return JsonSerializer.Deserialize<List<PolygonData>>(json) ?? new List<PolygonData>();
        }

        [HttpDelete("clear")]
        public IActionResult DeleteAllPolygons()
        {
            if (System.IO.File.Exists(FilePath))
            {
                System.IO.File.Delete(FilePath);
            }

            return Ok(new { message = "Все полигоны успешно удалены" });
        }

        public class RequestModel
        {
            public PointData Point { get; set; } = new();
            public List<PolygonData> Polygons { get; set; } = new();
        }
    }
}