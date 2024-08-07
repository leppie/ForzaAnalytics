USE [ForzaAnalytics]
GO
/****** Object:  StoredProcedure [dbo].[DeleteSession]    Script Date: 2024/07/20 02:37:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[DeleteSession]
(
    @id int
)
AS
BEGIN
  update [Packet] set SessionId = NULL where SessionId = @id
  delete from [Session] where Id = @id
END
GO
