using System.ComponentModel.DataAnnotations;

namespace BookingSystem.DTOs
{
	public class BookingDto
	{
		[Required]
		public string Username { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[MinLength(6)]
		public string Password { get; set; }
	}

}