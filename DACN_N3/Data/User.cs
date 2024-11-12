using System;
using System.Collections.Generic;

namespace DACN_N3.Data;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<CinemaTicket> CinemaTickets { get; set; } = new List<CinemaTicket>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();

    public virtual ICollection<ViewHistory> ViewHistories { get; set; } = new List<ViewHistory>();

    public virtual ICollection<Watchlist> Watchlists { get; set; } = new List<Watchlist>();
}
