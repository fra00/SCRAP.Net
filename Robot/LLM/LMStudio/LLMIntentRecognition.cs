using MainRobot.Robot.NaturalLanguage;
using MainRobot.Robot.NaturalLanguage.SpacyNet;

namespace MainRobot.Robot.ActionExec.LLM
{
    public class LLMIntentRecognition : IIntentRecognition
    {
        private ILLMRestCall llmRestCall { get; set; }

        public LLMIntentRecognition(ILLMRestCall llmRestCall)
        {
            this.llmRestCall = llmRestCall;
        }
        public void Init()
        {
            return;
        }

        public async Task<List<IntentData>> ProcessIntent(string sentence)
        {
            var intent = await llmRestCall.ProcessIntent(sentence);
            if (intent != null) {
                foreach(var c in intent) {
                    if (IntentDefinition.ASK == c.Intent)
                    {
                        var r = await llmRestCall.ProcessIntentASK(sentence);
                        c.Data = r;
                    }
                    if (IntentDefinition.GOTO == c.Intent)
                    {
                        var r = await llmRestCall.ProcessIntentGOTO(sentence);
                        c.Data = r;
                    }
                }
                return intent;
            }
            return new List<IntentData>();
        }
    }
}
