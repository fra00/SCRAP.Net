using MainRobot.Http;
using MainRobot.Robot.ActionExec.LLM;
using MainRobot.Robot.NaturalLanguage.SpacyNet;
using Newtonsoft.Json;

namespace MainRobot.Robot.ActionExec
{
    public class LLMRestCall :  ILLMRestCall
    {
        public void Init()
        {

        }

        public async Task<List<IntentData>?> ProcessIntent(string sentence)
        {
            var client = new RestApiClient(Configuration.HTTP_URL_BASE_LLM);
            var r = await client.SendRequestAsync("POST", "completions", new
            {
                model= Configuration.HTTP_URL_LLM,
                messages = new List<object>
                {
                    new { role= "user", content= "INTENT " + sentence }
                },
                temperature = 0.7,
                max_tokens = -1,
                stream = false
            });
            var content = await r.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<LlmStudioResponse>(content);

            var result = new List<IntentData> {
                new IntentData {
                    Intent = data?.choices?.FirstOrDefault()?.message?.content,
                    Data =""
                }
            };
            return result;
        }

        public async Task<string> ProcessIntentASK(string sentence)
        {
            var client = new RestApiClient(Configuration.HTTP_URL_BASE_LLM);
            var r = await client.SendRequestAsync("POST", "completions", new
            {
                model = Configuration.HTTP_URL_LLM,
                messages = new List<object>
                {
                    new { role= "user", content= "ASK " + sentence }
                },
                temperature = 0.7,
                max_tokens = -1,
                stream = false
            });
            var content = await r.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<LlmStudioResponse>(content);

            return data?.choices?.FirstOrDefault()?.message?.content;
        }

        public async Task<string> ProcessIntentGOTO(string sentence)
        {
            var client = new RestApiClient(Configuration.HTTP_URL_BASE_LLM);
            var r = await client.SendRequestAsync("POST", "completions", new
            {
                model = Configuration.HTTP_URL_LLM,
                messages = new List<object>
                {
                    new { role= "user", content= "GOTO " + sentence }
                },
                temperature = 0.7,
                max_tokens = -1,
                stream = false
            });
            var content = await r.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<LlmStudioResponse>(content);

            return data?.choices?.FirstOrDefault()?.message?.content;
        }
    }
}
