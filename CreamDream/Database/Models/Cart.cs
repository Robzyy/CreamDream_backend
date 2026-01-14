using System;
using System.Collections.Generic;

namespace CreamDream.Database.Models;

public partial class Cart
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string CreatedAt { get; set; } = null!;

    public string UpdatedAt { get; set; } = null!;

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual User User { get; set; } = null!;
}
