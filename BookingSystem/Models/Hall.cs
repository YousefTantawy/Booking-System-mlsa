using System;
using System.Collections.Generic;

namespace BookingSystem.Models;

public partial class Hall
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int MaxCapacity { get; set; }

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
