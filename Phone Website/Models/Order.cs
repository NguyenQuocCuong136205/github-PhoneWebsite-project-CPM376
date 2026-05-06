using System;
using System.Collections.Generic;

namespace Phone_Website.Models;

public partial class Order
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public DateTime? OrderDay { get; set; }

    public int? PromotionId { get; set; }

    public string? ShippingPhone { get; set; }

    public string? ShippingAddress { get; set; }

    public int? StatusId { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User? User { get; set; }
}
