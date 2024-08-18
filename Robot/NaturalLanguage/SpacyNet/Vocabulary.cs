using MainRobot.Robot.NaturalLanguage;

namespace MainRobot.NaturalLanguage.SpacyNet
{
    public static class Vocabulary
    {
        public static Dictionary<string, string> Intent = new Dictionary<string, string> {
            { "muoviti",IntentDefinition.GOTO },
            { "vai",IntentDefinition.GOTO },
            { "torna",IntentDefinition.GOTO },
            { "spostati",IntentDefinition.GOTO },
            { "vieni",IntentDefinition.GOTO },
            { "ripeti",IntentDefinition.REPEAT}
        };
    }
}
