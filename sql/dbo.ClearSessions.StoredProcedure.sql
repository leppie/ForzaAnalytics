USE [ForzaAnalytics]
GO
/****** Object:  StoredProcedure [dbo].[ClearSessions]    Script Date: 2024/07/20 02:37:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[ClearSessions]
AS
BEGIN
  update [Packet] set SessionId = NULL, Line = NULL
  delete from [Session]
  DBCC CHECKIDENT ('[Session]', RESEED, 0);

END
GO
