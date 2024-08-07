USE [ForzaAnalytics]
GO
/****** Object:  StoredProcedure [dbo].[EstimateTrackFromSessions]    Script Date: 2024/07/20 02:37:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[EstimateTrackFromSessions]
(
    @name nvarchar(100),
    @startId int,
    @endId int = @startId
)
AS
BEGIN
  
  declare @layout geometry, @startPos geometry, @endPos geometry;

  select @layout = geometry::UnionAggregate(Line).BufferWithCurves(5)
  from [Session] s
  join [Packet] p on p.Id between s.StartId and s.EndId
  where s.Id between @startId and @endId

  select @startPos = geometry::ConvexHullAggregate(Position).STCentroid()
  from [Session] s
  join [Packet] p on p.Id between s.StartId and s.EndId
  where s.Id between @startId and @endId
  and Lap <> 0 and CurrentLapTime = 0

  insert into [Track](Name, Layout, BoundingBox, StartPosition, EndPosition)
  values (@name, @layout, @layout.STEnvelope(), @startPos, @endPos)

END
GO
