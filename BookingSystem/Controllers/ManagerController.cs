using BookingSystem.DTOs;
using Microsoft.EntityFrameworkCore;
using BookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // ONLY users with a token containing the "Manager" role can even enter this controller
    [Authorize(Roles = "Manager")]
    public class ManagerController : ControllerBase
    {
        private readonly MaindbContext _context;

        public ManagerController(MaindbContext context)
        {
            _context = context;
        }

        // ------------------- MOVIES -------------------
        [HttpPost("AddMovies")]
        public async Task<IActionResult> AddMovie(CreateMovieDto request)
        {
            var newMovie = new Movie
            {
                Title = request.Title,
                Description = request.Description,
                Duration = request.Duration,
                ReleaseDate = request.ReleaseDate,
                Language = request.Language,
                Genre = request.Genre
            };

            _context.Movies.Add(newMovie);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Movie added successfully!", MovieId = newMovie.MovieId });
        }

        [HttpDelete("DeleteMovie/{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();

            // Note: Deleting a movie will fail if there are showtimes attached to it.
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return Ok("Movie deleted.");
        }

        // ------------------- HALLS -------------------
        [HttpPost("AddHalls")]
        public async Task<IActionResult> AddHall([FromBody] CreateHallDto request)
        {
            var newHall = new Hall
            {
                Name = request.Name
            };

            _context.Halls.Add(newHall);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Hall added successfully!", HallId = newHall.HallId });
        }

        [HttpDelete("DeleteHall/{hallId}")]
        public async Task<IActionResult> DeleteHall(int hallId)
        {
            var hall = await _context.Halls.FindAsync(hallId);
            if (hall == null) return NotFound($"Hall with ID {hallId} not found.");

            // DEFENSIVE CHECK 1: Are there any showtimes scheduled here?
            var hasShowtimes = await _context.Showtimes.AnyAsync(s => s.HallId == hallId);
            if (hasShowtimes)
            {
                return Conflict("Cannot delete this hall. There are active showtimes scheduled inside it. Reassign or delete the showtimes first.");
            }

            var seatsToDelete = await _context.Seats
            .Where(s => s.HallId == hallId)
            .ToListAsync();

            _context.Seats.RemoveRange(seatsToDelete);
            _context.Halls.Remove(hall);
            await _context.SaveChangesAsync();

            return Ok("Hall deleted successfully.");
        }
        // ------------------- SEATS -------------------
        [HttpPost("Seats")]
        public async Task<IActionResult> AddSeat([FromBody] CreateSeatDto request)
        {
            var hall = await _context.Halls.FindAsync(request.HallId);
            if (hall == null)
            {
                return NotFound($"Hall with ID {request.HallId} does not exist.");
            }

            var newSeat = new Seat
            {
                HallId = request.HallId,
                RowChar = request.RowChar,
                SeatNumber = request.SeatNumber
            };

            hall.MaxCapacity += 1;

            _context.Seats.Add(newSeat);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Seat added successfully!", SeatId = newSeat.SeatId });
        }

        [HttpDelete("Seats/{hallId}/{seatId}")]
        public async Task<IActionResult> DeleteSeat(int hallId, int seatId)
        {
            var seat = await _context.Seats.FindAsync(hallId, seatId);
            if (seat == null) return NotFound("Seat not found.");

            var hall = await _context.Halls.FindAsync(hallId);
            if (hall == null) return NotFound($"Hall with ID {hallId} does not exist.");

            var tickets = await _context.Tickets.AnyAsync(t => t.HallId == hallId && t.SeatId == seatId);
            if (tickets) return Conflict("Cannot delete seat. Tickets exist for this selected seat."); 

            _context.Seats.Remove(seat);
            hall.MaxCapacity -= 1;
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Seat removed successfully!"});
        }
        // ------------------- SHOWTIMES -------------------
        [HttpPost("AddShowtimes")]
        public async Task<IActionResult> AddShowtime([FromBody] CreateShowtimeDto request)
        {
            // Defensive Check: Do both the Movie and the Hall exist?
            var movieExists = await _context.Movies.AnyAsync(m => m.MovieId == request.MovieId);
            if (!movieExists) return NotFound($"Movie with ID {request.MovieId} does not exist.");

            var hallExists = await _context.Halls.AnyAsync(h => h.HallId == request.HallId);
            if (!hallExists) return NotFound($"Hall with ID {request.HallId} does not exist.");

            // Defensive Check: Is there a time conflict?
            // (We don't want two movies playing in Hall 1 at the exact same time)
            var hasConflict = await _context.Showtimes.AnyAsync(s =>
                s.HallId == request.HallId &&
                s.StartTime < request.EndTime &&
                s.EndTime > request.StartTime);

            if (hasConflict)
            {
                return Conflict("There is already a showtime scheduled in this hall during that time.");
            }

            var newShowtime = new Showtime
            {
                MovieId = request.MovieId,
                HallId = request.HallId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Price = request.Price
            };

            _context.Showtimes.Add(newShowtime);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Showtime scheduled successfully!", ShowtimeId = newShowtime.ShowtimeId });
        }

        [HttpDelete("DeleteShowtime/{showtimeId}")]
        public async Task<IActionResult> DeleteShowtime(int showtimeId)
        {
            var showtime = await _context.Showtimes.FindAsync(showtimeId);
            if (showtime == null) return NotFound($"Showtime with ID {showtimeId} not found.");

            // DEFENSIVE CHECK: Are there any bookings attached to this showtime?
            var hasBookings = await _context.Bookings.AnyAsync(b => b.ShowtimeId == showtimeId);
            if (hasBookings)
            {
                return Conflict("Cannot cancel this showtime. Users have already booked tickets. You must refund/cancel the bookings first.");
            }

            _context.Showtimes.Remove(showtime);
            await _context.SaveChangesAsync();

            return Ok("Showtime removed successfully.");
        }
    }
}