using MainRobot.Robot.ActionExec;
using MainRobot.Robot.NaturalLanguage;
using MainRobot.Robot.NaturalLanguage.SpacyNet;

namespace MainRobot.Robot.LLM.HuggingFace
{
    public class LLMHuggingFace : IIntentRecognition
    {
        private ILLMRestCall llmRestCall { get; set; }

        public LLMHuggingFace(ILLMRestCall llmRestCall)
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
            if (intent != null)
            {
                return intent;
            }
            return new List<IntentData>();
        }
    }
}
