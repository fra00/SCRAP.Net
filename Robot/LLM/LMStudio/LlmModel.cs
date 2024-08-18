using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MainRobot.Robot.ActionExec.LLM
{
    /*
     * {
    "id": "chatcmpl-c732jda1yhlrlf9rjkg3k",
    "object": "chat.completion",
    "created": 1719081277,
    "model": "lmstudio-community/Meta-Llama-3-8B-Instruct-GGUF/Meta-Llama-3-8B-Instruct-Q4_K_M.gguf",
    "choices": [
        {
            "index": 0,
            "message": {
                "role": "assistant",
                "content": "ASK"
            },
            "finish_reason": "stop"
        }
    ],
    "usage": {
        "prompt_tokens": 369,
        "completion_tokens": 1,
        "total_tokens": 370
    }
}*/
    public class LlmStudioResponse
    {
        public string id { get; set; }
        //public string object { get; set; }
        public double created { get; set; }
        public string model { get; set; }

        public List<LlmStudioResponseChoiche> choices { get; set; }
    }

    public class LlmStudioResponseChoiche
    {
        public int index { get; set; }
        public LlmStudioResponseMessage message { get; set; }
        public string finish_reason { get; set; }

    }
    public class LlmStudioResponseMessage
    {
        public string role { get; set; }
        public string content { get; set; }

    }
}
