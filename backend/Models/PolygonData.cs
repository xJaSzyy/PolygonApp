namespace PolygonApp.Models;

public class PolygonData
{
    public int Id { get; set; }
    public List<PointData> Points { get; set; } = new();
}
