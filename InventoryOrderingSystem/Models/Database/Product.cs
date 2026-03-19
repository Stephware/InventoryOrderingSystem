using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryOrderingSystem.Models.Database;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    [ConcurrencyCheck]
    public int Stock { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
