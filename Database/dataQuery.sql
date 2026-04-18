-- 1. Create Roles
INSERT INTO roles (Name)
VALUES 
('Customer'),
('Admin'),
('Manager');

-- 2. Create Halls (Cinema Rooms)
INSERT INTO halls (Name, MaxCapacity)
VALUES 
('ScreenX Immersive', 120),
('Dolby Cinema', 80),
('Private Lounge', 20);

-- 3. Create Movies
INSERT INTO movies (Title, Description, Duration, ReleaseDate, Language, Genre)
VALUES 
('Dune: Part Two', 'Paul Atreides unites with Chani and the Fremen.', 166, '2024-03-01', 'English', 'Sci-Fi'),
('Spider-Man: Across the Spider-Verse', 'Miles Morales catapults across the Multiverse.', 140, '2023-06-02', 'English', 'Animation'),
('Oppenheimer', 'The story of the atomic bomb development.', 180, '2023-07-21', 'English', 'Biography');