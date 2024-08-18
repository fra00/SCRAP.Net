using Catalyst;
using MainRobot.Robot.NaturalLanguage;
using MainRobot.Robot.NaturalLanguage.SpacyNet;
using Mosaik.Core;

namespace MainRobot.NaturalLanguage.SpacyNet
{
    public class IntentRecognition : IIntentRecognition
    {
        private Pipeline nlp = null;

        private string substringBounds(string sentence, int[] bounds)
        {
            return sentence.Substring(bounds[0], bounds[1] - bounds[0] + 1);
        }

        public IntentRecognition()
        {

        }

        public async void Init()
        {
            Catalyst.Models.Italian.Register();
            Storage.Current = new DiskStorage("catalyst-models");
            nlp = await Pipeline.ForAsync(Language.Italian);
        }

        public async Task<List<IntentData>> ProcessIntent(string sentence)
        {
            var doc = new Document(sentence, Language.Italian);
            nlp.ProcessSingle(doc);
            List<IntentData>? listIntent = new List<IntentData>();

            var data = "";
            string intent = null;

            //prova
            if (sentence.ToLower().StartsWith("ripeti"))
            {
                listIntent.Add(new IntentData
                {
                    Intent = "REPEAT",
                    Data = sentence.ToLower().Replace("ripeti", "")
                });
            }
            else
                foreach (var token in doc.TokensData)
                {
                    //verifico il verbo
                    foreach (var t in token)
                    {
                        if (t.Tag == PartOfSpeech.VERB)
                        {
                            var verb = substringBounds(sentence, t.Bounds);
                            //association verb / intent
                            Vocabulary.Intent.TryGetValue(verb, out intent);
                            if (string.IsNullOrEmpty(intent))
                            {
                                continue;
                            }
                        }
                        var d = parseData(sentence, intent, t);
                        if (d != null)
                        {
                            data = d;
                        }

                        if (!string.IsNullOrEmpty(intent) && !string.IsNullOrEmpty(data))
                        {
                            listIntent.Add(new IntentData
                            {
                                Intent = intent,
                                Data = data
                            });
                            Console.WriteLine("Intento " + intent);
                            Console.WriteLine("Nome " + data);
                            intent = "";
                            data = "";
                        }
                    }
                }
            return listIntent;
        }

        private string? parseData(string sentence, string? intent, TokenData t)
        {
            switch (intent)
            {
                case "GOTO":
                    return parseDataGoto(sentence, t);
                case "REPEAT":
                    return parseDataRepeat(sentence, t);
            }
            return null;
        }

        private string? parseDataRepeat(string sentence, TokenData t)
        {
            var f = sentence.Replace("ripeti", "");
            return f;
        }

        private string? parseDataGoto(string sentence, TokenData t)
        {
            if (t.Tag == PartOfSpeech.NOUN)
            {
                return substringBounds(sentence, t.Bounds);
            }
            return null;
        }
    }
}
