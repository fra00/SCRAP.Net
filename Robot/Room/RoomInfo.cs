using MainRobot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRobot.Robot.Room
{
    public class RoomInfo : IRoomInfo
    {
        private Dictionary<string, RPoint> dictRoomPoint { get; set; }

        public RoomInfo()
        {
            dictRoomPoint = loadRoomPoint();
        }

        private Dictionary<string, RPoint> loadRoomPoint()
        {
            return new Dictionary<string, RPoint> {
                { "cucina",new RPoint(460,290) },
                { "sala",new RPoint(200,700) },
                { "ingresso",new RPoint(70,300) },
                { "corridoio",new RPoint(730,460) },
                { "bagno",new RPoint(680,370) },
                { "camera",new RPoint(900,590) },
                { "cameretta",new RPoint(840,280) },
                { "studio",new RPoint(630,490) }
            };
        }

        public RPoint? GetPointRoom(string room)
        {
            RPoint point = null;
            dictRoomPoint.TryGetValue(room, out point);
            return point;
        }
    }
}
