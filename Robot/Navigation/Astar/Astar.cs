using MainRobot.Common;
using MainRobot.Robot.Navigation.Helpers;
using Newtonsoft.Json;
using Robot;
using System.Linq;
using System.Runtime.CompilerServices;

//TODO:
//implementare logica fake obstacle
//verificare se viene richiamato il costrutto e temp obstacle viene azzerato in tal caso 
//svuotarlo ogni tot tempo

namespace MainRobot.Robot.Navigation.Astar
{
    public class Astar : IAstar
    {
        private IHelperInvisibleWall helperInvisbleWall;

        private List<Path>? OpenPath;
        private List<PointPath>? ClosedPath;
        private Path? End;
        private string? ordClosest;

        //conversione dell'immagine della mappa in un array di punti
        bool[,] wallInMap = new bool[1000, 1000];

        public bool[,] obstacleInMap = new bool[1000, 1000];
        private List<Weight> astarWeight = null;

        public List<Weight> AstarWeight
        {
            get
            {
                return astarWeight;
            }
        }


        private int CalcolaMediaPonderata(int x, int y)
        {
            var sum = 0;
            var count = 0;


            // Filtra i dati all'interno del quadrato
            foreach (var punto in astarWeight)
            {
                double distanzaX = Math.Abs(punto.X - x);
                double distanzaY = Math.Abs(punto.Y - y);
                if (distanzaX <= RobotConfiguration.HALF_WIDTH_ROBOT && distanzaY <= RobotConfiguration.HALF_HEIGHT_ROBOT)
                {
                    count+=1;
                    sum += punto.W;
                }
            }

            // Se non ci sono dati all'interno del quadrato, restituisci 0
            if (count==0)
            {
                return 0;
            }

            var media = sum / count;
            return media;
            
        }


        private void initAstarWeight()
        {
            var json = File.ReadAllText("weight.json");
            astarWeight = JsonConvert.DeserializeObject<List<Weight>>(string.IsNullOrEmpty(json) ? "[]" : json);
        }


        private void convertImgToList()
        {
            // Ottengo le dimensioni dell'immagine
            int width = 1000;
            int height = 1000;
            // Apro l'immagine usando la classe Image di ImageSharp
            using (Image<Rgba32> image = Image.Load<Rgba32>("mappaMuri.png"))
            {
                // Itero su tutti i pixel dell'immagine
                for (int y = 0; y < width; y++)
                {
                    for (int x = 0; x < height; x++)
                    {

                        // Ottengo il colore del pixel in posizione (200, 200) usando il metodo GetPixelRowSpan
                        Rgba32 color = image[x, y];
                        var wall = color.R == 0 && color.G == 0 && color.B == 0;
                        wallInMap[x, y] = wall;
                    }
                }
            }
        }
        public Astar(IHelperInvisibleWall helperInvisbleWall = null)
        {
            this.helperInvisbleWall = helperInvisbleWall;

            OpenPath = new List<Path>();
            ClosedPath = new List<PointPath>();

            initAstarWeight();

            convertImgToList();
        }

        public Path FindPath(RPoint s, RPoint e)
        {
            OpenPath = new List<Path>();
            ClosedPath = new List<PointPath>();

            this.End = new Path(e);
            Path startPath = new Path(s);

            //sceglie in quale quadrante è l'arrivo e orienta l'ordine di ricerca dei vicini
            //per favorire la ricerca dei tuoi arrivi
            int dX = s.X - e.X;
            int dY = s.Y - e.Y;

            ordClosest = "t,r,b,l";

            if (dX > 0 && dY > 0)
                ordClosest = "t,l,b,r";
            if (dX < 0 && dY > 0)
                ordClosest = "t,r,b,l";
            if (dX < 0 && dY < 0)
                ordClosest = "b,r,t,l";
            if (dX > 0 && dY < 0)
                ordClosest = "b,l,t,r";

            startPath.Score = this.CalculateScore(startPath);
            startPath.Tentative_G = 0;
            OpenPath.Add(startPath);
            return Search();
        }

