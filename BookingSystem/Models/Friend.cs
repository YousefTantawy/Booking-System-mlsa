using System;
using System.Collections.Generic;

namespace BookingSystem.Models;

public partial class Friend
{
    public int RequestId { get; set; }

    public int RequesterId { get; set; }

    public int AddresseeId { get; set; }

    public string Status { get; set; } = null!;

    public virtual User Addressee { get; set; } = null!;

    public virtual User Requester { get; set; } = null!;
}
