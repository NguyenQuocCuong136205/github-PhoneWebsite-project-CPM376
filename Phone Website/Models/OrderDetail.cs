using System;
using System.Collections.Generic;

namespace Phone_Website.Models;

public partial class OrderDetail
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public int? ProductId { get; set; }

    public decimal? Price { get; set; }

    public decimal? PriceSale { get; set; }

    public int? Quantity { get; set; }

    public virtual Order? Order { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
