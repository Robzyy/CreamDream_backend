using System;
using System.Collections.Generic;

namespace CreamDream.Database.Models;

public partial class RefreshToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public string ExpiresAt { get; set; } = null!;

    public string CreatedAt { get; set; } = null!;

    public int IsRevoked { get; set; }

    public virtual User User { get; set; } = null!;
}
