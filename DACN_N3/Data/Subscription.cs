using System;
using System.Collections.Generic;

namespace DACN_N3.Data;

public partial class Subscription
{
    public int SubscriptionId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int Duration { get; set; }

    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}
