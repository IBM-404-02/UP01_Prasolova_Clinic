USE [master]
GO

CREATE DATABASE [DB_Prasolova_Clinic]
GO

USE [DB_Prasolova_Clinic]
GO

CREATE TABLE [dbo].[Users](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [Login] [nvarchar](50) NOT NULL,
    [Password] [nvarchar](100) NOT NULL,
    [Role] [nvarchar](20) NOT NULL,
    [FIO] [nvarchar](150) NOT NULL,
    [Photo] [nvarchar](255) NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([ID] ASC)
)
GO

CREATE TABLE [dbo].[ServiceTypes](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](100) NOT NULL,
    CONSTRAINT [PK_ServiceTypes] PRIMARY KEY CLUSTERED ([ID] ASC)
)
GO

CREATE TABLE [dbo].[ServicesRendered](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [ServiceTypeID] [int] NOT NULL,
    [UserID] [int] NOT NULL,
    [Date] [date] NOT NULL,
    [ServiceName] [nvarchar](150) NOT NULL,
    [Quantity] [int] NOT NULL,
    [Price] [decimal](10, 2) NOT NULL,
    CONSTRAINT [PK_ServicesRendered] PRIMARY KEY CLUSTERED ([ID] ASC)
)
GO

ALTER TABLE [dbo].[ServicesRendered] WITH CHECK
    ADD CONSTRAINT [FK_ServicesRendered_ServiceTypes] FOREIGN KEY([ServiceTypeID])
    REFERENCES [dbo].[ServiceTypes] ([ID]) ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ServicesRendered] WITH CHECK
    ADD CONSTRAINT [FK_ServicesRendered_Users] FOREIGN KEY([UserID])
    REFERENCES [dbo].[Users] ([ID]) ON DELETE CASCADE
GO

SET IDENTITY_INSERT [dbo].[ServiceTypes] ON
INSERT [dbo].[ServiceTypes] ([ID], [Name]) VALUES
(1, N'Консультация'),
(2, N'Диагностика'),
(3, N'Лабораторные анализы'),
(4, N'Процедуры')
SET IDENTITY_INSERT [dbo].[ServiceTypes] OFF
GO

SET IDENTITY_INSERT [dbo].[Users] ON
INSERT [dbo].[Users] ([ID], [Login], [Password], [Role], [FIO], [Photo]) VALUES
(1, N'admin', N'7AF2D10B73AB7CD8F603937F7697CB5FE432C7FF', N'Admin', N'Петрова Анна Сергеевна', NULL),
(2, N'ivanov', N'CD027069371CDB4F80C68DCFB37E6F4A1BDB0222', N'User', N'Иванов Иван Иванович', NULL),
(3, N'smirnova', N'C525E4CF653707DC26A5DC2DB870A8FA3AF188EA', N'User', N'Смирнова Ольга Петровна', NULL)
SET IDENTITY_INSERT [dbo].[Users] OFF
GO

SET IDENTITY_INSERT [dbo].[ServicesRendered] ON
INSERT [dbo].[ServicesRendered] ([ID], [ServiceTypeID], [UserID], [Date], [ServiceName], [Quantity], [Price]) VALUES
(1, 1, 2, '2026-09-15', N'Первичный приём терапевта', 1, 1800.00),
(2, 3, 2, '2026-09-15', N'Общий анализ крови', 1, 650.00),
(3, 2, 2, '2026-09-20', N'УЗИ брюшной полости', 1, 2500.00),
(4, 1, 3, '2026-09-18', N'Приём кардиолога', 1, 2200.00),
(5, 2, 3, '2026-09-18', N'ЭКГ', 1, 900.00),
(6, 4, 3, '2026-09-25', N'Внутримышечная инъекция', 3, 300.00),
(7, 1, 2, '2026-10-01', N'Повторный приём терапевта', 1, 1500.00),
(8, 3, 2, '2026-10-01', N'Биохимический анализ крови', 1, 1200.00),
(9, 2, 2, '2026-10-05', N'Рентген грудной клетки', 1, 1800.00),
(10, 4, 2, '2026-10-08', N'Физиотерапия', 5, 600.00),
(11, 1, 3, '2026-10-03', N'Приём невролога', 1, 2100.00),
(12, 3, 3, '2026-10-03', N'Анализ мочи', 1, 400.00),
(13, 2, 3, '2026-10-10', N'МРТ коленного сустава', 1, 5500.00),
(14, 4, 3, '2026-10-12', N'Массаж спины', 10, 1000.00)
SET IDENTITY_INSERT [dbo].[ServicesRendered] OFF
GO
