using System;
using System.Collections.Generic;

namespace BookingSystem.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Friend> FriendAddressees { get; set; } = new List<Friend>();

    public virtual ICollection<Friend> FriendRequesters { get; set; } = new List<Friend>();

    public virtual Role Role { get; set; } = null!;
}
