/* =====================================================================
   Maharaja Travel's - base schema + reference data (cities, buses, routes, schedules, boarding points)
   Run this FIRST on a fresh SQL Server, then 02_Upgrade_Security_Payments.sql.
   All personal data (sample customers, bookings, feedback, card rows) and the
   old plain-text admin passwords that shipped with the original project have been removed.
   ===================================================================== */
create DATABASE [OnlineBusBooking]
GO
USE [OnlineBusBooking]
GO
/****** Object:  Table [dbo].[Admin_Login]    Script Date: 09-03-2024 14:57:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Admin_Login](
	[Admin_ID] [nvarchar](50) NULL,
	[UserName] [nvarchar](50) NULL,
	[Password] [nvarchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BookingMaster]    Script Date: 09-03-2024 14:57:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingMaster](
	[BookingId] [int] IDENTITY(1,1) NOT NULL,
	[RegId] [int] NULL,
	[BusId] [int] NULL,
	[Fname] [varchar](50) NULL,
	[Lname] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[Contact] [varchar](50) NULL,
	[City] [varchar](50) NULL,
	[SeatNo] [nvarchar](50) NULL,
	[TravelDate] [varchar](50) NULL,
	[Origin] [varchar](50) NULL,
	[Destination] [varchar](50) NULL,
	[BoardingID] [int] NULL,
	[Fare] [decimal](18, 2) NULL,
	[PNRNo] [varchar](50) NULL,
	[ScheduleID] [int] NULL,
 CONSTRAINT [PK_BookingMaster] PRIMARY KEY CLUSTERED 
(
	[BookingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BusMaster]    Script Date: 09-03-2024 14:57:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BusMaster](
	[BusId] [int] IDENTITY(1,1) NOT NULL,
	[BusNo] [varchar](50) NULL,
	[BustType] [varchar](50) NULL,
	[TotalSeat] [int] NULL,
	[SeatColumn] [int] NULL,
	[SeatRow] [int] NULL,
	[BookedSeat] [int] NULL,
	[AvailableSeats] [int] NULL,
	[BusName] [varchar](50) NULL,
 CONSTRAINT [PK_BusMaster] PRIMARY KEY CLUSTERED 
(
	[BusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CityDetails]    Script Date: 09-03-2024 14:57:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CityDetails](
	[CityID] [bigint] IDENTITY(1,1) NOT NULL,
	[CityName] [varchar](50) NULL,
 CONSTRAINT [PK_CityDetails] PRIMARY KEY CLUSTERED 
(
	[CityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Feedback]    Script Date: 09-03-2024 14:57:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feedback](
	[FeedbackID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](100) NULL,
	[Email] [nvarchar](100) NULL,
	[ContactNo] [nvarchar](20) NULL,
	[Rating] [int] NULL,
	[FeedbackText] [nvarchar](max) NULL,
	[ComplaintOrSuggestion] [nvarchar](max) NULL,
	[FeedbackDate] [datetime] NULL,
 CONSTRAINT [PK__Feedback__6A4BEDF622AA2996] PRIMARY KEY CLUSTERED 
(
	[FeedbackID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PickUpStand]    Script Date: 09-03-2024 14:57:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PickUpStand](
	[StandId] [int] IDENTITY(1,1) NOT NULL,
	[RouteId] [int] NULL,
	[PlaceName] [varchar](50) NULL,
	[PlaceTime] [varchar](50) NULL,
	[BusID] [int] NULL,
 CONSTRAINT [PK_PickUpStand] PRIMARY KEY CLUSTERED 
(
	[StandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PNRDetails]    Script Date: 09-03-2024 14:57:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PNRDetails](
	[PNRDetailsID] [bigint] IDENTITY(1,1) NOT NULL,
	[PNRNo] [varchar](50) NULL,
	[TotalAmount] [decimal](18, 2) NULL,
	[TotalTickets] [int] NULL,
	[CreatedBy] [int] NULL,
 CONSTRAINT [PK_PNRDetails] PRIMARY KEY CLUSTERED 
(
	[PNRDetailsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Registration]    Script Date: 09-03-2024 14:57:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Registration](
	[regId] [int] IDENTITY(1,1) NOT NULL,
	[Fname] [varchar](50) NULL,
	[Lname] [varchar](50) NULL,
	[EmailId] [varchar](50) NULL,
	[Address] [varchar](200) NULL,
	[City] [varchar](50) NULL,
	[Picode] [varchar](15) NULL,
	[Contact] [varchar](50) NULL,
	[Password] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_Registration] PRIMARY KEY CLUSTERED 
(
	[regId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RouteDetails]    Script Date: 09-03-2024 14:57:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RouteDetails](
	[RouteID] [int] IDENTITY(1,1) NOT NULL,
	[Origin] [varchar](50) NULL,
	[Destination] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[BusID] [int] NULL,
 CONSTRAINT [PK_RouteDetails] PRIMARY KEY CLUSTERED 
(
	[RouteID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScheduleMaster]    Script Date: 09-03-2024 14:57:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScheduleMaster](
	[ScheduleId] [int] IDENTITY(1,1) NOT NULL,
	[BusId] [int] NULL,
	[Date] [varchar](50) NULL,
	[Fare] [decimal](18, 2) NULL,
	[EstimatedTime] [varchar](50) NULL,
	[ArivalTime] [varchar](50) NULL,
	[DepartureTime] [varchar](50) NULL,
	[RouteID] [int] NULL,
	[BookedSeats] [int] NULL,
	[AvailableSeats] [int] NULL
) ON [PRIMARY]
GO
GO

GO
SET IDENTITY_INSERT [dbo].[BusMaster] ON 

INSERT [dbo].[BusMaster] ([BusId], [BusNo], [BustType], [TotalSeat], [SeatColumn], [SeatRow], [BookedSeat], [AvailableSeats], [BusName]) VALUES (1, N'MH 50 1213', N'AC', 35, 7, 5, 0, 35, N'mumbai exp')
INSERT [dbo].[BusMaster] ([BusId], [BusNo], [BustType], [TotalSeat], [SeatColumn], [SeatRow], [BookedSeat], [AvailableSeats], [BusName]) VALUES (2, N'MH 50 J 4305', N'Normal', 35, 7, 5, 0, 35, N'S.K Travels')
INSERT [dbo].[BusMaster] ([BusId], [BusNo], [BustType], [TotalSeat], [SeatColumn], [SeatRow], [BookedSeat], [AvailableSeats], [BusName]) VALUES (3, N'MH 50 AQ 2999', N'Normal', 35, 7, 5, 0, 35, N'mumbai exp')
INSERT [dbo].[BusMaster] ([BusId], [BusNo], [BustType], [TotalSeat], [SeatColumn], [SeatRow], [BookedSeat], [AvailableSeats], [BusName]) VALUES (4, N'MH 11 A  1122', N'AC', 35, 7, 5, 0, 35, N'mumbai exp')
INSERT [dbo].[BusMaster] ([BusId], [BusNo], [BustType], [TotalSeat], [SeatColumn], [SeatRow], [BookedSeat], [AvailableSeats], [BusName]) VALUES (5, N'MH 11 SE 9639', N'AC', 35, 7, 5, 0, 35, N'Maharaja Goa Express')
INSERT [dbo].[BusMaster] ([BusId], [BusNo], [BustType], [TotalSeat], [SeatColumn], [SeatRow], [BookedSeat], [AvailableSeats], [BusName]) VALUES (6, N'MH 50 BS 2211', N'Normal', 35, 7, 5, 0, 35, N'Maharaja ahmedabad Express')
INSERT [dbo].[BusMaster] ([BusId], [BusNo], [BustType], [TotalSeat], [SeatColumn], [SeatRow], [BookedSeat], [AvailableSeats], [BusName]) VALUES (7, N'MH 50 1213', N'AC', 35, 7, 5, 0, 35, N'Maharaja Rajkot Express')
SET IDENTITY_INSERT [dbo].[BusMaster] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[CityDetails] ON 

INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (1, N'Karad')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (2, N'Mumbai')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (3, N'Kolhapur')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (4, N'Satara')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (5, N'Sangli')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (6, N'Kolad')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (7, N'Mahad')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (8, N'Panvel')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (9, N'Vashi')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (10, N'Chembur')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (11, N'Dadar')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (12, N'Thane')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (13, N'Mumbai Central')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (14, N'Bandra')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (15, N'Andheri')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (16, N'Borivali')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (17, N'Malad')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (18, N'Kandivali')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (19, N'Goregaon')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (20, N'Jogeshwari')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (21, N'Vile Parle')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (22, N'Sion')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (23, N'Mulund')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (24, N'Powai')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (25, N'Nerul')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (26, N'Belapur')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (27, N'Kharghar')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (28, N'Kamothe')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (29, N'Kalamboli')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (30, N'Ulhasnagar')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (31, N'Pune')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (32, N'Lonavala')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (33, N'Khopoli')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (34, N'Santacruz')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (35, N'Mahim')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (36, N'Matunga')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (37, N'Lower Parel')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (38, N'Mahalaxmi')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (39, N'Ghatkopar')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (40, N'Vikhroli')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (41, N'Kanjurmarg')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (42, N'Bhandup')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (43, N'Kalwa')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (44, N'Mumbra')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (45, N'Diva')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (46, N'Dombivli')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (47, N'Thakurli')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (48, N'Kalyan')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (49, N'Vithalwadi')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (50, N'Ambernath')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (51, N'Badlapur')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (52, N'Vangani')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (53, N'Shelu')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (54, N'Neral')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (55, N'Bhivpuri Road')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (56, N'Karjat')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (57, N'Palasdhari')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (58, N'Kelavli')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (59, N'Dolavli')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (60, N'Lowjee')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (61, N'Pen')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (62, N'Vadkhal')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (63, N'Rasayani')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (64, N'Apta')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (65, N'Jite')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (66, N'Seawoods–Darave')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (67, N'CBD Belapur')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (68, N'Mansarovar')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (69, N'Khandeshwar')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (70, N'Navi Mumbai')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (71, N'Goa')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (72, N'Ahmedabad')
INSERT [dbo].[CityDetails] ([CityID], [CityName]) VALUES (73, N'Rajkot')
SET IDENTITY_INSERT [dbo].[CityDetails] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[PickUpStand] ON 

INSERT [dbo].[PickUpStand] ([StandId], [RouteId], [PlaceName], [PlaceTime], [BusID]) VALUES (1, 1, N'Karad Bus Stand ', N'06:30 PM', 1)
INSERT [dbo].[PickUpStand] ([StandId], [RouteId], [PlaceName], [PlaceTime], [BusID]) VALUES (2, 2, N'Satara Bus Stand ', N'08:30 PM', 2)
INSERT [dbo].[PickUpStand] ([StandId], [RouteId], [PlaceName], [PlaceTime], [BusID]) VALUES (3, 1, N'Karad Bus Stand ', N'11:37 PM', 1)
INSERT [dbo].[PickUpStand] ([StandId], [RouteId], [PlaceName], [PlaceTime], [BusID]) VALUES (4, 3, N'kolhapur bus stand', N'08:58 PM', 3)
INSERT [dbo].[PickUpStand] ([StandId], [RouteId], [PlaceName], [PlaceTime], [BusID]) VALUES (5, 7, N'Mumbai bus stand', N'09:03 PM', 7)
INSERT [dbo].[PickUpStand] ([StandId], [RouteId], [PlaceName], [PlaceTime], [BusID]) VALUES (6, 6, N'Mumbai bus stand', N'09:04 PM', 6)
INSERT [dbo].[PickUpStand] ([StandId], [RouteId], [PlaceName], [PlaceTime], [BusID]) VALUES (7, 5, N'Mumbai bus stand', N'10:04 PM', 5)
INSERT [dbo].[PickUpStand] ([StandId], [RouteId], [PlaceName], [PlaceTime], [BusID]) VALUES (8, 4, N'Pune Bus Stand', N'10:04 PM', 4)
INSERT [dbo].[PickUpStand] ([StandId], [RouteId], [PlaceName], [PlaceTime], [BusID]) VALUES (9, 3, N'kolhapur bus stand', N'11:05 PM', 3)
INSERT [dbo].[PickUpStand] ([StandId], [RouteId], [PlaceName], [PlaceTime], [BusID]) VALUES (10, 6, N'Mumbai bus stand', N'09:14 PM', 6)
SET IDENTITY_INSERT [dbo].[PickUpStand] OFF
GO

GO

GO
SET IDENTITY_INSERT [dbo].[RouteDetails] ON 

INSERT [dbo].[RouteDetails] ([RouteID], [Origin], [Destination], [CreatedDate], [BusID]) VALUES (1, N'Karad', N'Mumbai', CAST(N'2024-02-26T11:27:31.553' AS DateTime), 1)
INSERT [dbo].[RouteDetails] ([RouteID], [Origin], [Destination], [CreatedDate], [BusID]) VALUES (2, N'Karad', N'Pune', CAST(N'2024-02-26T11:28:57.263' AS DateTime), 2)
INSERT [dbo].[RouteDetails] ([RouteID], [Origin], [Destination], [CreatedDate], [BusID]) VALUES (3, N'Kolhapur', N'Mumbai', CAST(N'2024-02-29T16:57:36.933' AS DateTime), 3)
INSERT [dbo].[RouteDetails] ([RouteID], [Origin], [Destination], [CreatedDate], [BusID]) VALUES (4, N'Pune', N'Mumbai', CAST(N'2024-02-29T16:58:57.217' AS DateTime), 4)
INSERT [dbo].[RouteDetails] ([RouteID], [Origin], [Destination], [CreatedDate], [BusID]) VALUES (5, N'Mumbai ', N'Goa', CAST(N'2024-02-29T16:59:29.717' AS DateTime), 5)
INSERT [dbo].[RouteDetails] ([RouteID], [Origin], [Destination], [CreatedDate], [BusID]) VALUES (6, N'Mumbai ', N'Ahmedabad', CAST(N'2024-02-29T17:00:45.947' AS DateTime), 6)
INSERT [dbo].[RouteDetails] ([RouteID], [Origin], [Destination], [CreatedDate], [BusID]) VALUES (7, N'Mumbai', N'Rajkot', CAST(N'2024-02-29T17:01:31.770' AS DateTime), 7)
SET IDENTITY_INSERT [dbo].[RouteDetails] OFF
GO
SET IDENTITY_INSERT [dbo].[ScheduleMaster] ON 

INSERT [dbo].[ScheduleMaster] ([ScheduleId], [BusId], [Date], [Fare], [EstimatedTime], [ArivalTime], [DepartureTime], [RouteID], [BookedSeats], [AvailableSeats]) VALUES (1, 1, N'31-03-2024', CAST(400.00 AS Decimal(18, 2)), N'3.00hr', N'06:29 PM', N'07:29 PM', 1, 0, 35)
INSERT [dbo].[ScheduleMaster] ([ScheduleId], [BusId], [Date], [Fare], [EstimatedTime], [ArivalTime], [DepartureTime], [RouteID], [BookedSeats], [AvailableSeats]) VALUES (2, 2, N'31-03-2024', CAST(1000.00 AS Decimal(18, 2)), N'3.00hr', N'07:30 PM', N'08:30 PM', 2, 1, 34)
INSERT [dbo].[ScheduleMaster] ([ScheduleId], [BusId], [Date], [Fare], [EstimatedTime], [ArivalTime], [DepartureTime], [RouteID], [BookedSeats], [AvailableSeats]) VALUES (3, 1, N'31-03-2024', CAST(1000.00 AS Decimal(18, 2)), N'8.00hr', N'08:36 PM', N'09:37 PM', 1, 8, 27)
INSERT [dbo].[ScheduleMaster] ([ScheduleId], [BusId], [Date], [Fare], [EstimatedTime], [ArivalTime], [DepartureTime], [RouteID], [BookedSeats], [AvailableSeats]) VALUES (4, 3, N'31-03-2024', CAST(1000.00 AS Decimal(18, 2)), N'8.00hr', N'08:57 PM', N'09:57 PM', 3, 0, 35)
INSERT [dbo].[ScheduleMaster] ([ScheduleId], [BusId], [Date], [Fare], [EstimatedTime], [ArivalTime], [DepartureTime], [RouteID], [BookedSeats], [AvailableSeats]) VALUES (5, 6, N'31-03-2024', CAST(1500.00 AS Decimal(18, 2)), N'13hr30min', N'08:01 PM', N'09:02 PM', 6, 0, 35)
INSERT [dbo].[ScheduleMaster] ([ScheduleId], [BusId], [Date], [Fare], [EstimatedTime], [ArivalTime], [DepartureTime], [RouteID], [BookedSeats], [AvailableSeats]) VALUES (6, 5, N'31-03-2024', CAST(1000.00 AS Decimal(18, 2)), N'8.00hr', N'10:02 PM', N'11:02 PM', 5, 2, 33)
INSERT [dbo].[ScheduleMaster] ([ScheduleId], [BusId], [Date], [Fare], [EstimatedTime], [ArivalTime], [DepartureTime], [RouteID], [BookedSeats], [AvailableSeats]) VALUES (7, 6, N'31-03-2024', CAST(1000.00 AS Decimal(18, 2)), N'8.00hr', N'09:14 PM', N'10:14 PM', 6, 9, 26)
SET IDENTITY_INSERT [dbo].[ScheduleMaster] OFF
GO
ALTER TABLE [dbo].[Feedback] ADD  CONSTRAINT [DF__Feedback__Feedba__24927208]  DEFAULT (getdate()) FOR [FeedbackDate]
GO
/****** Object:  StoredProcedure [dbo].[addBordingDetails]    Script Date: 09-03-2024 14:57:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[addBordingDetails](
@RouteID int,
@PlaceName varchar(50),
@PlaceTime varchar(50),
@BusID int
)
as
set nocount on
begin
insert into dbo.PickUpStand
(RouteId,PlaceName,PlaceTime,BusID)
values(@RouteID,@PlaceName,@PlaceTime,@BusID)
end
GO
/****** Object:  StoredProcedure [dbo].[GetPassengerDetails]    Script Date: 09-03-2024 14:57:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetPassengerDetails]( --GetPassengerDetails '48TF1G'
@PNRNo varchar(50)
)
as
set nocount on
begin
select  BM.Fname,BM.Lname,BM.Contact,BM.SeatNo,BM.TravelDate,BM.Origin,BM.Destination,PS.PlaceName
from dbo.BookingMaster BM
 inner join dbo.PickUpStand PS on
 PS.StandId=BM.BoardingID and  BM.PNRNo=@PNRNo
 declare @BusID int
 select @BusID=(BusId) from dbo.BookingMaster where PNRNo=@PNRNo
 declare @BusName varchar(50)
 declare @DeptTime varchar(50)
 declare @PlaceName varchar(50)
 declare @TotalAmount decimal(18,2)
 declare @TotalTickets int;
 select @PlaceName=PlaceName from dbo.PickUpStand where BusID=@BusID
 select @BusName=BusName from dbo.BusMaster where BusId=@BusID
 select @PNRNO=PNRNo from dbo.BookingMaster where PNRNo=@PNRNo
 select @DeptTime=PlaceTime from dbo.PickUpStand where PlaceName=@PlaceName
 select @TotalAmount=TotalAmount,@TotalTickets=TotalTickets from dbo.PNRDetails where PNRNo=@PNRNo
 select @BusName as BusName,@PNRNO as PNRNo,@DeptTime as DeptTime,@TotalAmount as Amount,@TotalTickets as TotalTickets
end
GO
/****** Object:  StoredProcedure [dbo].[ispAddBusDetails]    Script Date: 09-03-2024 14:57:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[ispAddBusDetails](
@BusNo varchar(50),
@BustType varchar(50),
@SeatColumn int,
@SeatRow int,
@Origin varchar(50),
@Destination varchar(50),
@BusName varchar(50)	
)
as
set nocount on
begin
declare @Ret_Val int;
insert into dbo.BusMaster(BusNo,BustType,TotalSeat,SeatColumn,SeatRow,BookedSeat,AvailableSeats,BusName)
values(@BusNo,@BustType,(@SeatRow*@SeatColumn),@SeatColumn,@SeatRow,0,(@SeatRow*@SeatColumn),@BusName)
set @Ret_Val=@@IDENTITY;
if(@@ROWCOUNT>0)
  begin
   insert into dbo.RouteDetails(Origin,Destination,CreatedDate,BusID) values(@Origin,@Destination,GETDATE(),@Ret_Val)
  end
if not exists (select CityName from dbo.CityDetails where CityName=@Origin)
 begin
 insert into dbo.CityDetails (CityName) values(@Origin)
 end 
 if not exists (select CityName from dbo.CityDetails where CityName=@Destination)
 begin
 insert into dbo.CityDetails (CityName) values(@Destination)
 end 
end
GO
/****** Object:  StoredProcedure [dbo].[ispAddBusSchedule]    Script Date: 09-03-2024 14:57:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[ispAddBusSchedule]( --ispGetBusDataByBusID 1
@Date varchar(50) ,
@BusID int,
@Fare decimal(18,2),
@EstimatdTime varchar(50),
@ArrivalTime varchar(50),
@DepartureTime varchar(50),
@RouteID int
)	
as
set nocount on
begin
declare @AvailableSeats int;
select @AvailableSeats=AvailableSeats from dbo.BusMaster where BusId=@BusID 
insert into dbo.ScheduleMaster
(Date,Fare,EstimatedTime,ArivalTime,DepartureTime,RouteID,BusId,AvailableSeats,BookedSeats)
values(@Date,@Fare,@EstimatdTime,@ArrivalTime,@DepartureTime,@RouteID,@BusID,@AvailableSeats,0)
end
GO
/****** Object:  StoredProcedure [dbo].[ispAddPassengerDetails]    Script Date: 09-03-2024 14:57:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[ispAddPassengerDetails](
@RegId int,
@BusId int,
@Fname varchar(50),
@Lname varchar(50),
@Email varchar(50),
@Contact varchar(50),
@City varchar(50),
@SeatNo nvarchar(50),
@TravelDate varchar(50),
@Origin varchar(50),
@Destination varchar(50),
@BoardingId int,
@Fare decimal(18,2),
@TotalSeats int,
@PNRNo varchar(50)
)
as
set nocount on
begin
declare @BookedSeat int;
declare @AvailableSeats int;
declare @scheduleID int;
select @scheduleID=ScheduleId from dbo.ScheduleMaster where Date=@TravelDate
insert into dbo.BookingMaster
(RegId,BusId,Fname,Lname,Email,Contact,City,SeatNo,TravelDate,Origin,Destination,BoardingID,Fare,PNRNo,ScheduleID)
values(@RegId,@BusId,@Fname,@Lname,@Email,@Contact,@City,@SeatNo,@TravelDate,@Origin,@Destination,@BoardingId,@Fare,@PNRNo,@scheduleID)
select @BookedSeat=BookedSeats from dbo.ScheduleMaster where ScheduleId=@scheduleID
select @AvailableSeats=AvailableSeats from dbo.ScheduleMaster where ScheduleId=@scheduleID
update dbo.ScheduleMaster set BookedSeats=(@BookedSeat+1) where ScheduleId=@scheduleID
update dbo.ScheduleMaster set AvailableSeats=(@AvailableSeats-1) where ScheduleId=@scheduleID
end
GO
/****** Object:  StoredProcedure [dbo].[ispAddPNRDetails]    Script Date: 09-03-2024 14:57:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[ispAddPNRDetails](
@PNRNo varchar(50),
@TotalAmount decimal(18,2),
@TotalTicket int,
@CreatedBy int
)
as
set nocount on
begin
insert into dbo.PNRDetails 
(PNRNo,TotalAmount,TotalTickets,CreatedBy)
values(@PNRNo,@TotalAmount,@TotalTicket,@CreatedBy)
end
GO
/****** Object:  StoredProcedure [dbo].[ispGetAvailableBusDetails]    Script Date: 09-03-2024 14:57:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[ispGetAvailableBusDetails]( --ispGetAvailableBusDetails 'Mumbai','Pune','03/25/2017'
@Origin varchar(50),
@Destination varchar(50),
@TravelDate varchar(50)
)
as
set nocount on
begin
declare @scheduleid int;
select @scheduleid=ScheduleId from dbo.ScheduleMaster where Date=@TravelDate
select BM.BusId,BM.BusName,BM.BusNo,BM.SeatColumn,BM.SeatRow,BM.BustType,BM.TotalSeat,SM.AvailableSeats,SM.Fare,SM.EstimatedTime,SM.DepartureTime,SM.ArivalTime
from dbo.BusMaster BM
left join dbo.ScheduleMaster SM
on SM.BusId=BM.BusId
left join dbo.RouteDetails RD
on RD.RouteID=SM.RouteID
where SM.Date=@TravelDate and RD.Origin=@Origin and RD.Destination=@Destination and SM.ScheduleId=@scheduleid
end
GO
/****** Object:  StoredProcedure [dbo].[ispGetBoardingPoints]    Script Date: 09-03-2024 14:57:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[ispGetBoardingPoints](
@BusID int
)
as
set nocount on
begin
select * from dbo.PickUpStand where BusID=@BusID
end
GO
/****** Object:  StoredProcedure [dbo].[ispGetBookedSeatNo]    Script Date: 09-03-2024 14:57:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[ispGetBookedSeatNo]( -- ispGetBookedSeatNo 1
@BusID int,
@TravelDate varchar(50)
)
as
set nocount on
begin
declare @scheduleId int;
select @scheduleId=ScheduleId from dbo.ScheduleMaster where BusId=@BusID and Date=@TravelDate
select isnull(SeatNo,0) as SeatNo from dbo.BookingMaster
where ScheduleId=@scheduleId
end
GO
/****** Object:  StoredProcedure [dbo].[ispGetBookingReportByAdmin]    Script Date: 09-03-2024 14:57:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[ispGetBookingReportByAdmin]
as
set nocount on
begin
select BM.Fname+''+BM.Lname as PaxName,BM.Email,BM.Contact,BM.SeatNo,Convert(varchar(11),CONVERT(SMALLDATETIME, BM.TravelDate, 103),13) as TravelDate,BM.Origin,BM.Destination,
Fare,UM.Fname+''+UM.Lname as BookedBy,BB.BusName From dbo.BookingMaster BM
left join dbo.BusMaster BB on  BM.BusId=BB.BusId 
left join dbo.Registration UM on  UM.regId=BM.RegId
end
GO
/****** Object:  StoredProcedure [dbo].[ispGetBusDataByBusID]    Script Date: 09-03-2024 14:57:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[ispGetBusDataByBusID]( --ispGetBusDataByBusID 1
@BusID int
)	
as
set nocount on
begin
select BM.BusId,BM.BusNo,BM.BustType,BM.SeatColumn,BM.BusName,BM.SeatRow,RD.Origin,RD.Destination,RD.RouteID
from dbo.BusMaster BM 
left join dbo.RouteDetails RD
on BM.BusId=RD.BusID
where BM.BusId=@BusID
end
GO
/****** Object:  StoredProcedure [dbo].[ispGetBusDetailsForUpdation]    Script Date: 09-03-2024 14:57:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[ispGetBusDetailsForUpdation]
as
set nocount on
begin
select BM.BusId,BM.BusNo,BM.BusName,BM.BustType,BM.SeatColumn,BM.SeatRow,RD.Origin,RD.Destination,RD.RouteID
from dbo.BusMaster BM 
left join dbo.RouteDetails RD
on BM.BusId=RD.BusID
end
GO
/****** Object:  StoredProcedure [dbo].[ispGetCity]    Script Date: 09-03-2024 14:57:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[ispGetCity]
as
set nocount on
begin
select * from dbo.CityDetails
end
GO
/****** Object:  StoredProcedure [dbo].[ispGetPNRDetails]    Script Date: 09-03-2024 14:57:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[ispGetPNRDetails](
@UserID int
)
as
set nocount on
begin
select * from dbo.PNRDetails where CreatedBy=@UserID
end
GO
/****** Object:  StoredProcedure [dbo].[ispGetRouteDetails]    Script Date: 09-03-2024 14:57:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[ispGetRouteDetails]
as
set nocount on
begin
select * from dbo.RouteDetails
end
GO
/****** Object:  StoredProcedure [dbo].[ispUpdateBusData]    Script Date: 09-03-2024 14:57:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[ispUpdateBusData]( --ispGetBusDataByBusID 1
@BusID int,
@BusNo varchar(50),
@BusType varchar(50),
@seatColumn int,
@SeatRow int,
@BusName varchar(50),
@Origin varchar(50),
@Destination varchar(50)
)	
as
set nocount on
begin
update dbo.BusMaster set 
BusNo=@BusNo,BustType=@BusType,SeatColumn=@seatColumn,SeatRow=@SeatRow,BusName=@BusName
where BusId=@BusID
update dbo.RouteDetails set 
Origin=@Origin,Destination=@Destination where BusID=@BusID
end
GO
/****** Object:  StoredProcedure [dbo].[ispUserLogin]    Script Date: 09-03-2024 14:57:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ispUserLogin]
    @Identifier varchar(50),
    @Password varchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if the identifier is in email format
    IF CHARINDEX('@', @Identifier) > 0 
    BEGIN
        -- If in email format, select by EmailId
        SELECT * FROM dbo.Registration WHERE EmailId = @Identifier AND Password = @Password;
    END
    ELSE
    BEGIN
        -- If not in email format, select by MobileNo
        SELECT * FROM dbo.Registration WHERE Contact = @Identifier AND Password = @Password;
    END
END
GO
/****** Object:  StoredProcedure [dbo].[ispUserRegistration]    Script Date: 09-03-2024 14:57:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[ispUserRegistration](  
@FName varchar(50)='',  
@LName varchar(50),  
@EmailId varchar(50),  
@Address varchar(50),  
@City varchar(50),  
@PinCode varchar(15),  
@ContactNo varchar(50),  
@Password varchar(50),  
@Ret_Val bigint output  
)  
as  
set nocount on  
begin  
if exists(select Contact from dbo.Registration where Contact=@ContactNo)  
begin  
set @Ret_Val=-1;  
end  
else  
begin  
insert into dbo.Registration  
(Fname,Lname,EmailId,Address,City,Picode,Contact,Password,CreatedDate)  
values (@FName,@LName,@EmailId,@Address,@City,@PinCode,@ContactNo,@Password,GETDATE())  
set @Ret_Val=@@IDENTITY;  
end  
end
GO
