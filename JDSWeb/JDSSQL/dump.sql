DROP TABLE IF EXISTS ClothType;
DROP TABLE IF EXISTS ClothSize;
DROP TABLE IF EXISTS ClothColor;
DROP TABLE IF EXISTS Cloth;
DROP TABLE IF EXISTS Role;
DROP TABLE IF EXISTS [User];
DROP TABLE IF EXISTS Event;
DROP TABLE IF EXISTS Image;
DROP TABLE IF EXISTS ShopGallery;
DROP TABLE IF EXISTS EventGallery;

CREATE TABLE Role (
	Id INT PRIMARY KEY NOT NULL,
	Name VARCHAR(20) NOT NULL
);

CREATE TABLE [User] (
	Id INT IDENTITY(1,1) NOT NULL,
	Role INT NOT NULL,
	Username VARCHAR(50) NOT NULL,
	Email VARCHAR(50) NOT NULL,
	Password VARCHAR(100) NOT NULL,
	Newsletter BIT NOT NULL,
	PRIMARY KEY (Id),
	FOREIGN KEY (Role)
		REFERENCES Role(Id)
		ON UPDATE CASCADE
		ON DELETE CASCADE
);

CREATE TABLE ClothColor (
	Id INT PRIMARY KEY NOT NULL,
	Name VARCHAR(50) NOT NULL,
	Hexa VARCHAR(7) NOT NULL
);

CREATE TABLE ClothType (
	Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	Name VARCHAR(50) NOT NULL,
	Color INT NOT NULL,
	Description VARCHAR(100) NOT NULL,
	Price DECIMAL(6,2) NOT NULL,
	FOREIGN KEY (Color)
	REFERENCES ClothColor(Id)
		ON UPDATE CASCADE
		ON DELETE CASCADE
);

CREATE TABLE ClothSize (
	Id INT PRIMARY KEY NOT NULL,
	Name VARCHAR(50) NOT NULL,
	Shortcut VARCHAR(10) NOT NULL
);

CREATE TABLE Cloth (
	Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	Type INT NOT NULL,
	Size INT,
	Booked INT,
	FOREIGN KEY (Type)
		REFERENCES ClothType(Id)
		ON UPDATE CASCADE
		ON DELETE CASCADE,
	FOREIGN KEY (Size)
		REFERENCES ClothSize(Id)
		ON UPDATE CASCADE
		ON DELETE CASCADE,
	FOREIGN KEY (Booked)
		REFERENCES [User](Id)
		ON UPDATE CASCADE
		ON DELETE CASCADE
);

CREATE TABLE Event (
	Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	Date DATETIME NOT NULL,
	Title VARCHAR(100) NOT NULL,
	Description VARCHAR(500) NOT NULL,
	Public BIT NOT NULL
);

CREATE TABLE Image (
	Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	Url VARCHAR(100) NOT NULL,
	Alt VARCHAR(50) NOT NULL
);

CREATE TABLE ShopGallery (
	ClothTypeId INT NOT NULL,
	ImageId INT NOT NULL,
	FOREIGN KEY (ClothTypeId)
		REFERENCES ClothType(Id)
		ON UPDATE CASCADE
		ON DELETE CASCADE,
	FOREIGN KEY (ImageId)
		REFERENCES Image(Id)
		ON UPDATE CASCADE
		ON DELETE CASCADE,
	PRIMARY KEY (ClothTypeId, ImageId)
);

CREATE TABLE EventGallery (
	EventId INT NOT NULL,
	ImageId INT NOT NULL,
	FOREIGN KEY (EventId)
		REFERENCES Event(Id)
		ON UPDATE CASCADE
		ON DELETE CASCADE,
	FOREIGN KEY (ImageId)
		REFERENCES Image(Id)
		ON UPDATE CASCADE
		ON DELETE CASCADE,
	PRIMARY KEY (EventId, ImageId)
);

INSERT INTO ClothColor
VALUES (0, 'Jaune', '#F7C740'), (1, 'Bleu', '#0000FF'), (2, 'Noir', '#000000');

INSERT INTO ClothType (Name, Color, Description, Price)
VALUES ('T-Shirt', 0, 'T-Shirt jaune de la JDS', 35.0), ('T-Shirt', 1, 'T-Shirt bleu de la JDS', 35.0),
		('Bonnet', 2, 'Bonnet de la JDS', 20), 
		('Casquette', 2, 'Casquette de la JDS', 30), 
		('Pull', 0, 'Pull jaune de la JDS', 45), ('Pull', 1, 'Pull bleu de la JDS', 45);
		
INSERT INTO ClothSize
VALUES (0, 'extra-small', 'XS'), (1, 'small', 'S'), (2, 'medium', 'M'),
		(3, 'large', 'L'), (4, 'extra-large', 'XL'), (5, 'extra-extra-large', 'XXL');

INSERT INTO Cloth (Type, Size)
VALUES (1, 0), (1, 1), (1, 2), (1, 3),(1, 4), (1, 5),
		(2, NULL), (3, NULL),
		(6, 1), (6, 4);
		
		
INSERT INTO Role
VALUES (0, 'Member'), (1, 'Manager'), (2, 'Admin');

INSERT INTO [User] (Role, Username, Email, Password, Newsletter)
VALUES (2, 'dev.vinz', 'vincent@jeannin.ch', 'a0f3285b07c26c0dcd2191447f391170d06035e8d57e31a048ba87074f3a9a15', 0);

