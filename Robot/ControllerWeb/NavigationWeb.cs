using MainRobot.Http;
using MainRobot.Robot.ActionExec;
using MainRobot.Robot.Navigation.Helpers;
using MainRobot.Robot.Navigation.Interface;
using MainRobot.TTS;
using Newtonsoft.Json;

namespace MainRobot.Robot.ControllerWeb
{
    // Define a class for the navigation controller
    public class NavigationWebController
    {
        private INavigation navigation;
        private IMovement movement;
        private INavigationMover navigationMover;
        private IHelperInvisibleWall invisibleWall;
        private IActionExec actionExec;
        private ITextToSpeach tts;

        public NavigationWebController(INavigation navigation,
            INavigationMover navigationMover,
            IMovement movement,
            IHelperInvisibleWall invisibleWall,
            IActionExec actionExec,
            ITextToSpeach tts
            )
        {
            this.navigation = navigation;
            this.navigationMover = navigationMover;
            this.invisibleWall = invisibleWall;
            this.movement = movement;
            this.actionExec = actionExec;
            this.tts = tts;
        }

        /// <summary>
        /// Navigate to point
        /// </summary>
        /// <param name="data">
        /// int x,
        /// int y
        /// </param>
        /// <returns></returns>
        [HttpMethod("GET")]
        public object NavigateTo(Dictionary<string, object> data)
        {

            int x = int.Parse(data["x"].ToString());
            int y = int.Parse(data["y"].ToString());

            navigation.NavigateTo(new Common.RPoint(x, y));

            // Return a success message
            return new { message = "OK" };
        }

        /// <summary>
        /// set position of robot 
        /// </summary>
        /// <param name="data">
        /// int x,
        /// int y </param>
        /// <returns></returns>
        [HttpMethod("GET")]
        public object SetPosition(Dictionary<string, object> data)
        {

            int x = int.Parse(data["x"].ToString());
            int y = int.Parse(data["y"].ToString());

            navigationMover.UpdatePosition(new Common.RPoint(x, y), StatusRobot.CurrentAngle);

            // Return a success message
            return new { message = "OK" };
        }

        /// <summary>
        /// Add wall to point 
        /// </summary>
        /// <param name="data">
        /// int x,
        /// int y</param>
        /// <returns></returns>
        [HttpMethod("GET")]
        public object AddWall(Dictionary<string, object> data)
        {

            int x = int.Parse(data["x"].ToString());
            int y = int.Parse(data["y"].ToString());

            invisibleWall.Add(new Common.RPoint(x, y));
            invisibleWall.Save();

            // Return a success message
            return new { message = "OK" };
        }

        /// <summary>
        /// Remove wall at point
        /// </summary>
        /// <param name="data">
        /// int x,
        /// int y</param>
        /// <returns></returns>
        [HttpMethod("GET")]
        public object RemoveWall(Dictionary<string, object> data)
        {

            int x = int.Parse(data["x"].ToString());
            int y = int.Parse(data["y"].ToString());

            invisibleWall.Remove(new Common.RPoint(x, y));
            invisibleWall.Save();

            // Return a success message
            return new { message = "OK" };
        }

        /// <summary>
        /// novigate to recharge position
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpMethod("GET")]
        public object Recharge(Dictionary<string, object> data)
        {

            this.navigation.NavigateToRecharge();

            // Return a success message
            return new { message = "OK" };
        }


        [HttpMethod("GET")]
        public object ReadObstacleFromLidar(Dictionary<string, object> data)
        {
            navigation.ReadObstacleFromLidar();

            // Return a success message
            return new { message = "OK" };
        }

        [HttpMethod("GET")]
        public object ReadRawLidar(Dictionary<string, object> data)
        {
            IEnumerable<(int, float)>? r = navigation.ReadRawLidar().Result;
            string serialized = JsonConvert.SerializeObject(r);
            // Return a success message
            return new { message = "OK", data = serialized };
        }

        [HttpMethod("GET")]
        public object Fwd(Dictionary<string, object> data)
        {

            int distance = int.Parse(data["dist"].ToString());
            movement.EnableMoviment();
            movement.Forward(distance);

            // Return a success message
            return new { message = "OK" };
        }

        [HttpMethod("GET")]
        public object Bck(Dictionary<string, object> data)
        {

            int distance = int.Parse(data["dist"].ToString());
            movement.EnableMoviment();
            movement.Backward(distance);

            // Return a success message
            return new { message = "OK" };
        }

        [HttpMethod("GET")]
        public object Lft(Dictionary<string, object> data)
        {

            int angle = int.Parse(data["angle"].ToString());
            movement.EnableMoviment();
            movement.TurnLeft(0, angle
                );

            // Return a success message
            return new { message = "OK" };
        }

        [HttpMethod("GET")]
        public object Rht(Dictionary<string, object> data)
        {

            int angle = int.Parse(data["angle"].ToString());
            movement.EnableMoviment();
            movement.TurnRight(0, angle);

            // Return a success message
            return new { message = "OK" };
        }

        [HttpMethod("GET")]
        public object GetObstacleInMap(Dictionary<string, object> data)
        {
            var r = navigation.GetObstacleInMap();
            string serialized = JsonConvert.SerializeObject(r);
            // Return a success message
            return new { message = "OK", data = serialized };
        }

        [HttpMethod("GET")]
        public object SentenceAction(Dictionary<string, object> data)
        {

            string sentence = data["sentence"].ToString();

            actionExec.SentenceExec(sentence);
            // Return a success message
            return new { message = "OK" };
        }

        [HttpMethod("GET")]
        public object Talk(Dictionary<string, object> data)
        {
            string sentence = data["talk"].ToString();
            tts.TalkAsync(sentence);
            // Return a success message
            return new { message = "OK" };
        }


    }
}
