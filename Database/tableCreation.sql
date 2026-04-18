-- 1. Roles: No duplicate role names
CREATE TABLE roles (
    RoleId INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(50) UNIQUE NOT NULL -- Prevents two 'Admin' roles
);

-- 2. Halls: No duplicate hall names
CREATE TABLE halls (
    HallId INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) UNIQUE NOT NULL, -- Prevents two 'IMAX Hall A's
    MaxCapacity INT NOT NULL
);

-- 3. Movies: No duplicate movies
CREATE TABLE movies (
    MovieId INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Description TEXT,
    Duration INT NOT NULL,
    ReleaseDate DATE NOT NULL,
    Language VARCHAR(50),
    Genre VARCHAR(50),
    -- Prevents adding the exact same movie on the same date (allows for remakes)
    UNIQUE (Title, ReleaseDate) 
);

-- 4. Users: No duplicate emails or usernames
CREATE TABLE users (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) UNIQUE NOT NULL, -- No duplicate usernames
    Email VARCHAR(100) UNIQUE NOT NULL,   -- No duplicate emails
    PasswordHash VARCHAR(255) NOT NULL,
    RoleId INT NOT NULL,
    FOREIGN KEY (RoleId) REFERENCES roles(RoleId)
);

-- 5. Seats: No duplicate physical seats in the same hall
CREATE TABLE seats (
    SeatId INT AUTO_INCREMENT PRIMARY KEY,
    HallId INT NOT NULL,
    RowChar VARCHAR(5) NOT NULL,
    SeatNumber INT NOT NULL,
    FOREIGN KEY (HallId) REFERENCES halls(HallId),
    -- THE FIX: Prevents creating two 'Seat A1's in Hall 1
    UNIQUE (HallId, RowChar, SeatNumber) 
);

-- 6. Showtimes: No overlapping exact showtimes
CREATE TABLE showtimes (
    ShowtimeId INT AUTO_INCREMENT PRIMARY KEY,
    MovieId INT NOT NULL,
    HallId INT NOT NULL,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (MovieId) REFERENCES movies(MovieId),
    FOREIGN KEY (HallId) REFERENCES halls(HallId),
    -- THE FIX: Prevents scheduling two movies in the same hall at the exact same time
    UNIQUE (HallId, StartTime) 
);

-- 7. Bookings
CREATE TABLE bookings (
    BookingId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    ShowtimeId INT NOT NULL,
    BookingTime DATETIME NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL,
    Status VARCHAR(50) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES users(UserId),
    FOREIGN KEY (ShowtimeId) REFERENCES showtimes(ShowtimeId)
);

-- 8. Tickets: The "Double Booking" Prevention
CREATE TABLE tickets (
    TicketId INT AUTO_INCREMENT PRIMARY KEY,
    BookingId INT NOT NULL,
    ShowtimeId INT NOT NULL, -- WE ADDED THIS TO ENFORCE THE CONSTRAINT
    SeatId INT NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (BookingId) REFERENCES bookings(BookingId),
    FOREIGN KEY (ShowtimeId) REFERENCES showtimes(ShowtimeId),
    FOREIGN KEY (SeatId) REFERENCES seats(SeatId),
    -- THE FIX: Prevents the same seat from being sold twice for the same showtime
    UNIQUE (ShowtimeId, SeatId) 
);

-- 9. Friends: No duplicate friend requests
CREATE TABLE friends (
    RequestId INT AUTO_INCREMENT PRIMARY KEY,
    RequesterId INT NOT NULL,
    AddresseeId INT NOT NULL,
    Status VARCHAR(50) NOT NULL,
    FOREIGN KEY (RequesterId) REFERENCES users(UserId),
    FOREIGN KEY (AddresseeId) REFERENCES users(UserId),
    -- THE FIX: Bob can only send one friend request to Charlie
    UNIQUE (RequesterId, AddresseeId),
    -- PRO-TIP: Prevents Bob from befriending himself
    CHECK (RequesterId != AddresseeId) 
);