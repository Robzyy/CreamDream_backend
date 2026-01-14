-- CreamDream Database Schema
-- SQLite Database-First Migration
-- Created: 2026-01-09

-- Users Table
CREATE TABLE Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
    Email TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    Role TEXT NOT NULL CHECK(Role IN ('Customer', 'Admin')),
    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    DeletedAt TEXT
);

-- Refresh Tokens Table (for JWT token refresh functionality)
CREATE TABLE RefreshTokens (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    Token TEXT NOT NULL UNIQUE,
    ExpiresAt TEXT NOT NULL,
    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    IsRevoked INTEGER NOT NULL DEFAULT 0 CHECK(IsRevoked IN (0, 1)),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Product Types Table
CREATE TABLE ProductTypes (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,
    Description TEXT
);

-- Categories Table
CREATE TABLE Categories (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,
    Description TEXT
);

-- Products Table
CREATE TABLE Products (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Description TEXT,
    Price REAL NOT NULL CHECK(Price >= 0),
    ProductTypeId INTEGER NOT NULL,
    CategoryId INTEGER,
    ImageUrl TEXT,
    IsAvailable INTEGER NOT NULL DEFAULT 1 CHECK(IsAvailable IN (0, 1)),
    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProductTypeId) REFERENCES ProductTypes(Id) ON DELETE RESTRICT,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE SET NULL
);

