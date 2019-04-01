/****** Object:  Table [dbo].[Fact_ContactsByAccount]    Script Date: 1/24/2019 11:05:10 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Fact_ContactsByAccount](
	[Date] [smalldatetime] NOT NULL,
	[ContactId] [uniqueidentifier] NOT NULL,
	[AccountId] [uniqueidentifier] NOT NULL,
	[Views] [bigint] NOT NULL,
	[Duration] [bigint] NOT NULL,
	[Visits] [bigint] NOT NULL,
	[Value] [bigint] NOT NULL,
	[Bounces] [bigint] NOT NULL,
	[Conversions] [bigint] NOT NULL,
 CONSTRAINT [PK_Fact_ContactsByAccount] PRIMARY KEY CLUSTERED 
(
	[Date] ASC,
	[ContactId] ASC,
	[AccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Fact_ContactsByAccount]  WITH NOCHECK ADD  CONSTRAINT [FK_Fact_ContactsByAccount_Contacts] FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contacts] ([ContactId])
GO

ALTER TABLE [dbo].[Fact_ContactsByAccount] NOCHECK CONSTRAINT [FK_Fact_ContactsByAccount_Contacts]
GO

ALTER TABLE [dbo].[Fact_ContactsByAccount]  WITH NOCHECK ADD  CONSTRAINT [FK_Fact_ContactsByAccount_Accounts] FOREIGN KEY([AccountId])
REFERENCES [dbo].[Accounts] ([AccountId])
GO

ALTER TABLE [dbo].[Fact_ContactsByAccount] NOCHECK CONSTRAINT [FK_Fact_ContactsByAccount_Accounts]
GO



