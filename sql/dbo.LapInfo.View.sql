USE [ForzaAnalytics]
GO
/****** Object:  View [dbo].[LapInfo]    Script Date: 2024/07/20 02:37:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   VIEW [dbo].[LapInfo]
AS

with LAPS as
(
    SELECT sess.Id as SessionId, p.Lap, StartId = min(p.Id), max(p.Id) as EndId, count(1) as PacketCount, 
    max(p.Speed) * 3.6 as MaxSpeed,
    max(abs(p.AccelerationX)) as MaxX, max(abs(p.AccelerationZ)) as MaxZ, 
    geometry::UnionAggregate(Line) as Line
    FROM Session sess
    join [Packet] [p] on p.Id between sess.StartId + 1 and sess.EndId
    --where Distance >= 0
    group by sess.Id, p.Lap
),
L2 as
(
    select l.*, NextId = LEAD(StartId) over(Order by StartId)
    from LAPS l 
)

select l.SessionId, l.Lap, 
l.StartId, l.EndId, l.PacketCount, 
HadCollision = IIF(l.MAxX > 55 or l.MaxZ > 55, 1, 0),
LapTime = n.CurrentRaceTime - s.CurrentRaceTime + s.CurrentLapTime,
l.MaxSpeed, 
AvgSpeed = l.Line.STLength()/(n.CurrentRaceTime - s.CurrentRaceTime + s.CurrentLapTime) * 3.6,
l.Line.STLength() as GeoDistance, 
l.Line,
Distance = (ISNULL(n.Distance, e.Distance) - s.Distance),
LapTime2 = n.LastLapTime
from L2 l 
join [Packet] s on s.Id = l.StartId
join [Packet] e on e.Id = l.EndId
left join [Packet] n on n.Id = l.NextId and n.Lap = e.Lap + 1
where PacketCount > 60



GO
