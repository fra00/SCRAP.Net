using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRobot.Common.Graphics
{
    public class ImageUtility
    {
        // Calcola la luminosità di un colore in base ai suoi componenti RGB
        private static double GetBrightness(int r, int g, int b)
        {
            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }

        // Calcola la luminosità media di un'immagine
        static double GetAverageBrightness(Image<Rgba32> image)
        {
            // Inizializza la somma e il conteggio dei pixel
            double sum = 0;
            int count = 0;

            // Scorre tutti i pixel dell'immagine
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    // Ottiene il colore del pixel corrente
                    var color = image[x, y];

                    // Ottiene i componenti RGB del colore
                    int r = color.R;
                    int g = color.G;
                    int b = color.B;

                    // Calcola la luminosità del colore
                    double brightness = GetBrightness(r, g, b);

                    // Aggiunge la luminosità alla somma
                    sum += brightness;

                    // Incrementa il conteggio dei pixel
                    count++;
                }
            }

            // Restituisce la media della somma divisa per il conteggio
            return sum / count;
        }

        // Verifica se un'immagine è scura o no in base alla sua luminosità media
        public static bool IsDarkImage(Image<Rgba32> image, double? threshold = 50)
        {
            // Sceglie una soglia di luminosità
            //double threshold = 50;

            // Calcola la luminosità media dell'immagine
            double averageBrightness = GetAverageBrightness(image);

            // Restituisce vero se la luminosità media è inferiore alla soglia, falso altrimenti
            return averageBrightness < threshold;
        }

        public static List<RPoint> ListPixelOfColor(Image<Rgba32> inputImage, int r, int g, int b,
            int toleranceRed, int toleranceGreen, int toleranceBlue)
        {
            var pointsColor = new List<RPoint>();
            // Scorri tutti i pixel dell'immagine
            for (int x = 0; x < inputImage.Width; x++)
            {
                for (int y = 0; y < inputImage.Height; y++)
                {
                    // Ottieni il colore del pixel corrente
                    Rgba32 pixelColor = inputImage[x, y];

                    // Calcola la differenza tra il colore del pixel e il colore dato
                    int diffRed = Math.Abs(pixelColor.R - r);
                    int diffGreen = Math.Abs(pixelColor.G - g);
                    int diffBlue = Math.Abs(pixelColor.B - b);

                    // Se la differenza è minore della tolleranza per ogni componente, restituisci le coordinate del pixel
                    if (diffRed <= toleranceRed && diffGreen <= toleranceGreen && diffBlue <= toleranceBlue)
                    {
                        pointsColor.Add(new RPoint(x, y));
                    }
                }
            }
            // Se non viene trovato nessun pixel con il colore simile, restituisci un punto non valido
            return pointsColor;
        }

        // Metodo che restituisce la posizione del pixel che ha un colore simile a quello dato
        public static RPoint PositionOfColor(Image<Rgba32> inputImage, int r, int g, int b, int toleranceRed, int toleranceGreen, int toleranceBlue)
        {
            List<RPoint> points = ListPixelOfColor(inputImage,r,g,b,toleranceRed,toleranceGreen, toleranceBlue);
            if (!points.Any()) return null;
            var point = GetPointWithHighestDensity(points);
            return point;
        }

        public static RPoint GetPointWithHighestDensity(List<RPoint> points)
        {
            // Creo una variabile per memorizzare il punto con la somma minore
            RPoint bestPoint = new RPoint();

            // Creo una variabile per memorizzare la somma minore
            double minSum = double.MaxValue;

            // Scorro tutti i punti della lista
            foreach (RPoint p1 in points)
            {
                // Creo una variabile per memorizzare la somma delle distanze del punto corrente
                double sum = 0;

                // Scorro tutti gli altri punti della lista
                foreach (RPoint p2 in points)
                {
                    // Se il punto corrente è diverso dall'altro punto
                    if (p1 != p2)
                    {
                        // Calcolo la distanza tra i due punti usando il teorema di Pitagora
                        double distance = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));

                        // Aggiungo la distanza alla somma
                        sum += distance;
                    }
                }

                // Se la somma è minore della somma minore
                if (sum < minSum)
                {
                    // Aggiorno la somma minore con la somma corrente
                    minSum = sum;

                    // Aggiorno il punto con la somma minore con il punto corrente
                    bestPoint = p1;
                }
            }

            // Restituisco il punto con la somma minore
            return bestPoint;
        }

    }
}
