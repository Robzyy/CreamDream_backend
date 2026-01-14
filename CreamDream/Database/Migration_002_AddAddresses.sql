-- CreamDream Database Schema - Migration 2
-- Add Address Support for Order Delivery
-- Created: 2026-01-09

-- Addresses Table (One address per user)
CREATE TABLE Addresses (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL UNIQUE,
    FullName TEXT NOT NULL,
    PhoneNumber TEXT NOT NULL,
    StreetAddress TEXT NOT NULL,
    City TEXT NOT NULL,
    PostalCode TEXT NOT NULL,
    Country TEXT NOT NULL DEFAULT 'Romania',
    AddressNotes TEXT,
    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Add AddressId to Orders table
ALTER TABLE Orders ADD COLUMN AddressId INTEGER;

-- Add foreign key constraint (SQLite doesn't support ALTER TABLE ADD CONSTRAINT, so we note it here)
-- The constraint will be enforced in the application layer
-- FOREIGN KEY (AddressId) REFERENCES Addresses(Id) ON DELETE RESTRICT

-- Indexes for better performance
CREATE INDEX idx_addresses_userid ON Addresses(UserId);
CREATE INDEX idx_orders_addressid ON Orders(AddressId);
