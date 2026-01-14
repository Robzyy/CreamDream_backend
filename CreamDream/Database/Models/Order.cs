using System;
using System.Collections.Generic;

namespace CreamDream.Database.Models;

public partial class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string OrderNumber { get; set; } = null!;

    public string Status { get; set; } = null!;

    public double TotalAmount { get; set; }

    public string OrderDate { get; set; } = null!;

    public string? CompletedAt { get; set; }

    public string? Notes { get; set; }

    public int? AddressId { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User User { get; set; } = null!;
}
