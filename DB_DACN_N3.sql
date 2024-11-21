CREATE DATABASE MovieDB;
GO

-- Sử dụng cơ sở dữ liệu vừa tạo
USE MovieDB;
GO

-- Tạo bảng Users
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    Role NVARCHAR(50) NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- Tạo bảng Subscriptions (Gói đăng ký)
CREATE TABLE Subscriptions (
    SubscriptionID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    Duration INT NOT NULL -- Thời gian hiệu lực gói tính bằng ngày
);

-- Tạo bảng Movies
CREATE TABLE Movies (
    MovieID INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(255),
    ReleaseDate DATE,
    Duration INT,
    Director NVARCHAR(100),
    Cast NVARCHAR(500),
    Language NVARCHAR(50),
    AgeRating NVARCHAR(10),
    Poster NVARCHAR(555),
    Trailer NVARCHAR(555),
    HorizontalPoster NVARCHAR(555),
    TheatricalScreening BIT DEFAULT 0,
    ScreeningOrComing BIT DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- Tạo bảng Genres
CREATE TABLE Genres (
    GenreID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL
);

-- Tạo bảng MovieGenres (Liên kết Phim - Thể loại)
CREATE TABLE MovieGenres (
    MovieID INT,
    GenreID INT,
    PRIMARY KEY (MovieID, GenreID),
    FOREIGN KEY (MovieID) REFERENCES Movies(MovieID),
    FOREIGN KEY (GenreID) REFERENCES Genres(GenreID)
);

-- Tạo bảng Reviews (Đánh giá phim)
CREATE TABLE Reviews (
    ReviewID INT PRIMARY KEY IDENTITY(1,1),
    MovieID INT,
    UserID INT,
    Rating INT CHECK (Rating >= 1 AND Rating <= 5),
    Comment NVARCHAR(255),
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MovieID) REFERENCES Movies(MovieID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Tạo bảng Watchlist (Danh sách xem)
CREATE TABLE Watchlist (
    WatchlistID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT,
    MovieID INT,
    AddedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (MovieID) REFERENCES Movies(MovieID)
);

-- Tạo bảng ViewHistory (Lịch sử xem)
CREATE TABLE ViewHistory (
    ViewID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT,
    MovieID INT,
    WatchedDate DATETIME DEFAULT GETDATE(),
    Progress INT CHECK (Progress >= 0 AND Progress <= 100),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (MovieID) REFERENCES Movies(MovieID)
);

-- Tạo bảng UserSubscriptions (Đăng ký của người dùng)
CREATE TABLE UserSubscriptions (
    UserSubscriptionID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT,
    SubscriptionID INT,
    StartDate DATETIME DEFAULT GETDATE(),
    EndDate DATETIME,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (SubscriptionID) REFERENCES Subscriptions(SubscriptionID)
);

-- Tạo bảng Seasons
CREATE TABLE Seasons (
    SeasonID INT PRIMARY KEY IDENTITY(1,1),
    MovieID INT,
    SeasonNumber INT NOT NULL,
    Description NVARCHAR(255),
    ReleaseDate DATE,
    FOREIGN KEY (MovieID) REFERENCES Movies(MovieID)
);

-- Tạo bảng Episodes (Tập phim)
CREATE TABLE Episodes (
    EpisodeID INT PRIMARY KEY IDENTITY(1,1),
    SeasonID INT,
    EpisodeNumber INT NOT NULL,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(255),
    Duration INT,
    ReleaseDate DATE,
    VideoURL NVARCHAR(500),
    FOREIGN KEY (SeasonID) REFERENCES Seasons(SeasonID)
);

-- Tạo bảng Seats
CREATE TABLE Seats (
    SeatID INT PRIMARY KEY IDENTITY(1,1),
    SeatNumber NVARCHAR(10) NOT NULL,  -- Số ghế
    IsVIP BIT DEFAULT 0,               -- Ghế VIP hay không (1: VIP, 0: Thường)
    IsAvailable BIT DEFAULT 1,         -- Trạng thái ghế (1: Trống, 0: Đã đặt)
);

-- Tạo bảng CinemaTickets
CREATE TABLE CinemaTickets (
    TicketID INT PRIMARY KEY IDENTITY(1,1),
    MovieID INT,
    UserID INT,
    ShowTime DATETIME NOT NULL,        -- Thời gian chiếu
    SeatNumber NVARCHAR(10) NOT NULL,  -- Số ghế
    TicketPrice DECIMAL(10, 2) NOT NULL, -- Giá vé
    BookingDate DATETIME DEFAULT GETDATE(), -- Ngày đặt vé
    SeatID INT,                        -- Liên kết tới SeatID
    FOREIGN KEY (MovieID) REFERENCES Movies(MovieID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (SeatID) REFERENCES Seats(SeatID)
);

-- Tạo bảng Advertisements
CREATE TABLE Advertisements (
    AdID INT PRIMARY KEY IDENTITY(1,1),
    ImageURL NVARCHAR(500) NOT NULL, -- Đường dẫn tới hình ảnh
    Description NVARCHAR(255),       -- Mô tả (nếu cần)
    StartDate DATETIME NOT NULL,     -- Ngày bắt đầu hiển thị
    EndDate DATETIME,                -- Ngày kết thúc hiển thị
    IsActive BIT DEFAULT 1           -- Trạng thái quảng cáo (1: đang hiển thị, 0: không hiển thị)
);

-- Thêm dữ liệu mẫu vào bảng Users
INSERT INTO Users (Username, Email, Password, Role)
VALUES 
('user1', 'user1@example.com', 'password1', 'User'),
('admin', 'admin@gmail.com', '123', 'Admin');

---- Thêm dữ liệu mẫu vào bảng Movies
--INSERT INTO Movies (Title, Description, ReleaseDate, Duration, Director, Cast, Language, AgeRating, Poster, Trailer)
--VALUES 
--('Movie 1', 'Description of movie 1', '2023-01-01', 120, 'Director 1', 'Cast 1, Cast 2', 'English', 'PG-13', 'poster1.jpg', 'trailer1.mp4'),
--('Movie 2', 'Description of movie 2', '2023-02-01', 90, 'Director 2', 'Cast 3, Cast 4', 'English', 'R', 'poster2.jpg', 'trailer2.mp4');

---- Thêm dữ liệu mẫu vào bảng Genres
--INSERT INTO Genres (Name)
--VALUES ('Action'), ('Drama'), ('Comedy');

---- Thêm dữ liệu mẫu vào bảng MovieGenres
--INSERT INTO MovieGenres (MovieID, GenreID)
--VALUES (1, 1), (1, 2), (2, 3);

---- Thêm dữ liệu mẫu vào bảng Seasons
--INSERT INTO Seasons (MovieID, SeasonNumber, Description, ReleaseDate)
--VALUES (1, 1, 'Season 1 of Movie 1', '2023-03-01');

---- Thêm dữ liệu mẫu vào bảng Episodes
--INSERT INTO Episodes (SeasonID, EpisodeNumber, Title, Description, Duration, ReleaseDate, VideoURL)
--VALUES (1, 1, 'Episode 1', 'Episode 1 of Season 1', 45, '2023-03-01', 'episode1.mp4'),
--       (1, 2, 'Episode 2', 'Episode 2 of Season 1', 50, '2023-03-08', 'episode2.mp4');


INSERT INTO Movies (Title, Description, ReleaseDate, Duration, Director, Cast, Language, AgeRating, Poster, HorizontalPoster, Trailer)
VALUES 
('One Piece', N'Phiên bản anime của manga nổi tiếng về cướp biển.', '2023-01-01', 90, N'Đạo diễn 1', N'Diễn viên lồng ghép 1, Diễn viên lồng ghép 2', N'Tiếng Nhật', 'PG-13', 'onePiece.jfif','onePieceH.jpeg', 'onepiece_trailer.mp4'),
('Doraemon', N'Một chú mèo robot từ tương lai giúp đỡ một cậu bé.', '2023-02-01', 80, N'Đạo diễn 2', N'Diễn viên lồng ghép 3, Diễn viên lồng ghép 4', N'Tiếng Nhật', 'G', 'doraemon.jpg', 'doraemonH.webp','doraemon_trailer.mp4'),
('Peaky Blinders', N'Câu chuyện về một gia đình gangster ở Birmingham sau Thế chiến I.', '2023-03-01', 60, N'Đạo diễn 3', N'Diễn viên 1, Diễn viên 2', N'Tiếng Anh', 'TV-MA', 'peakyBlinder.png', 'peakyBlinderH.jpg', 'peakyblinders_trailer.mp4');

INSERT INTO Seasons (MovieID, SeasonNumber, Description, ReleaseDate)
VALUES 
(1, 1, N'Mùa 1 của One Piece', '2023-01-01'),  
(2, 1, N'Mùa 1 của Doraemon', '2023-02-01'),  
(3, 1, N'Mùa 1 của Peaky Blinders', '2023-03-01'),
(3, 2, N'Mùa 2 của Peaky Blinders', '2023-04-01'); 

INSERT INTO Episodes (SeasonID, EpisodeNumber, Title, Description, Duration, ReleaseDate, VideoURL)
VALUES 
(1, 1, N'Tập 1', N'Tập 1 của One Piece', 30, '2023-01-01', 'https://1drv.ms/v/c/ea3f7b6210775117/IQQ-xnPtweYVR4L7ZXl1pBM0AQqfJzScQ9lFQQsOtHBZOV4'),
(1, 2, N'Tập 2', N'Tập 2 của One Piece', 30, '2023-01-08', 'https://1drv.ms/v/c/ea3f7b6210775117/IQQrntjGKFoeQpKdrHDgaldLAVyjAkAAOXXEgcFu-EAejsA'),
(1, 3, N'Tập 3', N'Tập 3 của One Piece', 30, '2023-01-15', 'https://1drv.ms/v/c/ea3f7b6210775117/IQR-GM2dvywYTJpThamf_LdDAdmCVjkIIIF1JizwSljxptg'),

(3, 1, N'Tập 1', N'Tập 1 của Mùa 1', 60, '2023-03-01', 'https://1drv.ms/v/c/ea3f7b6210775117/IQTIWLix0uPXQocAtyQfnCdYAd9Cv_yfn2zQua7riI23gSY'),
(4, 1, N'Tập 1', N'Tập 1 của Mùa 2', 60, '2023-04-01', 'https://1drv.ms/v/c/ea3f7b6210775117/IQSDrz6-6iv_QIEEuSR2_HqBAXKu_lsLN9hViYI6muHLvuA'),
(2, 1, N'Tập 1', N'Tập 1 của Doraemon', 40, '2023-02-01', 'https://1drv.ms/v/c/ea3f7b6210775117/IQQeNZdytZxQRrA2o1KXdesiAeiN4Z6oX3jO5aw3JcIJsmQ'),
(2, 2, N'Tập 2', N'Tập 2 của Doraemon', 40, '2023-02-08', 'https://1drv.ms/v/c/ea3f7b6210775117/IQSQaT5brRrLRYc2zvGsTAs-ATU4M-4L7bluKQh9iFoX_lw');

INSERT INTO Genres (Name) VALUES 
('Anime'), --1
('Adventure'), --2
('Fantasy'), --3
('Action'), --4
('Drama'), --5
('Crime'), --6
('Historical'),--7
('Family'),--8
('Comedy'),--9
('Mystery'), --10
('Slice of Life'), --11
('Supernatural'); --12

INSERT INTO MovieGenres (MovieID, GenreID) VALUES
(1, 1),  -- One Piece - Anime
(1, 2),  -- One Piece - Adventure
(1, 3),  -- One Piece - Fantasy
(1, 4),  -- One Piece - Action
(1, 9),  -- One Piece - Comedy
(2, 1),  -- Doraemon - Anime
(2, 3),  -- Doraemon - Family
(2, 9),  -- Doraemon - Comedy
(2, 11),  -- Doraemon - Slice of Life
(2, 12),  -- Doraemon - Supernatural
(3, 5),  -- Peaky Blinders - Drama
(3, 6), -- Peaky Blinders - Crime
(3, 7); -- Peaky Blinders - Historical

-- Thêm dữ liệu mẫu vào bảng Subscriptions
INSERT INTO Subscriptions (Name, Price, Duration)
VALUES (N'gói theo tháng', 9999, 30), ('gói theo năm', 99999, 365);

-- Thêm dữ liệu mẫu vào bảng UserSubscriptions
INSERT INTO UserSubscriptions (UserID, SubscriptionID, StartDate, EndDate)
VALUES (1, 1, GETDATE(), DATEADD(DAY, 30, GETDATE()));


-- Thêm mẫu dữ liệu vào bảng Advertisements
INSERT INTO Advertisements (ImageURL, Description, StartDate, EndDate)
VALUES 
('banner1.jpg', 'Giảm giá đặc biệt cho thành viên', '2024-01-01', '2024-01-31'),
('banner2.jpg', 'Xem phim mới nhất', '2024-02-01', '2024-02-28');

-- Thêm mẫu dữ liệu vào bảng CinemaTickets
--INSERT INTO CinemaTickets (MovieID, UserID, ShowTime, SeatNumber, TicketPrice)
--VALUES 
--(1, 1, '2024-01-15 19:00', 'A10', 75000),
--(2, 2, '2024-02-20 21:00', 'B15', 75000);

---- Thêm dữ liệu mẫu vào bảng ShowTimes
--INSERT INTO ShowTimes (MovieID, ShowDate, StartTime, EndTime, CinemaRoom)
--VALUES 
--(1, '2024-01-15', '19:00', '21:00', 'Phòng 1'),
--(2, '2024-02-20', '21:00', '23:00', 'Phòng 2');

---- Thêm dữ liệu mẫu vào bảng Seats
INSERT INTO Seats (SeatNumber, IsVIP, IsAvailable)
VALUES 
    -- Hàng A
    ('A1', 0, 1), ('A2', 0, 1), ('A3', 0, 1), ('A4', 0, 1), ('A5', 0, 1),
    ('A6', 0, 1), ('A7', 0, 1), ('A8', 0, 1), ('A9', 0, 1), ('A10', 0, 1),

    -- Hàng B
    ('B1', 0, 1), ('B2', 0, 1), ('B3', 0, 1), ('B4', 0, 1), ('B5', 0, 1),
    ('B6', 0, 1), ('B7', 0, 1), ('B8', 0, 1), ('B9', 0, 1), ('B10', 0, 1),

    -- Hàng C
    ('C1', 0, 1), ('C2', 0, 1), ('C3', 0, 1), ('C4', 0, 1), ('C5', 0, 1),
    ('C6', 0, 1), ('C7', 0, 1), ('C8', 0, 1), ('C9', 0, 1), ('C10', 0, 1),

    -- Hàng D
    ('D1', 0, 1), ('D2', 0, 1), ('D3', 0, 1), ('D4', 0, 1), ('D5', 0, 1),
    ('D6', 0, 1), ('D7', 0, 1), ('D8', 0, 1), ('D9', 0, 1), ('D10', 0, 1),

    -- Hàng E (VIP)
    ('E1', 1, 1), ('E2', 1, 1), ('E3', 1, 1), ('E4', 1, 1), ('E5', 1, 1),
    ('E6', 1, 1), ('E7', 1, 1), ('E8', 1, 1), ('E9', 1, 1), ('E10', 1, 1),

    -- Hàng F (VIP)
    ('F1', 1, 1), ('F2', 1, 1), ('F3', 1, 1), ('F4', 1, 1), ('F5', 1, 1),
    ('F6', 1, 1), ('F7', 1, 1), ('F8', 1, 1), ('F9', 1, 1), ('F10', 1, 1),

    -- Hàng G
    ('G1', 0, 1), ('G2', 0, 1), ('G3', 0, 1), ('G4', 0, 1), ('G5', 0, 1),
    ('G6', 0, 1), ('G7', 0, 1), ('G8', 0, 1), ('G9', 0, 1), ('G10', 0, 1),

    -- Hàng H
    ('H1', 0, 1), ('H2', 0, 1), ('H3', 0, 1), ('H4', 0, 1), ('H5', 0, 1),
    ('H6', 0, 1), ('H7', 0, 1), ('H8', 0, 1), ('H9', 0, 1), ('H10', 0, 1),

    -- Hàng I
    ('I1', 0, 1), ('I2', 0, 1), ('I3', 0, 1), ('I4', 0, 1), ('I5', 0, 1),
    ('I6', 0, 1), ('I7', 0, 1), ('I8', 0, 1), ('I9', 0, 1), ('I10', 0, 1),

    -- Hàng J
    ('J1', 0, 1), ('J2', 0, 1), ('J3', 0, 1), ('J4', 0, 1), ('J5', 0, 1),
    ('J6', 0, 1), ('J7', 0, 1), ('J8', 0, 1), ('J9', 0, 1), ('J10', 0, 1);


