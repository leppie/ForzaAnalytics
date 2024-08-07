USE [ForzaAnalytics]
GO
/****** Object:  StoredProcedure [dbo].[GuessTrackFromSession]    Script Date: 2024/07/20 02:37:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[GuessTrackFromSession]
(
    @startId int
)
AS
BEGIN
  
  declare @layout geometry, @startPos geometry, @endPos geometry, @bb geometry;

  select @layout = geometry::UnionAggregate(Line).BufferWithCurves(1)
  from [Session] s
  join [Packet] p on p.Id between s.StartId and s.EndId
  where s.Id = @startId

  select @bb = @layout.STEnvelope()

  select @startPos = geometry::ConvexHullAggregate(Position).STCentroid()
  from [Session] s
  join [Packet] p on p.Id between s.StartId and s.EndId
  where s.Id = @startId
  and Lap <> 0 and CurrentLapTime = 0

  select t.*
  into #ct
  from [Track] t 
  where t.BoundingBox.Filter(@bb) = 1 and t.Layout.Filter(@layout) = 1
  
  declare @c int = (select count(1) from #ct);

  if (@c > 1)
  begin
    update [Session]
    set TrackId = (select top 1 Id from #ct where @startPos.ShortestLineTo(StartPosition).STLength() < 10 order by @startPos.ShortestLineTo(StartPosition).STLength())
    where Id = @startId

    select *, @startPos.ShortestLineTo(StartPosition).STLength() as SO from #ct where @startPos.ShortestLineTo(StartPosition).STLength() < 10
    order by SO
  end
  else
  begin
    update [Session]
    set TrackId = (select Id from #ct)
    where Id = @startId

    select * from #ct
  end

END
GO
