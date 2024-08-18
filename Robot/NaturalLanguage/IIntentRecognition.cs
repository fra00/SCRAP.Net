using MainRobot.Robot.NaturalLanguage.SpacyNet;

namespace MainRobot.Robot.NaturalLanguage
{
    public interface IIntentRecognition
    {
        void Init();
        Task<List<IntentData>> ProcessIntent(string sentence);
    }

    public static class IntentDefinition
    {
        public static string GOTO = "GOTO";
        public static string ASK = "ASK";
        public static string MOVE = "MOVE";
        public static string REPEAT = "REPEAT";
    }
}