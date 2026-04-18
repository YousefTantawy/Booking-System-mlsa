using System;
using System.Collections.Generic;

namespace BookingSystem.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int BookingId { get; set; }

    public int ShowtimeId { get; set; }

    public int HallId { get; set; }

    public int SeatId { get; set; }

    public decimal Price { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Seat Seat { get; set; } = null!;

    public virtual Showtime Showtime { get; set; } = null!;
}
