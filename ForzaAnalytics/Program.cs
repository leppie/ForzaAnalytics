using System.Diagnostics;
using System.Net.Sockets;
using Dapper;
using ForzaAnalytics;

Console.WriteLine("FA client");

Debug.Assert(Packet.GetSize() == 324);

using var sql = await DB.Setup();

await sql.ExecuteAsync("EXEC DetectSessions");

string cmd = DB.CreateInsertCmd();

using var udp = new UdpClient(8000);

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (o, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

try
{
    var lastRT = float.NegativeInfinity;
    var lastD = float.NegativeInfinity;
    DateTimeOffset ts = DateTimeOffset.MinValue;
    Console.WriteLine("Running");
    while (true)
    {
        var p = await udp.ReceiveAsync(cts.Token);
        var pp = Packet.GetPacket(p);

        if (pp.IsRaceOn && pp.RacePosition > 0)
        {
            var isStart = pp.IsStart();
            if (isStart || ts == DateTimeOffset.MinValue)
            {
                await sql.ExecuteAsync("EXEC DetectSessions");

                if (ts == DateTimeOffset.MinValue && !isStart)
                {
                    var start = pp.AsStart();
                    await sql.ExecuteAsync(cmd, new[] { start });
                }
                ts = DateTimeOffset.Now;
                Console.WriteLine("S");
            }
            else if (((pp.CurrentRaceTime < lastRT || (pp.Distance + 25 < lastD && lastD != 0)) && pp.Distance != 0))
            {
                if (lastD - pp.Distance > 2000 && pp.CurrentLapTime == 0 && pp.Lap == 1)
                {
                    pp.CurrentRaceTime = pp.LastLapTime;
                    pp.Distance = 5954;
                    Console.WriteLine("X");
                }
                else if (!(pp.Steer == -127 && pp.Speed < 0.05 && pp.Handbrake == 255))
                {
                    var d = await DB.PurgeAfter(sql, pp.CurrentRaceTime, pp.Distance, ts);
                    Console.WriteLine("P:{0}", d);
                }
                else // LO, LJ, CH, BO
                {
                    Console.WriteLine("Q:{0}", pp.Distance);
                }
            }
            else if (pp.Distance == 5954 && pp.CurrentLapTime == 0 && pp.Lap == 1)
            {
                pp.CurrentRaceTime = pp.LastLapTime;
                Console.WriteLine("X");
            }

            var a = await sql.ExecuteAsync(cmd, new[] { pp });
            Console.Write(a);
            Console.Write((char)8);

            lastRT = pp.CurrentRaceTime;
            lastD = pp.Distance;
        }
    }
}
catch (OperationCanceledException)
{
}

await sql.ExecuteAsync("EXEC DetectSessions");
Console.WriteLine("Exiting");





