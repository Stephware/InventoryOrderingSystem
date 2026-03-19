using System;
using System.Collections.Generic;

namespace InventoryOrderingSystem.Models.Database;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