-- Carts Table (persistent carts for users)
CREATE TABLE Carts (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL UNIQUE,
    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Cart Items Table
CREATE TABLE CartItems (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    CartId INTEGER NOT NULL,
    ProductId INTEGER NOT NULL,
    Quantity INTEGER NOT NULL CHECK(Quantity > 0),
    AddedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (CartId) REFERENCES Carts(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
    UNIQUE(CartId, ProductId)
);

-- Orders Table
CREATE TABLE Orders (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    OrderNumber TEXT NOT NULL UNIQUE,
    Status TEXT NOT NULL CHECK(Status IN ('Pending', 'Processing', 'Completed', 'Cancelled')),
    TotalAmount REAL NOT NULL CHECK(TotalAmount >= 0),
    OrderDate TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CompletedAt TEXT,
    Notes TEXT,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE RESTRICT
);

-- Order Items Table (with price snapshots)
CREATE TABLE OrderItems (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    OrderId INTEGER NOT NULL,
    ProductId INTEGER NOT NULL,
    ProductName TEXT NOT NULL,
    Quantity INTEGER NOT NULL CHECK(Quantity > 0),
    UnitPrice REAL NOT NULL CHECK(UnitPrice >= 0),
    Subtotal REAL NOT NULL CHECK(Subtotal >= 0),
    FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE RESTRICT
);

-- Indexes for better query performance
CREATE INDEX idx_refreshtokens_userid ON RefreshTokens(UserId);
CREATE INDEX idx_refreshtokens_token ON RefreshTokens(Token);
CREATE INDEX idx_products_producttypeid ON Products(ProductTypeId);
CREATE INDEX idx_products_categoryid ON Products(CategoryId);
CREATE INDEX idx_products_isavailable ON Products(IsAvailable);
CREATE INDEX idx_carts_userid ON Carts(UserId);
CREATE INDEX idx_cartitems_cartid ON CartItems(CartId);
CREATE INDEX idx_cartitems_productid ON CartItems(ProductId);
CREATE INDEX idx_orders_userid ON Orders(UserId);
CREATE INDEX idx_orders_status ON Orders(Status);
CREATE INDEX idx_orders_orderdate ON Orders(OrderDate);
CREATE INDEX idx_orderitems_orderid ON OrderItems(OrderId);

-- Seed Product Types
INSERT INTO ProductTypes (Name, Description) VALUES 
    ('Donut', 'Delicious donuts in various flavors'),
    ('Coffee', 'Hot and cold coffee drinks'),
    ('ColdDrink', 'Refreshing cold beverages'),
    ('HotDrink', 'Warm beverages'),
    ('Bagel', 'Fresh bagels with toppings');

-- Seed Sample Categories (can be modified later)
INSERT INTO Categories (Name, Description) VALUES 
    ('Classic', 'Traditional favorites'),
    ('Specialty', 'Unique and premium items'),
    ('Seasonal', 'Limited time offerings'),
    ('Vegan', 'Plant-based options');

-- Seed Sample Products
INSERT INTO Products (Name, Description, Price, ProductTypeId, CategoryId, IsAvailable) VALUES 
    -- Donuts
    ('Plain Donut', 'Ingredients (approx. 73 g): flour, milk, sugar, egg, butter, yeast, salt, powdered sugar', 2.00, 1, 1, 1),
    ('Salted Caramel Donut', 'Ingredients (120 g): flour, milk, sugar, egg, butter, yeast, salt, white chocolate, milk chocolate, liquid cream, liquid glucose, powdered sugar', 3.20, 1, 2, 1),
    ('Boston Cream Donut', 'Flour, milk, sugar, egg, butter, yeast, salt, white chocolate, cornstarch, vanilla paste, liquid cream, black chocolate glaze (110g)', 3.20, 1, 2, 1),
    ('Strawberry White Chocolate Donut', 'Flour, milk, sugar, egg, butter, yeast, salt, white chocolate, liquid cream, gelatin, strawberry paste, white chocolate glaze, freeze-dried strawberry (110g)', 3.20, 1, 2, 1),
    ('Pistachio Raspberry Donut', 'Flour, milk, sugar, egg, butter, yeast, salt, white chocolate, liquid cream, gelatin, pistachio paste, raspberry puree, white chocolate glaze with pistachio, freeze-dried raspberry (110g)', 3.20, 1, 2, 1),
    ('Vanilla Donut', 'Ingredients (120 g): flour, milk, sugar, egg, butter, yeast, salt, white chocolate, cornstarch, vanilla paste, liquid cream, powdered sugar', 3.20, 1, 1, 1),
    ('Cini Minis Donut', 'Flour, milk, sugar, egg, butter, yeast, salt, white chocolate, liquid cream, gelatin, spiced chai powder, white chocolate glaze, Cini Minis cereal (110g)', 3.20, 1, 2, 1),
    ('Creme Brulee Donut', 'Ingredients (120 g): flour, milk, sugar, egg, butter, yeast, salt, white chocolate, cornstarch, vanilla paste, liquid cream, caramelized brown sugar', 3.20, 1, 2, 1),
    ('Coconut Raspberry Donut', 'Ingredients (120 g): flour, milk, sugar, egg, butter, yeast, salt, white chocolate, cornstarch, coconut paste, liquid cream, gelatin, raspberry jam, powdered sugar', 3.20, 1, 1, 1),
    ('Apple Pie Donut', 'Flour, milk, sugar, egg, butter, yeast, salt, white chocolate, liquid cream, gelatin, vanilla paste, apple and cinnamon filling, white chocolate glaze, salted caramel topping, freeze-dried apple (110g)', 3.20, 1, 2, 1),
    ('Matcha Strawberry Donut', 'Flour, milk, sugar, egg, butter, yeast, salt, white chocolate, liquid cream, gelatin, matcha powder, white chocolate glaze, freeze-dried strawberry, strawberry puree (110g)', 3.20, 1, 2, 1),
    ('Lotus Biscoff Donut', 'Flour, milk, sugar, egg, butter, yeast, salt, liquid cream, Lotus Biscoff spread, caramel glaze, Lotus Biscoff biscuits (110g)', 3.20, 1, 2, 1),
    
    -- Coffee Drinks
    ('Espresso', 'Ingredients (20/36 ml): Espresso', 2.00, 2, 1, 1),
    ('Cortado', 'Ingredients (100 ml): 1 espresso shot, milk', 2.00, 2, 1, 1),
    ('Long Black', 'Ingredients (200 ml): 2 espresso shots, hot water', 2.50, 2, 1, 1),
    ('Cappuccino', 'Ingredients (200 ml): 1 espresso shot, 180 ml milk', 3.00, 2, 1, 1),
    ('Flat White', 'Ingredients (200 ml): 2 espresso shots, 164 ml milk', 3.10, 2, 1, 1),
    ('Latte', 'Ingredients (240 ml): 2 espresso shots, 220 ml milk', 3.20, 2, 1, 1),
    ('Iced Long Black', 'Ingredients (300 ml): 2 espresso shots, water, ice', 2.50, 2, 1, 1),
    ('Iced Latte', 'Ingredients (400 ml): 2 espresso shots, 220 ml milk, ice', 3.30, 2, 1, 1),
    ('Signature Iced Drink', 'Ingredients (400 ml): 2 espresso shots, 210 ml milk, syrup, topping, ice', 3.80, 2, 2, 1),
    ('Signature Hot Drink', 'Ingredients (280 ml): 2 espresso shots, 210 ml milk, syrup, topping, whipped cream', 3.80, 2, 2, 1),
    
    -- Matcha Drinks (Cold)
    ('Strawberry Matcha Latte', 'Ingredients: matcha powder, water, milk, strawberry puree', 4.50, 3, 2, 1),
    ('Oat Milk Vanilla Matcha', 'Ingredients: matcha powder, oat milk, vanilla syrup', 4.50, 3, 2, 1),
    ('Mango Matcha Latte', 'Matcha powder, water, milk, mango puree, cold foam, freeze-dried mango (Iced only)', 4.50, 3, 2, 1),
    
    -- Lemonades
    ('Strawberry Lemonade', 'Water, lemon juice, strawberry puree, ice', 3.30, 3, 1, 1),
    ('Mango Lemonade', 'Water, lemon juice, mango puree, ice', 3.30, 3, 1, 1),
    
    -- Hot Drinks
    ('Hot Tea', 'Ingredients (280 ml): boiled water, tea sachet', 2.20, 4, 1, 1),
    ('White Hot Chocolate', 'Ingredients (280 ml): Belgian white chocolate, milk, whipped cream, topping', 3.30, 4, 1, 1),
    ('Dark Hot Chocolate', 'Ingredients (280 ml): Belgian milk chocolate, milk, whipped cream, topping', 3.30, 4, 1, 1),
    
    -- Bagels
    ('Arugula Pesto Mozzarella Bagel', 'Bagel bread (water, flour, salt, brown sugar, yeast, oil), pesto sauce, arugula, mozzarella', 4.50, 5, 1, 1),
    ('Raw Prosciutto Bagel', 'Bagel bread (water, flour, salt, brown sugar, yeast, oil), pesto sauce, baby spinach, mozzarella, raw prosciutto', 4.50, 5, 1, 1),
    ('Ham and Salami Bagel', 'Bagel bread (water, flour, salt, brown sugar, yeast, oil), lollo bionda lettuce, cascaval cheese, ham, salami', 4.50, 5, 1, 1);

