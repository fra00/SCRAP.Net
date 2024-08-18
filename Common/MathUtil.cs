using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MainRobot.Common
{
    public class MathUtil
    {

        public static RPoint MovePointOfDistance(RPoint p, double distance, double angle)
        {
            double radiansAngle = angle * Math.PI / 180;
            int newX = p.X + (int)(distance * Math.Cos(radiansAngle));
            int newY = p.Y + (int)(distance * Math.Sin(radiansAngle));
            return new RPoint(newX, newY);
        }

        public static RPoint MovePointOfDistanceRounded10(RPoint p, double distance, double angle)
        {
            var newPoint = MathUtil.MovePointOfDistance(p, distance, angle);
            var nextX = newPoint.X - (newPoint.X % 10);
            var nextY = newPoint.Y - (newPoint.Y % 10);
            return newPoint;
        }

        public static double AngleBetweenTwoPoints(RPoint p1, RPoint p2)
        {
            // angle in degrees
            return Math.Atan2(p2.Y - p1.Y, p2.X - p1.X) * 180 / Math.PI;
        }

        public static double AngleBetweenTwoPoint180Origin(RPoint p1, RPoint p2)
        {
            // calcola l'angolo tra i punti in radianti
            double angolo = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
            // converti in gradi
            angolo = angolo * 180 / Math.PI;
            // aggiungi 180 gradi se i punti sono nel terzo o quarto quadrante
            //if (x1 < 0 && x2 < 0)
            //{
                angolo += 180;
            //}
            // usa il modulo 360 per restituire l'angolo in [0, 360)
            angolo = angolo % 360;
            // restituisci l'angolo
            return angolo;
        }


        public static double DifferenceTwoAngle(double angle, double angle1) {
            // Calcola la differenza tra gli angoli
            var differenza = angle - angle1;

            // Se la differenza è negativa, la fa diventare positiva
            if (differenza < 0)
            {
                differenza += 360;
            }

            // Ritorna la differenza
            return differenza;
        }

        /// <summary>
        /// esegue la differenza di due angoli con il segno tra 180,0,-180
        /// </summary>
        /// <param name="firstAngle"></param>
        /// <param name="secondAngle"></param>
        /// <returns></returns>
        public static double DifferenceTwoAngleSigned(double firstAngle, double secondAngle)
        {
            double difference = secondAngle - firstAngle;
            while (difference < -180) difference += 360;
            while (difference > 180) difference -= 360;
            return difference;
        }

        public static double DifferenceTwoAngleZero(double angle, double angle1)
        {
            // Differenza tra gli angoli
            double differenza = angle - angle1;

            // Se a è minore di b, inverte il segno della differenza
            if (angle < angle1)
            {
                differenza *= -1;
            }

            // Ritorna la differenza
            return differenza;
        }
        

        public static double SumTwoAngle(double angle, double angle1)
        {
            // Somma gli angoli
            var somma = angle + angle1;

            // Se la somma è maggiore di 359, sottrae 360
            while (somma > 359)
            {
                somma -= 360;
            }

            // Ritorna la somma
            return somma;
        }
        public static double Distance(RPoint p1, RPoint p2)
        {
            // Calcola la differenza tra le coordinate x
            double dx = p2.X - p1.X;

            // Calcola la differenza tra le coordinate y
            double dy = p2.Y - p1.Y;

            // Calcola la distanza euclidea
            return Math.Sqrt(dx * dx + dy * dy);
        }

    }
}
