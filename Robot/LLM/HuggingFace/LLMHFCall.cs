using MainRobot.Http;
using MainRobot.Robot.NaturalLanguage;
using MainRobot.Robot.NaturalLanguage.Intent;
using MainRobot.Robot.NaturalLanguage.SpacyNet;
using Newtonsoft.Json;

namespace MainRobot.Robot.ActionExec
{
    public class LLMHFCall : ILLMRestCall
    {

        private string UrlLLM = "https://api-inference.huggingface.co/models/";
        private string modelLLM = "meta-llama/Meta-Llama-3-8B-Instruct";
        private string token = "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";
        private string prompt = "Sei un robot che si chiama Rottame e risponde solo in formato JSON. " +
            "Fornisci SOLO un singolo comando JSON valido e niente altro.\n" +
            "Comandi validi:\n" +
            "{\"action\":\"move\",\"direction\":\"forward\",\"distance\":INT}\n" +
            "{\"action\":\"move\",\"direction\":\"backward\",\"distance\":INT}\n" +
            "{\"action\":\"move\",\"direction\":\"left\",\"angle\":INT}\n" +
            "{\"action\":\"move\",\"direction\":\"right\",\"angle\":INT}\n" +
            "{\"action\":\"GOTO\",\"destination\":STRING}\n" +
            "{\"action\":\"speech\",\"text\":STRING}\n" +
            "Regole: Distanza (10,20,30...). Angolo (90,180,270). " +
            "Destinazioni: cucina, sala, bagno, ingresso, corridoio, studio, camera da letto,cameretta.\n\n" +
            "Adatta la lunghezza della risposta in base alla complessità della domanda:\n" +
            "- Per domande o calcoli, usa l'azione \"speech\" e fornisci la risposta come testo." +
            "Fornisci solo un singolo comando JSON senza ripetizioni e senza aggiungere altro testo. " +
            "Tutte le informazioni devono essere contenute all'interno del JSON. Usa solo le azioni valide elencate.\n\n" +
            "Umano: {{sentence}}\n\nJSON:";

        public void Init()
        {

        }

        public async Task<List<IntentData>?> ProcessIntent(string sentence)
        {
            var cleanSentence = sentence.TrimEnd(' ');
            if (string.IsNullOrEmpty(cleanSentence))
                cleanSentence = "voglio parlare con te";
            var client = new RestApiClient($"{UrlLLM}");
            client.addHeader("Authorization", $"Bearer {token}");
            var r = await client.SendRequestAsync("POST", modelLLM, new LLMHFInput
            {
                inputs = prompt.Replace("{{sentence}}" , cleanSentence),
                parameters = new LLMHFParameters
                {
                    max_new_tokens = 150,
                    temperature = 0.9,
                    top_p = 0.1,
                    do_sample = false
                }
            });
            var content = await r.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<List<GeneratedText>>(content);

            if (data != null && data.Any()) {
                var textResult = data?.FirstOrDefault();
                if (textResult != null)
                {
                    var splittedArray = textResult.generated_text.Split("JSON:");
                    var jsonResponse = splittedArray[1];
                    var json = jsonResponse.Split("}")[0] + "}";
                    ResultAction? action = JsonConvert.DeserializeObject<ResultAction>(json);
                    if (action == null) return null;
                    var intent = new IntentData();
                    switch (action.action.ToLower())
                    {
                        case "goto":
                            intent.Intent = IntentDefinition.GOTO;
                            intent.Data = JsonConvert.DeserializeObject<IntentGoto>(json)?.destination;
                            break;
                        case "move":
                            intent.Intent = IntentDefinition.MOVE;
                            break;
                        case "speech":
                            intent.Intent = IntentDefinition.ASK;
                            intent.Data = JsonConvert.DeserializeObject<IntentSpeech>(json)?.text;
                            break;
                    }
                    return new List<IntentData> { intent };
                }
            }
            return null;
        }

        public Task<string?>? ProcessIntentASK(string sentence)
        {
            return null;
        }

        public Task<string?>? ProcessIntentGOTO(string sentence)
        {
            return null;
        }
    }
}
