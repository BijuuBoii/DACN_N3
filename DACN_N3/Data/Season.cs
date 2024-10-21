using System;
using System.Collections.Generic;

namespace DACN_N3.Data;

public partial class Season
{
    public int SeasonId { get; set; }

    public int? MovieId { get; set; }

    public int SeasonNumber { get; set; }

    public string? Description { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public virtual ICollection<Episode> Episodes { get; set; } = new List<Episode>();

    public virtual Movie? Movie { get; set; }
}
