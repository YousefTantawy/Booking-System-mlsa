using BookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

            var userId = int.Parse(idClaim.Value);

            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .Select(b => new
                {
                    b.Id,
                    b.BookingTime,
                    b.Status,
                    b.TotalAmount,
                    MovieTitle = b.Showtime.Movie.Title,
                    Tickets = b.Tickets.Select(t => new
                    {
                        TicketId = t.Id,
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
                    m.Id,
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

        [HttpGet("movies/{movieId}")]
        public async Task<IActionResult> getMovieDetailed(int movieId)
        {
            var moviesDetailed = await _context.Movies
                .Where(m => m.Id == movieId)
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
                        s.Id,
                        s.StartTime,
                        s.Price,
                        HallName = s.Hall.Name 
                    }).ToList()

                }).ToListAsync();

            if (moviesDetailed.Count == 0)
            {
                return NotFound("Movie details not found");
            }

            return Ok(moviesDetailed);
        }


    }
}
