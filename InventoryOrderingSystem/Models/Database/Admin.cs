using System;
using System.Collections.Generic;

namespace InventoryOrderingSystem.Models.Database;

public partial class Admin
{
    public int AdminId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;
}
