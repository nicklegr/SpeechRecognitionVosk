using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using Vosk;

namespace SpeechRecognitionVosk
{
    class Token
    {
        public double Conf { get; set; }
        public double End { get; set; }
        public double Start { get; set; }
        public string Word { get; set; }
    }

    class RecognitionResult
    {
        public Token[] Result { get; set; }
        public string Text { get; set; }
    }

    class Program
    {
        static string SecondsToString(double seconds)
        {
            long tick = (long)(seconds * 1000 * 1000 * 10); // 100-nanosecond units
            var timespan = new TimeSpan(tick);
            return $"{timespan.Hours:D2}:{timespan.Minutes:D2}:{timespan.Seconds:D2}.{timespan.Milliseconds:D3}";
        }

        public static void DemoBytes(Model model)
        {
            // Demo byte buffer
            VoskRecognizer rec = new VoskRecognizer(model, 16000.0f);
            rec.SetMaxAlternatives(0);
            rec.SetWords(true);

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true,
            };

            // 入力ファイルはたいていの動画から下記のコマンドで変換できる
            // ffmpeg -hide_banner -i test.mp4 -ar 16000 -ac 1 -f s16le test.wav
            using (var source = File.OpenRead("test.wav"))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (rec.AcceptWaveform(buffer, bytesRead))
                    {
                        string result = rec.Result();
                        // Console.WriteLine(result);

                        var json = JsonSerializer.Deserialize<RecognitionResult>(result, options);
                        if (json.Result != null && json.Result.Length >= 1)
                        {
                            var timestamp = SecondsToString(json.Result[0].Start);
                            var text = json.Text.Replace(" ", "");
                            Console.WriteLine($"{timestamp}: {text}");
                        }
                    }
                    else
                    {
                        // Console.WriteLine(rec.PartialResult());
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
