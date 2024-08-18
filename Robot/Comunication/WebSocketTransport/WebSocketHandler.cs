using MainRobot.Robot.ActionExec;
using MainRobot.Robot.Navigation.Helpers;
using MainRobot.Robot.Navigation.Interface;
using MainRobot.TTS;

namespace MainRobot.Robot.Comunication.WebSocketTransport
{
    public class WebSocketHandler
    {
        private INavigation navigation;
        private IMovement movement;
        private INavigationMover navigationMover;
        private IHelperInvisibleWall invisibleWall;
        private IActionExec actionExec;
        private ITextToSpeach tts;

        public WebSocketHandler(INavigation navigation,
            INavigationMover navigationMover,
            IMovement movement,
            IHelperInvisibleWall invisibleWall,
            IActionExec actionExec,
            ITextToSpeach tts)
        {
            this.navigation = navigation;
            this.navigationMover = navigationMover;
            this.invisibleWall = invisibleWall;
            this.movement = movement;
            this.actionExec = actionExec;
            this.tts = tts;
        }

        public async Task<WebSocketOutputData> handler(WebSocketOutputData input)
        {
            switch (input.command)
            {
                case "SENTENCE":
                    string sentence = input.command.ToString();

                    await actionExec.SentenceExec(sentence);
                    // Return a success message
                    return input;
            }
            return new WebSocketOutputData();
        }
    }
}
