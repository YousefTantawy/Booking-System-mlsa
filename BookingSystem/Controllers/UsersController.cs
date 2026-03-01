using BookingSystem.DTOs;
using BookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace BookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MaindbContext _context;
        public UsersController(MaindbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("FriendList")]
        public async Task<IActionResult> FriendList()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null) return Unauthorized("Invalid Token: User ID not found.");
            var userId = int.Parse(idClaim.Value);

            var friends = await _context.Friends
            .Where(f => f.RequesterId == userId || f.AddresseeId == userId)
            .Select(f => new
            {
                FriendId = f.RequesterId == userId ? f.Addressee.Id : f.Requester.Id,
                FriendName = f.RequesterId == userId ? f.Addressee.Username : f.Requester.Username,
                FriendEmail = f.RequesterId == userId ? f.Addressee.Email : f.Requester.Email,
                FriendStatus = f.Status
            })
            .ToListAsync();

            if (friends.Count == 0)
            {
                return NotFound("No friends found for this user.");
            }

            return Ok(friends);
        }

        [Authorize]
        [HttpPost("FriendRequest")]
        public async Task<IActionResult> FriendRequest(int friendId)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null) return Unauthorized("Invalid Token: User ID not found.");
            var userId = int.Parse(idClaim.Value);

            if (userId == friendId)
            {
                return BadRequest("You cannot send a friend request to yourself.");
            }

            var targetUser = await _context.Users.FindAsync(friendId);
            if (targetUser == null)
            {
                return NotFound("User not found.");
            }

            var existingRequest = await _context.Friends
                .FirstOrDefaultAsync(f =>
                    (f.RequesterId == userId && f.AddresseeId == friendId) ||
                    (f.RequesterId == friendId && f.AddresseeId == userId));

            if (existingRequest != null)
            {
                return BadRequest($"Relationship already exists (Status: {existingRequest.Status})");
            }
            
            var newFriendship = new Friend
            {
                RequesterId = userId,
                AddresseeId = friendId,
                Status = "Pending"
            };

            _context.Friends.Add(newFriendship);
            await _context.SaveChangesAsync();

            return Ok("Friend Request Sent Successfully.");
        }

        [Authorize]
        [HttpPost("AcceptRequest")] 
        public async Task<IActionResult> AcceptFriendRequest(int requesterId)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null) return Unauthorized("Invalid Token: User ID not found.");
            int myId = int.Parse(idClaim.Value);

            var friendship = await _context.Friends
                .FirstOrDefaultAsync(f =>
                    f.RequesterId == requesterId &&
                    f.AddresseeId == myId);
            
            if (friendship == null)
            {
                return NotFound("Friend request not found.");
            }

            if (friendship.Status == "Accepted")
            {
                return BadRequest("You are already friends.");
            }

            friendship.Status = "Accepted";

            await _context.SaveChangesAsync();

            return Ok("Friend request accepted!");
        }
    }
}
