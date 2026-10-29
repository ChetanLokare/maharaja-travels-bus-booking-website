/* OPTIONAL - demo data only.
   The sample schedules in 01_Schema_And_Reference_Data.sql are dated 31-03-2024, so the search page hides them
   as "past trips". This moves them to the next few days so you can try the app right away.
   Do NOT run this against real data. */
USE [OnlineBusBooking];
GO
UPDATE dbo.ScheduleMaster
SET [Date] = CONVERT(varchar(10), DATEADD(DAY, 1 + (ScheduleId % 3), CAST(GETDATE() AS date)), 105)   -- dd-MM-yyyy
WHERE [Date] = '31-03-2024';
GO
