USE [ForzaAnalytics]
GO
/****** Object:  StoredProcedure [dbo].[ClearAll]    Script Date: 2024/07/20 02:37:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[ClearAll]
AS
BEGIN
  exec dbo.ClearSessions

  delete from [Packet]
  DBCC CHECKIDENT ('[Packet]', RESEED, 0);

END
GO
