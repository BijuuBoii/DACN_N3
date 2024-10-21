using System;
using System.Collections.Generic;

namespace DACN_N3.Data;

public partial class Watchlist
{
    public int WatchlistId { get; set; }

    public int? UserId { get; set; }

    public int? MovieId { get; set; }

    public DateTime? AddedDate { get; set; }

    public virtual Movie? Movie { get; set; }

    public virtual User? User { get; set; }
}
