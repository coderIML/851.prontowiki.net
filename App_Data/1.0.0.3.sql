/****** Object:  Table [dbo].[Attachments]    Script Date: 04/08/2006 18:04:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Attachments](
	[AttachmentID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_Attachments_AttachmentID]  DEFAULT (newid()),
	[PageName] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[AttachmentName] [varchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[AttachmentData] [image] NOT NULL,
	[Extension] [varchar](4) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ChangedBy] [nvarchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Modified] [timestamp] NULL,
 CONSTRAINT [PK_Attachments_1] PRIMARY KEY CLUSTERED 
(
	[AttachmentID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF