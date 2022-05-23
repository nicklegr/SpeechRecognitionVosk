using System;
using System.IO;
using Vosk;

namespace SpeechRecognitionVosk
{
    class Program
    {
        public static void DemoBytes(Model model)
        {
            // Demo byte buffer
            VoskRecognizer rec = new VoskRecognizer(model, 16000.0f);
            rec.SetMaxAlternatives(0);
            rec.SetWords(true);
            using (var source = File.OpenRead("test.wav"))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (rec.AcceptWaveform(buffer, bytesRead))
                    {
                        Console.WriteLine(rec.Result());
                    }
                    else
                    {
                        Console.WriteLine(rec.PartialResult());
                    }
                }
            }
            Console.WriteLine(rec.FinalResult());
        }

        static void Main(string[] args)
        {
            // You can set to -1 to disable logging messages
            Vosk.Vosk.SetLogLevel(0);

            Model model = new Model("model");
            DemoBytes(model);
        }
    }
}
