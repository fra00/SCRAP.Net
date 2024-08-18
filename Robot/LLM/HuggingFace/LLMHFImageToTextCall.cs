using MainRobot.Http;
using MainRobot.Robot.LLM.HuggingFace;
using MainRobot.Robot.NaturalLanguage;
using MainRobot.Robot.NaturalLanguage.Intent;
using MainRobot.Robot.NaturalLanguage.SpacyNet;
using Newtonsoft.Json;

namespace MainRobot.Robot.ActionExec
{
    public class LLMHFImageToTextCall
    {

        private string UrlLLM = "https://api-inference.huggingface.co/models/";
        private string modelLLM = "microsoft/git-large-coco";
        private string token = "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";
        private string prompt = "{{sentence}}";

        private string modelLLM1 = "meta-llama/Meta-Llama-3-8B-Instruct";
        private string prompt1 = "traduci in italiano questa frase \"{{sentence}}\" Risposta:";

        private LLLMApiCall lllMApiCall { get; set; }
        private LLLMApiCall lllMApiCall1 { get; set; }

        public LLMHFImageToTextCall()
        {
            lllMApiCall = new LLLMApiCall(UrlLLM, modelLLM, token, prompt);
            lllMApiCall1 = new LLLMApiCall(UrlLLM, modelLLM1, token, prompt1);
        }

        public async Task<string?> ProcessImage(string image)
        {
            if (string.IsNullOrEmpty(image))
                return "Non c'è niente da vedere";

            var response = await lllMApiCall.CallImage(image);
            if (response == null) return "non so come descrivere quello che vedo";
            try
            {
                var data = JsonConvert.DeserializeObject<List<GeneratedText>>(response);
                if (data == null) return "non riesco a eleborare quello che vedo";
                var resp = data.FirstOrDefault();
                if (resp == null) return "non so dirti quello che vedo";

                var translatedCall = await lllMApiCall1.CallPrompt(resp.generated_text);
                var dataTr = JsonConvert.DeserializeObject<List<GeneratedText>>(translatedCall);
                var translated = dataTr.FirstOrDefault()?.generated_text??"";
                var foo = translated.Split("Risposta:")[1].Split('\n')[0];
                return foo;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "ho qualche problema alla vista penso";
            }

        }
    }
}
