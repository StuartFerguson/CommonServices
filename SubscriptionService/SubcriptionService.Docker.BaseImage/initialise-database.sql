CREATE DATABASE SubscriptionServiceConfiguration
GO

USE [SubscriptionServiceConfiguration_gh]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 27/09/2019 06:07:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CatchupSubscriptionConfigurations]    Script Date: 27/09/2019 06:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CatchupSubscriptionConfigurations](
	[SubscriptionId] [uniqueidentifier] NOT NULL,
	[CreateDateTime] [datetime2](7) NOT NULL,
	[EndPointUri] [nvarchar](max) NULL,
	[EventStoreServerId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Position] [int] NOT NULL,
	[StreamName] [nvarchar](max) NULL,
 CONSTRAINT [PK_CatchupSubscriptionConfigurations] PRIMARY KEY CLUSTERED 
(
	[SubscriptionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EventStoreServers]    Script Date: 27/09/2019 06:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EventStoreServers](
	[EventStoreServerId] [uniqueidentifier] NOT NULL,
	[ConnectionString] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
 CONSTRAINT [PK_EventStoreServers] PRIMARY KEY CLUSTERED 
(
	[EventStoreServerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubscriptionConfigurations]    Script Date: 27/09/2019 06:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubscriptionConfigurations](
	[SubscriptionId] [uniqueidentifier] NOT NULL,
	[EventStoreServerId] [uniqueidentifier] NOT NULL,
	[StreamName] [nvarchar](max) NULL,
	[GroupName] [nvarchar](max) NULL,
	[EndPointUri] [nvarchar](max) NULL,
	[StreamPosition] [int] NULL,
 CONSTRAINT [PK_SubscriptionConfigurations] PRIMARY KEY CLUSTERED 
(
	[SubscriptionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[CatchupSubscriptionConfigurations]  WITH CHECK ADD  CONSTRAINT [FK_CatchupSubscriptionConfigurations_EventStoreServers_EventSto~] FOREIGN KEY([EventStoreServerId])
REFERENCES [dbo].[EventStoreServers] ([EventStoreServerId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CatchupSubscriptionConfigurations] CHECK CONSTRAINT [FK_CatchupSubscriptionConfigurations_EventStoreServers_EventSto~]
GO
ALTER TABLE [dbo].[SubscriptionConfigurations]  WITH CHECK ADD  CONSTRAINT [FK_SubscriptionConfigurations_EventStoreServers_EventStoreServe~] FOREIGN KEY([EventStoreServerId])
REFERENCES [dbo].[EventStoreServers] ([EventStoreServerId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SubscriptionConfigurations] CHECK CONSTRAINT [FK_SubscriptionConfigurations_EventStoreServers_EventStoreServe~]
GO

USE [SubscriptionServiceConfiguration_gh]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20181210110753_InitialDatabase', N'2.2.6-servicing-10079')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20181212111524_RemoveIsNetCoreStream', N'2.2.6-servicing-10079')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20181223083413_AddMultiSubscriptionServiceSupport', N'2.2.6-servicing-10079')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190926192345_NewConfigFormat', N'2.2.6-servicing-10079')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190926201504_NewConfigFormatAddCatchup', N'2.2.6-servicing-10079')
GO

