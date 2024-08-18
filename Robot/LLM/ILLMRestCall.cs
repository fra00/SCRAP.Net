using MainRobot.Robot.NaturalLanguage.SpacyNet;

namespace MainRobot.Robot.ActionExec
{
    public interface ILLMRestCall {
        Task<string?> ProcessIntentASK(string sentence);
        Task<List<IntentData>> ProcessIntent(string sentence);
        Task<string?> ProcessIntentGOTO(string sentence);
    }
}
