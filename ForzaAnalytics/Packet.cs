using System.Net.Sockets;

namespace ForzaAnalytics
{
    internal struct Packet
    {
        // Sled
        uint _IsRaceOn;
        public bool IsRaceOn => _IsRaceOn != 0;
        uint _TimestampMS;
        public long TimestampMS => _TimestampMS;
        public float EngineMaxRpm { get; }
        public float EngineIdleRpm { get; }
        public float CurrentEngineRpm { get; }
        public float AccelerationX { get; } // In the car's local space {get;} X = right, Y = up, Z = forward
        public float AccelerationY { get; }
        public float AccelerationZ { get; }
        public float VelocityX { get; } // In the car's local space {get;} X = right, Y = up, Z = forward
        public float VelocityY { get; }
        public float VelocityZ { get; }
        public float AngularVelocityX { get; } // In the car's local space {get;} X = pitch, Y = yaw, Z = roll
        public float AngularVelocityY { get; }
        public float AngularVelocityZ { get; }
        public float Yaw { get; }
        public float Pitch { get; }
        public float Roll { get; }
        public float NormalizedSuspensionTravelFrontLeft { get; } // Suspension travel normalized: 0.0f = max stretch {get;} 1.0 = max compression
        public float NormalizedSuspensionTravelFrontRight { get; }
        public float NormalizedSuspensionTravelRearLeft { get; }
        public float NormalizedSuspensionTravelRearRight { get; }
        public float TireSlipRatioFrontLeft { get; } // Tire normalized slip ratio, = 0 means 100% grip and |ratio| > 1.0 means loss of grip.
        public float TireSlipRatioFrontRight { get; }
        public float TireSlipRatioRearLeft { get; }
        public float TireSlipRatioRearRight { get; }
        public float WheelRotationSpeedFrontLeft { get; } // Wheel rotation speed radians/sec.
        public float WheelRotationSpeedFrontRight { get; }
        public float WheelRotationSpeedRearLeft { get; }
        public float WheelRotationSpeedRearRight { get; }
        public float WheelOnRumbleStripFrontLeft { get; } // = 1 when wheel is on rumble strip, = 0 when off.
        public float WheelOnRumbleStripFrontRight { get; }
        public float WheelOnRumbleStripRearLeft { get; }
        public float WheelOnRumbleStripRearRight { get; }
        public float WheelInPuddleDepthFrontLeft { get; } // = from 0 to 1, where 1 is the deepest puddle
        public float WheelInPuddleDepthFrontRight { get; }
        public float WheelInPuddleDepthRearLeft { get; }
        public float WheelInPuddleDepthRearRight { get; }
        public float SurfaceRumbleFrontLeft { get; } // Non-dimensional surface rumble values passed to controller force feedback
        public float SurfaceRumbleFrontRight { get; }
        public float SurfaceRumbleRearLeft { get; }
        public float SurfaceRumbleRearRight { get; }
        public float TireSlipAngleFrontLeft { get; } // Tire normalized slip angle, = 0 means 100% grip and |angle| > 1.0 means loss of grip.
        public float TireSlipAngleFrontRight { get; }
        public float TireSlipAngleRearLeft { get; }
        public float TireSlipAngleRearRight { get; }
        public float TireCombinedSlipFrontLeft { get; } // Tire normalized combined slip, = 0 means 100% grip and |slip| > 1.0 means loss of grip.
        public float TireCombinedSlipFrontRight { get; }
        public float TireCombinedSlipRearLeft { get; }
        public float TireCombinedSlipRearRight { get; }
        public float SuspensionTravelMetersFrontLeft { get; } // Actual suspension travel in meters
        public float SuspensionTravelMetersFrontRight { get; }
        public float SuspensionTravelMetersRearLeft { get; }
        public float SuspensionTravelMetersRearRight { get; }
        public int CarOrdinal { get; } // Unique ID of the car make/model // u
        public int CarClass { get; } // Between 0 (D -- worst cars) and 7 (X class -- best cars) inclusive // u
        public int CarPerformanceIndex { get; } // Between 100 (slowest car) and 999 (fastest car) inclusive // u
        public int DrivetrainType { get; } // Corresponds to EDrivetrainType {get;} 0 = FWD, 1 = RWD, 2 = AWD // u
        public int NumCylinders { get; } // Number of cylinders in the engine // u
        public int CarCategory { get; }

        public int _unknown1 { get; } // u
        public int _unknown2 { get; } // u

        // Dash
        public float PositionX { get; }
        public float PositionY { get; }
        public float PositionZ { get; }
        public float Speed { get; }
        public float Power { get; }
        public float Torque { get; }
        public float TireTempFl { get; }
        public float TireTempFr { get; }
        public float TireTempRl { get; }
        public float TireTempRr { get; }
        public float Boost { get; }
        public float Fuel { get; }
        public float Distance { get; set; }
        public float BestLapTime { get; private set; }
        public float LastLapTime { get; private set; }
        public float CurrentLapTime { get; private set; }
        public float CurrentRaceTime { get; set; }
        ushort _Lap;
        public int Lap => _Lap;
        public byte RacePosition { get; }
        public byte Accelerator { get; }
        public byte Brake { get; }
        public byte Clutch { get; }
        public byte Handbrake { get; }
        public byte Gear { get; }
        sbyte _Steer;
        public short Steer => _Steer;
        public byte NormalDrivingLine { get; }
        public byte NormalAiBrakeDifference { get; }

        public static unsafe Packet GetPacket(UdpReceiveResult p)
        {
            fixed (byte* d = p.Buffer)
            {
                Packet* pp = (Packet*)d;
                return *pp;
            }
        }

        public static unsafe int GetSize() => sizeof(Packet);

        internal bool IsStart()
        {
            return CurrentLapTime == 0 && Distance == 0 && LastLapTime == 0 && BestLapTime == 0 && Lap == 0;
        }

        internal Packet AsStart()
        {
            return this with
            {
                CurrentLapTime = 0,
                BestLapTime = 0,
                _Lap = 0,
                Distance = 0,
                LastLapTime = 0
            };
        }
    }
}
