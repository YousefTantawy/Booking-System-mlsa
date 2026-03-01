-- 1. Roles (Admin, Customer, etc.)
CREATE TABLE Roles (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(50) NOT NULL UNIQUE
);

-- 2. Users (Accounts)
CREATE TABLE Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(100) NOT NULL,
    Email VARCHAR(150) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    RoleId INT NOT NULL,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- 3. Movies (The Content)
CREATE TABLE Movies (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(200) NOT NULL,
    Description TEXT,
    Duration INT NOT NULL, -- In minutes
    ReleaseDate DATE,
    Language VARCHAR(50),
    Genre VARCHAR(50)
);

-- 4. Halls (The Physical Rooms)
CREATE TABLE Halls (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    MaxCapacity INT NOT NULL
);

-- 5. Seats (The Chairs inside Halls)
-- We split 'seat' into Row and Number for better sorting
CREATE TABLE Seats (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    HallId INT NOT NULL,
    RowChar CHAR(1) NOT NULL, -- e.g., 'A', 'B'
    SeatNumber INT NOT NULL,  -- e.g., 1, 2, 3
    FOREIGN KEY (HallId) REFERENCES Halls(Id),
    UNIQUE(HallId, RowChar, SeatNumber) -- Prevents duplicate seats in the same spot
);

-- 6. Showtimes (The Schedule)
CREATE TABLE Showtimes (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    MovieId INT NOT NULL,
    HallId INT NOT NULL,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME NOT NULL, -- Calculated in code, stored here for ease
    Price DECIMAL(10, 2) NOT NULL, -- Standard price for this specific time
    FOREIGN KEY (MovieId) REFERENCES Movies(Id),
    FOREIGN KEY (HallId) REFERENCES Halls(Id)
);

-- 7. Bookings (The Transaction Header)
CREATE TABLE Bookings (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    ShowtimeId INT NOT NULL,
    BookingTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    TotalAmount DECIMAL(10, 2) NOT NULL,
    Status VARCHAR(20) NOT NULL, -- 'Pending', 'Confirmed', 'Cancelled'
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (ShowtimeId) REFERENCES Showtimes(Id)
);

-- 8. Tickets (The Individual Seats Purchased)
CREATE TABLE Tickets (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    BookingId INT NOT NULL,
    SeatId INT NOT NULL,
    Price DECIMAL(10, 2) NOT NULL, -- Snapshot of price at purchase time
    FOREIGN KEY (BookingId) REFERENCES Bookings(Id),
    FOREIGN KEY (SeatId) REFERENCES Seats(Id)
);

-- 9. Friends (Social)
CREATE TABLE Friends (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    RequesterId INT NOT NULL, -- The user who sent the request
    AddresseeId INT NOT NULL, -- The user receiving the request
    Status VARCHAR(20) NOT NULL, -- 'Pending', 'Accepted', 'Blocked'
    FOREIGN KEY (RequesterId) REFERENCES Users(Id),
    FOREIGN KEY (AddresseeId) REFERENCES Users(Id)
);