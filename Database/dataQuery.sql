-- 1. Create Roles (Admin, User)
INSERT INTO roles (Name)
VALUES 
('Customer'),
('Admin'),
('Staff');

-- 2. Create Halls (Cinema Rooms)
INSERT INTO halls (Name, MaxCapacity)
VALUES 
('IMAX Hall A', 150),
('Standard Hall B', 100),
('VIP Lounge', 30);

-- 3. Create Movies
INSERT INTO movies (Title, Description, Duration, ReleaseDate, Language, Genre)
VALUES 
('Inception', 'A thief who enters the dreams of others.', 148, '2010-07-16', 'English', 'Sci-Fi'),
('The Dark Knight', 'Batman faces the Joker.', 152, '2008-07-18', 'English', 'Action'),
('Interstellar', 'A team travels through a wormhole.', 169, '2014-11-07', 'English', 'Sci-Fi');

-- 4. Create Users (Depends on Roles)
-- Assuming Role ID 1 = Admin, 2 = User
INSERT INTO users (Username, Email, PasswordHash, RoleId)
VALUES 
('AliceAdmin', 'alice@motive.com', 'hashed_pass_123', 1),
('BobUser', 'bob@gmail.com', 'hashed_pass_456', 2),
('CharlieUser', 'charlie@yahoo.com', 'hashed_pass_789', 2);

-- 5. Create Seats (Depends on Halls)
-- Adding seats for Hall 1 (IMAX Hall A)
INSERT INTO seats (HallId, RowChar, SeatNumber)
VALUES 
(1, 'A', 1),
(1, 'A', 2),
(1, 'B', 1),
(1, 'B', 2),
(2, 'A', 1),
(2, 'A', 2),
(2, 'B', 1),
(2, 'B', 2);

-- 6. Create Showtimes (Depends on Movies & Halls)
-- Movie 1 (Inception) in Hall 1
INSERT INTO showtimes (MovieId, HallId, StartTime, EndTime, Price)
VALUES 
(1, 1, '2026-02-10 18:00:00', '2026-02-10 20:30:00', 15.00),
(2, 2, '2026-02-11 15:00:00', '2026-02-11 17:30:00', 12.50);

-- 7. Create Bookings (Depends on Users & Showtimes)
-- User 2 (Bob) books Showtime 1
INSERT INTO bookings (UserId, ShowtimeId, BookingTime, TotalAmount, Status)
VALUES 
(1, 1, NOW(), 30.00, 'Confirmed'),
(1, 2, NOW(), 12.50, 'Pending');

-- 8. Create Tickets (Depends on Bookings & Seats)
-- Linking Booking 1 to specific seats
INSERT INTO tickets (BookingId, SeatId, Price)
VALUES 
(1, 1, 15.00), -- Booking #1, Seat #1
(1, 2, 15.00); -- Booking #1, Seat #2

-- 9. Create Friends (Depends on Users)
-- Bob (2) sends a request to Charlie (3)
INSERT INTO friends (RequesterId, AddresseeId, Status)
VALUES 
(2, 3, 'Accepted'),
(1, 2, 'Pending');

0	38	15:14:34	INSERT INTO bookings (UserId, ShowtimeId, BookingTime, TotalAmount, Status)
 VALUES 
 (1, 1, NOW(), 30.00, 'Confirmed'),
 (2, 1, NOW(), 12.50, 'Pending');