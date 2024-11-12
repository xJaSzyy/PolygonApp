using PolygonApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PolygonApp.Controllers;

[ApiController]
[Route("api/polygon")]
public class PolygonController : ControllerBase
{

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

    private readonly ApplicationDbContext _context;

    public PolygonController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("save")]
    public async Task<IActionResult> SavePolygons([FromBody] PolygonRequestModel request)
    {
        if (request.Polygons == null || request.Polygons.Any(p => p.Points == null || p.Points.Count < 3))
        {
            return BadRequest("Некорректные данные о полигонах");
        }

        foreach (var polygonRequest in request.Polygons)
        {
            var polygon = new PolygonData();
            _context.Polygons.Add(polygon);
            await _context.SaveChangesAsync();

            var points = polygonRequest.Points.Select(point => new PointData
            {
                Latitude = point.Latitude,
                Longitude = point.Longitude,
                PolygonDataId = polygon.Id,
            }).ToList();

            _context.Points.AddRange(points);
        }

        await _context.SaveChangesAsync();


        return Ok(new { message = "Полигоны успешно сохранены", polygons = request.Polygons });
    }


    [HttpGet("load")]
    public async Task<IActionResult> LoadPolygons()
    {
        var polygons = await _context.Polygons
            .Include(p => p.Points)
            .ToListAsync();
        return Ok(polygons);
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> DeleteAllPolygons()
    {
        _context.Points.RemoveRange(_context.Points);
        _context.Polygons.RemoveRange(_context.Polygons);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Все полигоны успешно удалены" });
    }

    public class RequestModel
    {
        public PointData Point { get; set; } = new();
        public List<PolygonData> Polygons { get; set; } = new();
    }

    public class PolygonRequestModel
    {
        public List<PolygonData> Polygons { get; set; } = new();
    }
}
