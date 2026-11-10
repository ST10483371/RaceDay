using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RaceDay.Web.Models;

// Mirrors RaceDay.API's Race model. JSON names match what the API sends and expects.
public class Race
{
    [JsonPropertyName("raceID")]
    public int RaceID { get; set; }

    [Required, StringLength(150)]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(150)]
    [JsonPropertyName("location")]
    public string Location { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Start time")]
    [JsonPropertyName("startTime")]
    public string StartTime { get; set; } = string.Empty;

    [Required]
    [Display(Name = "End time")]
    [JsonPropertyName("endTime")]
    public string EndTime { get; set; } = string.Empty;

    [Range(0, 10000)]
    [Display(Name = "Distance (km)")]
    [JsonPropertyName("distance_km")]
    public int Distance_km { get; set; }

    [Required]
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}
