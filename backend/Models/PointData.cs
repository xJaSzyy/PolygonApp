using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PolygonApp.Models;

public class PointData
{
    public int Id { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public int? PolygonDataId { get; set; }  

    [JsonIgnore] 
    public PolygonData? PolygonData { get; set; }
}

