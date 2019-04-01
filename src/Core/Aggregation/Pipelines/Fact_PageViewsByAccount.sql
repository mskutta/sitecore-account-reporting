/****** Object:  Table [dbo].[Fact_PageViewsByAccount]    Script Date: 1/24/2019 11:05:10 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Fact_PageViewsByAccount](
	[Date] [smalldatetime] NOT NULL,
	[ItemId] [uniqueidentifier] NOT NULL,
	[AccountId] [uniqueidentifier] NOT NULL,
	[Views] [bigint] NOT NULL,
	[Duration] [bigint] NOT NULL,
	[Visits] [bigint] NOT NULL,
	[Value] [bigint] NOT NULL,
	[Bounces] [bigint] NOT NULL,
	[Conversions] [bigint] NOT NULL,
 CONSTRAINT [PK_Fact_PageViewsByAccount] PRIMARY KEY CLUSTERED 
(
	[Date] ASC,
	[ItemId] ASC,
	[AccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Fact_PageViewsByAccount]  WITH NOCHECK ADD  CONSTRAINT [FK_Fact_PageViewsByAccount_Accounts] FOREIGN KEY([AccountId])
REFERENCES [dbo].[Accounts] ([AccountId])
GO

ALTER TABLE [dbo].[Fact_PageViewsByAccount] NOCHECK CONSTRAINT [FK_Fact_PageViewsByAccount_Accounts]
GO

ALTER TABLE [dbo].[Fact_PageViewsByAccount]  WITH NOCHECK ADD  CONSTRAINT [FK_Fact_PageViewsByAccount_Items] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([ItemId])
GO

ALTER TABLE [dbo].[Fact_PageViewsByAccount] NOCHECK CONSTRAINT [FK_Fact_PageViewsByAccount_Items]
GO



