using System.ComponentModel.DataAnnotations;

namespace Common;

public class TimingsOptions
{
    public const string Section = "Timing";

    [Required]
    [Range(1, 60)]
    public int CompilationOffsetSecs { get; set; }

    [Required]
    [Range(1, 60)]
    public int CompilationDurationPredictionSecs { get; set; }

    [Required]
    [Range(15, 60)]
    public int PlaneDocLifetimesSecs { get; set; }
}
