using System;
using System.Speech.Synthesis;

public class Tts
{
    // Create a new SpeechSynthesizer instance
    SpeechSynthesizer synth = new SpeechSynthesizer();

    // Configure the audio output
    synth.SetOutputToDefaultAudioDevice();

            // Speak a string synchronously
            synth.Speak("Hello, this is a demo of text to speech synthesis in .NET");

            // Speak a string asynchronously
            synth.SpeakAsync("This is an asynchronous speech");

            // Change the voice gender and age
            synth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);

            // Change the volume and rate
            synth.Volume = 80; // 0...100
            synth.Rate = -2; // -10...10

            // Speak another string with the new settings
            synth.Speak("This is a female voice with lower volume and rate");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
}