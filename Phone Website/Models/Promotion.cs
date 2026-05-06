using System;
using System.Collections.Generic;

namespace Phone_Website.Models;

public partial class Promotion
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? Type { get; set; }

    public double? Discount { get; set; }

    public string? Code { get; set; }

    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
