-- Dummy data for coffee.co Shop table
-- Copy and paste these SQL statements into your MySQL database

-- Create the Shop table if it doesn't exist
CREATE TABLE IF NOT EXISTS Shop (
  ShopID INT AUTO_INCREMENT PRIMARY KEY,
  ShopName VARCHAR(255) NOT NULL,
  Rating DECIMAL(3,2) NOT NULL DEFAULT 0.00,
  DateEntered DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  Favorited BOOLEAN NOT NULL DEFAULT FALSE,
  Deleted BOOLEAN NOT NULL DEFAULT FALSE
);

-- Insert sample coffee shops
INSERT INTO Shop (ShopName, Rating, DateEntered, Favorited, Deleted) VALUES
('Brew & Bean', 4.8, '2024-01-15 10:30:00', TRUE, FALSE),
('The Daily Grind', 4.5, '2024-01-20 14:15:00', FALSE, FALSE),
('Café Mocha', 4.2, '2024-02-01 09:00:00', TRUE, FALSE),
('Espresso Express', 3.9, '2024-02-10 11:45:00', FALSE, FALSE),
('Java Junction', 4.7, '2024-02-15 16:20:00', TRUE, FALSE),
('Coffee Corner', 4.0, '2024-02-20 13:30:00', FALSE, FALSE),
('Bean There', 4.6, '2024-03-01 08:15:00', TRUE, FALSE),
('Roast & Toast', 3.8, '2024-03-05 15:00:00', FALSE, FALSE),
('Caffeine Fix', 4.4, '2024-03-10 10:00:00', FALSE, FALSE),
('Morning Brew', 4.9, '2024-03-15 07:30:00', TRUE, FALSE),
('Latte Love', 4.3, '2024-03-20 12:00:00', FALSE, FALSE),
('Steamy Cups', 3.7, '2024-03-25 14:45:00', FALSE, FALSE),
('Perfect Pour', 4.6, '2024-04-01 09:30:00', TRUE, FALSE),
('Urban Coffee', 4.1, '2024-04-05 11:00:00', FALSE, FALSE),
('Artisan Roast', 4.8, '2024-04-10 08:45:00', TRUE, FALSE);

-- Note: ShopID will be auto-generated
-- DateEntered can also use CURRENT_TIMESTAMP if you want current date/time:
-- INSERT INTO Shop (ShopName, Rating, DateEntered, Favorited, Deleted) VALUES
-- ('Shop Name', 4.5, CURRENT_TIMESTAMP, TRUE, FALSE);

