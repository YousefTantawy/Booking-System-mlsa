using BookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BookingSystem.DTOs;

namespace BookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class bookingController : ControllerBase
    {
        private readonly MaindbContext _context;
        public bookingController(MaindbContext context)
        {
            _context = context;
        }

        [HttpGet("Bookings")]
        [Authorize]
        public async Task<IActionResult> getBookings()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (idClaim == null)
            {
                return Unauthorized("Invalid Token: User ID not found.");
            }

            if (!int.TryParse(idClaim.Value, out int userId))
            {
                return BadRequest("Invalid Token: User ID format is incorrect.");
            }

            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .Select(b => new
                {
                    b.BookingId,
                    b.BookingTime,
                    b.Status,
                    b.TotalAmount,
                    MovieTitle = b.Showtime.Movie.Title,
                    Tickets = b.Tickets.Select(t => new
                    {
                        TicketId = t.TicketId,
                        Price = t.Price,
                        SeatNumber = t.Seat.RowChar + "-" + t.Seat.SeatNumber
                    }).ToList()
                })
                .ToListAsync();

            if (bookings.Count == 0)
            {
                return NotFound("No booking found for this user.");
            }

            return Ok(bookings);
        }

        [HttpGet("Movies")]
        public async Task<IActionResult> getMovies()
        {
            var movies = await _context.Movies
                .Select(m => new
                {
                    m.MovieId,
                    m.Title,
                    m.Description,
                    m.Language,
                    m.Genre,
                    m.Duration
                }).ToListAsync();

            if (movies.Count == 0)
            {
                return NotFound("No movies found.");
            }

            return Ok(movies);
        }

        [HttpGet("MovieDetails/{movieId}")]
        public async Task<IActionResult> getMovieDetailed(int movieId)
        {
            var moviesDetailed = await _context.Movies
                .Where(m => m.MovieId == movieId)
                .Select(m => new
                {
                    m.Title,
                    m.Description,
                    m.Language,
                    m.Genre,
                    m.Duration,
                    m.ReleaseDate,
                    Showtimes = m.Showtimes.Select(s => new
                    {
                        s.ShowtimeId,
                        s.StartTime,
                        s.Price,
                        HallName = s.Hall.Name 
                    }).ToList()
                }).FirstOrDefaultAsync();

            if (moviesDetailed == null)
            {
                return NotFound("Movie details not found");
            }

            return Ok(moviesDetailed);
        }

        [HttpPost("BookTickets")]
        [Authorize]
        public async Task<IActionResult> BookTickets([FromBody] CreateBookingDto request)
        {
            // Get User ID from Token
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            {
                return Unauthorized("Invalid Token");
            }

            // Make sure user has specified seats
            if (request.SeatIds == null || !request.SeatIds.Any())
            {
                return BadRequest("You must select at least one seat.");
            }

            // Verify the Showtime exists and get the price
            var showtime = await _context.Showtimes
                .Include(s => s.Hall)
                .FirstOrDefaultAsync(s => s.ShowtimeId == request.ShowtimeId);

            if (showtime == null)
            {
                return NotFound("Showtime not found.");
            }

            // This means dont make any changes until I confirm, but consider the following
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Verify all requested seats actually belong to this Hall
                var validSeatsCount = await _context.Seats
                    .Where(s => s.HallId == showtime.HallId && request.SeatIds.Contains(s.SeatId))
                    .CountAsync();

                if (validSeatsCount != request.SeatIds.Count)
                {
                    return BadRequest("One or more selected seats do not exist in this hall.");
                }

                // Check if any of these seats are ALREADY booked for this showtime
                var alreadyBookedSeats = await _context.Tickets
                    .Where(t => t.ShowtimeId == request.ShowtimeId && request.SeatIds.Contains(t.SeatId))
                    .AnyAsync();

                if (alreadyBookedSeats)
                {
                    return Conflict("One or more selected seats have already been booked by someone else.");
                }

                decimal totalAmount = showtime.Price * request.SeatIds.Count;

                // Create the Booking Record
                var newBooking = new Booking
                {
                    UserId = userId,
                    ShowtimeId = request.ShowtimeId,
                    BookingTime = DateTime.UtcNow,
                    TotalAmount = totalAmount,
                    Status = "Confirmed"
                };

                _context.Bookings.Add(newBooking);
                await _context.SaveChangesAsync(); // Saves to get the new BookingId

                // Create the Individual Tickets linked to that Booking
                var tickets = request.SeatIds.Select(seatId => new Ticket
                {
                    BookingId = newBooking.BookingId,
                    ShowtimeId = request.ShowtimeId,
                    HallId = showtime.HallId,
                    SeatId = seatId,
                    Price = showtime.Price
                }).ToList();

                _context.Tickets.AddRange(tickets);
                await _context.SaveChangesAsync();

                // The confirmation
                await transaction.CommitAsync();

                return Ok(new { Message = "Booking successful!", BookingId = newBooking.BookingId });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return StatusCode(500, "An error occurred while processing your booking. No charges were made.");
            }
        }
    }
}
