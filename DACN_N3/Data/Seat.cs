using System;
using System.Collections.Generic;

namespace DACN_N3.Data;

public partial class Seat
{
    public int SeatId { get; set; }

    public int? ShowTimeId { get; set; }

    public string SeatNumber { get; set; } = null!;

    public bool? IsVip { get; set; }

    public bool? IsAvailable { get; set; }

    public virtual ICollection<CinemaTicket> CinemaTickets { get; set; } = new List<CinemaTicket>();

    public virtual ShowTime? ShowTime { get; set; }
}
