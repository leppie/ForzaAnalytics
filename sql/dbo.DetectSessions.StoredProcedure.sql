USE [ForzaAnalytics]
GO
/****** Object:  StoredProcedure [dbo].[DetectSessions]    Script Date: 2024/07/20 02:37:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[DetectSessions]
AS
BEGIN
  --update s
  --set EndId = (select max(Id) from [Packet] p where p.Distance <> 0 and p.CarOrdinal = s.CarId)
  --from (select top 1 * from [Session] order by 1 desc) s

  insert into [Car](Id, Name)
  select distinct CarOrdinal, 'New car'
  from [Packet] p
  where SessionId is null and CarOrdinal not in (select Id from [Car])

  select [Id] as StartId, (select max(p.Id) from Packet p where p.Id < [MaxId] and Distance <> 0 and p.CarOrdinal = d.CarOrdinal) as EndId,  CarOrdinal as CarId, PacketCount = 0
  into #sessions
  from 
  (
      SELECT [Id], [MaxId] = LEAD(Id) over (order by Id), CarOrdinal
      FROM [Packet]
      where SessionId is null
      and Distance = 0 and LastLapTime = 0 and BestLapTime = 0 and Lap = 0
  ) d
  order by 1 -- needed?

  update s
  set EndId = (select max(Id) from [Packet] p where p.Distance <> 0 and p.CarOrdinal = s.CarId)
  from (select top 1 * from #sessions order by 1 desc) s

  update s
  set PacketCount = (select count(1) from [Packet] p where p.Id between s.StartId and s.EndId and p.CarOrdinal = s.CarId)
  from #sessions s

  delete from #sessions where [PacketCount] < 60

  insert into [Session](StartId, EndId, CarId, PacketCount)
  select * from #sessions order by 1
   
  update p
  set SessionId = s.Id
  from Session s 
  join Packet p on p.Id between s.StartId and s.EndId
  where p.SessionId is null

  delete from [Packet] where SessionId is null

  declare @id int = ISNULL((select max(Id) from [Packet] where Line is not null), 0);
   
  update p
  set Line = Position.ShortestLineTo(NextPos)
  from 
  (
    select 
    *,
    NextPos = LEAD(Position) over (order by Id)
    from Packet 
    where Id > @id
  ) p
  
  update Packet
  set Line = NULL
  where Id > @id and Line.STLength() > 10
END
GO
