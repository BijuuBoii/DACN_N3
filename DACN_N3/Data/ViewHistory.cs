using System;
using System.Collections.Generic;

namespace DACN_N3.Data;

public partial class ViewHistory
{
    public int ViewId { get; set; }

    public int? UserId { get; set; }

    public int? MovieId { get; set; }

    public DateTime? WatchedDate { get; set; }

    public int? Progress { get; set; }

    public virtual Movie? Movie { get; set; }

    public virtual User? User { get; set; }
}
