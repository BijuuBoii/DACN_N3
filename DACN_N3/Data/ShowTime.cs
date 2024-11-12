using System;
using System.Collections.Generic;

namespace DACN_N3.Data;

public partial class ShowTime
{
    public int ShowTimeId { get; set; }

    public int? MovieId { get; set; }

    public DateOnly ShowDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string CinemaRoom { get; set; } = null!;

    public virtual Movie? Movie { get; set; }

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
