USE [ForzaAnalytics]
GO
/****** Object:  Table [dbo].[Lap]    Script Date: 2024/07/20 02:37:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Lap](
	[SessionId] [int] NOT NULL,
	[Lap] [int] NOT NULL,
	[StartId] [int] NULL,
	[EndId] [int] NULL,
	[PacketCount] [int] NULL,
	[Line] [geometry] NULL,
 CONSTRAINT [PK_Lap] PRIMARY KEY CLUSTERED 
(
	[SessionId] ASC,
	[Lap] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Lap]  WITH CHECK ADD  CONSTRAINT [FK_Lap_Packet] FOREIGN KEY([StartId])
REFERENCES [dbo].[Packet] ([Id])
GO
ALTER TABLE [dbo].[Lap] CHECK CONSTRAINT [FK_Lap_Packet]
GO
ALTER TABLE [dbo].[Lap]  WITH CHECK ADD  CONSTRAINT [FK_Lap_Packet1] FOREIGN KEY([StartId])
REFERENCES [dbo].[Packet] ([Id])
GO
ALTER TABLE [dbo].[Lap] CHECK CONSTRAINT [FK_Lap_Packet1]
GO
ALTER TABLE [dbo].[Lap]  WITH CHECK ADD  CONSTRAINT [FK_Lap_Session] FOREIGN KEY([SessionId])
REFERENCES [dbo].[Session] ([Id])
GO
ALTER TABLE [dbo].[Lap] CHECK CONSTRAINT [FK_Lap_Session]
GO
