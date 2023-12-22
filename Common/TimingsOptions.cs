using System.ComponentModel.DataAnnotations;

namespace Common;

public class TimingsOptions
{
    public const string Timing = "Timing";
    
    public int CompilationOffsetSecs => 1;
    
    public int CompilationDurationPredictionSecs => 1;
}