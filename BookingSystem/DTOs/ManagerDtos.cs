namespace BookingSystem.DTOs
{
    public class CreateMovieDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Duration { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public string Language { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
    }
    public class CreateHallDto
    {
        public string Name { get; set; } = string.Empty;
    }
    public class CreateSeatDto
    {
        public int HallId { get; set; }
        public string RowChar { get; set; } = string.Empty;
        public int SeatNumber { get; set; }
    }
    public class CreateShowtimeDto
    {
        public int MovieId { get; set; }
        public int HallId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Price { get; set; }
    }
}
