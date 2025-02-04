using System.ComponentModel.DataAnnotations;

namespace Common;

public class MetadataOptions
{
    public const string Section = "Metadata";

    [Required]
    public int Scale { get; set; }
}
