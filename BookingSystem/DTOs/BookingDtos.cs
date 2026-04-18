using System.ComponentModel.DataAnnotations;

namespace BookingSystem.DTOs
{
	public class CreateBookingDto
    {
        public int ShowtimeId { get; set; }
        public List<int> SeatIds { get; set; } = new List<int>();
    }

}