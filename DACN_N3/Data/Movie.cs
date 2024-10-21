using System;
using System.Collections.Generic;

namespace DACN_N3.Data;

public partial class Movie
{
    public int MovieId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public int? Duration { get; set; }

    public string? Director { get; set; }

    public string? Cast { get; set; }

    public string? Language { get; set; }

    public string? AgeRating { get; set; }

    public string? Poster { get; set; }

    public string? Trailer { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Season> Seasons { get; set; } = new List<Season>();

    public virtual ICollection<ViewHistory> ViewHistories { get; set; } = new List<ViewHistory>();

    public virtual ICollection<Watchlist> Watchlists { get; set; } = new List<Watchlist>();

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
