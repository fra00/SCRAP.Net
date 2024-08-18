namespace MainRobot.Robot.ActionExec
{

    public class LLMHFInput
    {
        public string inputs { get; set; }
        public LLMHFParameters parameters { get; set; }
    }

    public class LLMHFInputImage
    {
        public InputsImage inputs { get; set; }
    }

    public class InputsImage
    {
        public string image { get; set; }
    }

    public class GeneratedText
    {
        public string generated_text { get; set; }
    }
    public class LLMHFParameters
    {
        public int max_new_tokens { get; set; }
        public double temperature { get; set; }
        public double top_p { get; set; }
        public bool do_sample { get; set; }
    }

    public class ResultAction
    {
        public string action { get; set; }
    }
}
