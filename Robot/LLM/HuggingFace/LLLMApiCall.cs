using MainRobot.Http;
using MainRobot.Robot.ActionExec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace MainRobot.Robot.LLM.HuggingFace
{
    public class LLLMApiCall
    {
        private string UrlLLM = "https://api-inference.huggingface.co/models/";
        private string modelLLM = "microsoft/git-large-coco";
        private string token = "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";
        private string prompt = "";
        public LLLMApiCall(string UrlLLM,
                string modelLLM,
                string token,
                string prompt)
        {
            this.UrlLLM = UrlLLM;
            this.modelLLM = modelLLM;
            this.token = token;
            this.prompt = prompt;
        }

        public async Task<string> CallPrompt(string sentence)
        {

            var client = new RestApiClient($"{UrlLLM}");
            client.addHeader("Authorization", $"Bearer {token}");
            var r = await client.SendRequestAsync("POST", modelLLM, new LLMHFInput
            {
                inputs = prompt.Replace("{{sentence}}", sentence),
                parameters = new LLMHFParameters
                {
                    max_new_tokens = 150,
                    temperature = 0.9,
                    top_p = 0.1,
                    do_sample = false
                }
            });
            var content = await r.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> CallImage(string sentence)
        {

            var client = new RestApiClient($"{UrlLLM}");
            client.addHeader("Authorization", $"Bearer {token}");
            var r = await client.SendRequestAsync("POST", modelLLM, new LLMHFInputImage
            {
                inputs = new InputsImage
                {
                    image = sentence
                }
            });
            var content = await r.Content.ReadAsStringAsync();
            return content;
        }
    }
}
