using System;
using System.Collections.Generic;

namespace DACN_N3.Data;

public partial class Episode
{
    public int EpisodeId { get; set; }

    public int? SeasonId { get; set; }

    public int EpisodeNumber { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int? Duration { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public string? VideoUrl { get; set; }

    public virtual Season? Season { get; set; }
}
