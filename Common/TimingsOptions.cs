namespace Common;

public class TimingsOptions
{
    public const string Section = "Timing";

    public int CompilationOffsetSecs { get; set; }

    public int CompilationDurationPredictionSecs { get; set; }

    public int PlaneDocLifetimesSecs { get; set; }
}
