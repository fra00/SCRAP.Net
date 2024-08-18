namespace MainRobot.Common
{
    public class Location
    {
        public int Id;
        public RPoint Position;
        public int Angle;

        public Location() { }

        public Location(int id, RPoint position, int angle)
        {
            Id = id;
            Position = position;
            Angle = angle;
        }

    }

}
