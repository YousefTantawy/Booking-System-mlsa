using System;
using System.Collections.Generic;

namespace BookingSystem.Models;

public partial class Movie
{
    public int MovieId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int Duration { get; set; }

    public DateOnly ReleaseDate { get; set; }

    public string? Language { get; set; }

    public string? Genre { get; set; }

    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