        public Path Search()
        {
            while (OpenPath.Any())
            {
                OpenPath = OpenPath.OrderBy(c => c.Score).ToList();
                // x := the node in openset having the lowest f_score[] value

                Path current = OpenPath.First();

                if (current.CurrentPoint.X == End.CurrentPoint.X
                        && current.CurrentPoint.Y == End.CurrentPoint.Y)
                    return current;

                // remove x from openset
                OpenPath.Remove(current);

                // add x to closedset
                ClosedPath.Add(current.CurrentPoint);

                List<Path> closest = FindClosest(current);
                // foreach y in neighbor_nodes(x)
                foreach (Path cl in closest)
                {
                    // if y in closedset
                    // continue
                    bool isInClosed = false;
                    foreach (PointPath v in ClosedPath)
                    {
                        if (v.X == cl.CurrentPoint.X && v.Y == cl.CurrentPoint.Y)
                        {
                            isInClosed = true;
                            break;
                        }
                    }
                    if (isInClosed)
                        continue;

                    // tentative_g_score := g_score[x] + dist_between(x,y)
                    int tentative = current.Tentative_G
                            + (RobotConfiguration.MIN_STEP_FOR_FINDPATH /* + 100 */);

                    // if y not in openset
                    // add y to openset
                    bool isInOpen = false;
                    foreach (Path open in this.OpenPath)
                    {
                        if (open.CurrentPoint.X == cl.CurrentPoint.X
                                && open.CurrentPoint.Y == cl.CurrentPoint.Y)
                        {
                            isInOpen = true;
                            break;
                        }
                    }

                    bool tentative_is_better = false;
                    if (isInOpen == false)
                    {
                        // tentative_is_better := true
                        OpenPath.Add(cl);
                        tentative_is_better = true;
                    }
                    else if (tentative < cl.Tentative_G)
                    {
                        // elseif tentative_g_score < g_score[y]
                        // tentative_is_better := true
                        tentative_is_better = true;
                    }

                    if (tentative_is_better)
                    {
                        /*
                         * came_from[y] := x g_score[y] := tentative_g_score
                         * h_score[y] := heuristic_estimate_of_distance(y, goal)
                         * f_score[y] := g_score[y] + h_score[y]
                         */
                        cl.Parent = current;
                        cl.Tentative_G = tentative;

                        cl.Score = this.CalculateScore(cl) + CalcolaMediaPonderata(cl.CurrentPoint.X,cl.CurrentPoint.Y)
                                + tentative;
                        //cl.Score = this.CalculateScore(cl) + cl.CurrentPoint.w
                        //        + tentative;
                        // System.out.println("closest x" +cl.CurrentPoint.X +
                        // "y"+cl.CurrentPoint.Y+ " " + cl.Score + " " +
                        // cl.Tentative_G + " " + (cl.Score + cl.Tentative_G));
                    }
                }
            }
            return null;
        }


        public List<Path> FindClosest(Path p)
        {
            List<Path> newListPath = new List<Path>();

            foreach (var ord in ordClosest.Split(","))
            {
                int DX = 0; int DY = 0;
                PointPath? closest = null;
                switch (ord)
                {
                    case "t":
                        DY = -RobotConfiguration.MIN_STEP_FOR_FINDPATH;
                        break;
                    case "r":
                        DX = RobotConfiguration.MIN_STEP_FOR_FINDPATH;
                        break;
                    case "b":
                        DY = RobotConfiguration.MIN_STEP_FOR_FINDPATH;
                        break;
                    case "l":
                        DX = -RobotConfiguration.MIN_STEP_FOR_FINDPATH;
                        break;

                }

                closest = new PointPath(p.CurrentPoint.X + DX,
                                        p.CurrentPoint.Y + DY);
                Path newPath = new Path(closest);
                if (this.CalculateValidity(newPath))
                {
                    newListPath.Add(newPath);
                }
            }
            return newListPath;
        }

