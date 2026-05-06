using System;
using System.Collections.Generic;

namespace Phone_Website.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal? OriginalPrice { get; set; }

    public decimal? Price { get; set; }

    public string? Photo { get; set; }

    public int? CategoryId { get; set; }

    public int? OrderDetailId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual OrderDetail? OrderDetail { get; set; }
}
