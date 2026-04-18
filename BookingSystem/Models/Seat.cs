using System;
using System.Collections.Generic;

namespace BookingSystem.Models;

public partial class Seat
{
    public int SeatId { get; set; }

    public int HallId { get; set; }

    public string RowChar { get; set; } = null!;

    public int SeatNumber { get; set; }

    public virtual Hall Hall { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
