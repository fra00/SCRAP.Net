using MainRobot.Robot.Comunication.WebSocketTransport;
using MainRobot.Robot.NaturalLanguage;
using MainRobot.Robot.Navigation.Interface;
using MainRobot.Robot.Room;
using MainRobot.TTS;

namespace MainRobot.Robot.ActionExec
{
    public class ActionExec : IActionExec
    {
        private IIntentRecognition intentRecognition;
        private IRoomInfo roomInfo;
        private INavigation navigation;
        private ITextToSpeach textToSpeach;
        private IMovement movement;
        private IWebSocketCommand webSocketCommand;


        public ActionExec(IIntentRecognition intentRecognition,
            IRoomInfo roomInfo,
            INavigation navigation,
            IMovement movement,
            ITextToSpeach textToSpeach,
            IWebSocketCommand webSocketCommand)
        {
            this.intentRecognition = intentRecognition;
            this.roomInfo = roomInfo;
            this.navigation = navigation;
            this.movement = movement;
            this.textToSpeach = textToSpeach;
            this.webSocketCommand = webSocketCommand;
        }

        public async Task SentenceExec(string sentence)
        {

            if (sentence.Replace("INTENT ", "").ToLower().Replace("\n", "").TrimStart() == "guarda avanti")
            {
                var img = await webSocketCommand.Looks();
                var imageToText = await (new LLMHFImageToTextCall().ProcessImage(img));
                textToSpeach.TalkAsync(imageToText);
                return;
            }

            if (sentence.Replace("INTENT ", "").ToLower().Replace("\n", "").TrimStart() == "fai sì con la testa")
            {
                await movement.RotateXCell(110);
                await movement.RotateXCell(90);
                await movement.RotateXCell(110);
                await movement.RotateXCell(90);
                return;
            }

            if (sentence.Replace("INTENT ", "").ToLower().Replace("\n", "").TrimStart() == "fai no con la testa")
            {
                await movement.RotateYCell(110);
                await movement.RotateYCell(70);
                await movement.RotateYCell(110);
                await movement.RotateYCell(90);
                return;
            }


            intentRecognition.Init();
            var intents = intentRecognition.ProcessIntent(sentence).Result;

            foreach (var i in intents)
            {
                if (i.Intent == IntentDefinition.GOTO)
                {
                    if (string.IsNullOrEmpty(i.Data))
                    {
                        textToSpeach.TalkAsync("Non ho capito la destinazione");
                        return;
                    }
                    var destPoint = roomInfo.GetPointRoom(i.Data);
                    if (destPoint == null)
                    {
                        textToSpeach.TalkAsync("Non ho capito la destinazione");
                        return;
                    }
                    await navigation.NavigateTo(destPoint);
                }
                if (i.Intent == IntentDefinition.REPEAT)
                {
                    textToSpeach.TalkAsync(i.Data);
                }
                if (i.Intent == IntentDefinition.ASK)
                {
                    textToSpeach.TalkAsync(i.Data);
                }
            }
        }
    }
}
