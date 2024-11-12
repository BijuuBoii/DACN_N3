using System;
using System.Collections.Generic;

namespace DACN_N3.Data;

public partial class Advertisement
{
    public int AdId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool? IsActive { get; set; }
}