        public Boolean CalculateValidity(Path p)
        {
            bool increment = false;

            if (p.CurrentPoint.X < 0 || p.CurrentPoint.Y < 0)
            {
                return false;
            }
            if (p.CurrentPoint.X >= RobotConfiguration.WIDHT_MAP
                    || p.CurrentPoint.Y >= RobotConfiguration.HEIGHT_MAP)
            {
                return false;
            }


            //if (wallInMap[p.CurrentPoint.X, p.CurrentPoint.Y])
            //{
            //    return false;
            //}
            for (var yy = -RobotConfiguration.HALF_HEIGHT_ROBOT; yy < RobotConfiguration.HALF_HEIGHT_ROBOT; yy++)
            {
                if (p.CurrentPoint.Y + yy > RobotConfiguration.HEIGHT_MAP) continue;
                for (var xx = -RobotConfiguration.HALF_WIDTH_ROBOT; xx < RobotConfiguration.HALF_WIDTH_ROBOT; xx++)
                {
                    if (p.CurrentPoint.X + xx > RobotConfiguration.WIDHT_MAP) continue;
                    //è l'immagine convertita in array e contiene le informazioni sulla mappa
                    if (wallInMap[p.CurrentPoint.X + xx, p.CurrentPoint.Y + yy])
                    {
                        return false;
                    }
                    //sono gli ostacoli incontrati durante il percorso 
                    if (obstacleInMap[p.CurrentPoint.X + xx, p.CurrentPoint.Y + yy])
                    {
                        return false;
                    }
                }
            }

            //sono gli ostacoli fissi che l'utente può aggiungere invisble wall
            if (helperInvisbleWall != null && helperInvisbleWall.Find(new RPoint(p.CurrentPoint.X, p.CurrentPoint.Y)))
            {
                return false;
            }

            //TODO
            //FakeObstacle fakeObs = new DalFakeObstacle().GetFakeObstacle((int)p.CurrentPoint.X, (int)p.CurrentPoint.Y);
            //if (fakeObs != null) return false;

            var aW = astarWeight.FirstOrDefault(c => p.CurrentPoint.X == c.X &&
                                                    p.CurrentPoint.Y == c.Y);
            p.CurrentPoint.w = aW == null ? 0 : aW.W;

            return true;
        }

        public double CalculateScore(Path p)
        {
            double manhattanDistance = Math.Abs(End.CurrentPoint.X - p.CurrentPoint.X)
                    + Math.Abs(End.CurrentPoint.Y - p.CurrentPoint.Y);
            return manhattanDistance;
        }

        public void ObstacleEncountered(RPoint point)
        {
            this.SetWeight(point, true);
            obstacleInMap[point.X, point.Y] = true;
        }

        public void ClearObstacle()
        {
            obstacleInMap = new bool[1000, 1000];
        }

        public void SetWeightPoints(IEnumerable<RPoint> points, bool increment)
        {
            foreach (var p in points)
            {
                var weight = astarWeight.FirstOrDefault(c => c.X == p.X
                                                        && c.Y == p.Y);
                if (weight == null)
                {
                    weight = new Weight
                    {
                        X = p.X,
                        Y = p.Y,
                        W = 0
                    };
                    astarWeight.Add(weight);
                }
                var nextW = increment ? weight.W + RobotConfiguration.MIN_STEP_FOR_WEIGHTPATH : 0;
                weight.W = nextW;
            }
            // Serializza l'array modificato in una nuova stringa json
            string newJson = JsonConvert.SerializeObject(astarWeight);

            //// Scrivi la nuova stringa json su un file di testo
            File.WriteAllText("weight.json", newJson);
        }

        public void SetWeight(RPoint point, bool increment)
        {
            var weight = astarWeight.FirstOrDefault(c => c.X == point.X
                                                    && c.Y == point.Y);
            if (weight == null)
            {
                weight = new Weight
                {
                    X = point.X,
                    Y = point.Y,
                    W = 0
                };
                astarWeight.Add(weight);
            }
            var nextW = increment ? weight.W + RobotConfiguration.MIN_STEP_FOR_WEIGHTPATH : 0;
            weight.W = nextW;
            // Serializza l'array modificato in una nuova stringa json
            string newJson = JsonConvert.SerializeObject(astarWeight);

            //// Scrivi la nuova stringa json su un file di testo
            File.WriteAllText("weight.json", newJson);
        }

        public void ObstacleAdd(RPoint point)
        {
            obstacleInMap[point.X, point.Y] = true;
        }

        public bool IsObstacle(RPoint point)
        {
            return obstacleInMap[point.X, point.Y];
        }

        public bool[,] GetObstacleInMap()
        {
            return obstacleInMap;
        }

        public bool IsWall(RPoint point, int tolerance)
        {

            // Calcolare i limiti del quadrato di tolleranza
            int minX = Math.Max(0, point.X - tolerance);
            int maxX = Math.Min(wallInMap.GetLength(0) - 1, point.X + tolerance);
            int minY = Math.Max(0, point.Y - tolerance);
            int maxY = Math.Min(wallInMap.GetLength(1) - 1, point.Y + tolerance);

            // Controllare i punti all'interno del quadrato
            for (int i = minX; i <= maxX; i++)
            {
                for (int j = minY; j <= maxY; j++)
                {
                    if (wallInMap[i, j])
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
