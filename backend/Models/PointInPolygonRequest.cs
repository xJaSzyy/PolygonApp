namespace PolygonApp.Models
{
    public class PointInPolygonRequest
    {
        public double PointX { get; set; }
        public double PointY { get; set; }

        public List<Point>? Polygon { get; set; }
    }

    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}
