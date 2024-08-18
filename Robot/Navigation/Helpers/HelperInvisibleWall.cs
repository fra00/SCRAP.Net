using MainRobot.Common;
using Newtonsoft.Json;
using Robot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRobot.Robot.Navigation.Helpers
{
    public class HelperInvisibleWall : IHelperInvisibleWall
    {
        private List<RPoint> _points;

        public HelperInvisibleWall()
        {
            this.Load();
        }

        public void Add(RPoint point)
        {
            if (Find(point)) return;
            _points.Add(point);
        }

        public bool Find(RPoint point)
        {
            int minX = point.X - RobotConfiguration.HALF_WIDTH_ROBOT;
            int maxX = point.X + RobotConfiguration.HALF_WIDTH_ROBOT;
            int minY = point.Y - RobotConfiguration.HALF_HEIGHT_ROBOT;
            int maxY = point.Y + RobotConfiguration.HALF_HEIGHT_ROBOT;

            minX = Math.Max(0, minX);
            maxX = Math.Min(RobotConfiguration.WIDHT_MAP, maxX);
            minY = Math.Max(0, minY);
            maxY = Math.Min(RobotConfiguration.HEIGHT_MAP, maxY);

            return _points.Any(c => c.X >= minX && c.X <= maxX && c.Y >= minY && c.Y <= maxY);
        }

        public void Load()
        {
            var json = File.ReadAllText("invisibleWall.json");            
            _points = string.IsNullOrEmpty(json) ? new List<RPoint>() : JsonConvert.DeserializeObject<List<RPoint>>(json);
        }

        public void Remove(RPoint point)
        {
            if (point == null) return;
            var p = _points.FirstOrDefault(c => c.X == point.X && c.Y == point.Y);
            if (p == null) return;
            _points.Remove(p);
        }

        public void Save()
        {
            var t = JsonConvert.SerializeObject(_points);
            // Scrivi la nuova stringa json su un file di testo
            File.WriteAllText("invisibleWall.json", t);
        }
    }

    public interface IHelperInvisibleWall {
        void Load();
        void Save();
        void Add(RPoint point);
        void Remove(RPoint point);
        bool Find(RPoint point);
    }
}
