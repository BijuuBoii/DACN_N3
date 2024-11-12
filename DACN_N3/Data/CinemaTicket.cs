using System;
using System.Collections.Generic;

namespace DACN_N3.Data;

public partial class CinemaTicket
{
    public int TicketId { get; set; }

    public int? MovieId { get; set; }

    public int? UserId { get; set; }

    public DateTime ShowTime { get; set; }

    public string SeatNumber { get; set; } = null!;

    public decimal TicketPrice { get; set; }

    public DateTime? BookingDate { get; set; }

    public int? SeatId { get; set; }

    public virtual Movie? Movie { get; set; }

    public virtual Seat? Seat { get; set; }

    public virtual User? User { get; set; }
}
