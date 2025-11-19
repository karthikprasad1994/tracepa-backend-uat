using System.Diagnostics;
using Whisper.net;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;

namespace TracePA_Agent_API.Services
{
    public class VideoService
    {
        private readonly string _baseDir;
        private readonly string _voice;
        private readonly WhisperFactory _whisperFactory;

        public VideoService(IConfiguration config)
        {
            _baseDir = config["AppSettings:BaseDir"]!;
            _voice = config["AppSettings:Voice"] ?? "en-US-GuyNeural";

            var modelPath = Path.Combine(AppContext.BaseDirectory, "ggml-tiny.bin");
            if (!File.Exists(modelPath))
                throw new FileNotFoundException($"Whisper model file not found at {modelPath}");

            _whisperFactory = WhisperFactory.FromPath(modelPath);

            // Ensure directories exist
            Directory.CreateDirectory(Path.Combine(_baseDir, "temp"));
            Directory.CreateDirectory(Path.Combine(_baseDir, "outputs"));
        }

        public async Task<string> ProcessVideoAsync(string videoPath)
        {
            string filename = Path.GetFileNameWithoutExtension(videoPath);
            string tempDir = Path.Combine(_baseDir, "temp");
            string outputDir = Path.Combine(_baseDir, "outputs");

            string audioPath = Path.Combine(tempDir, $"{filename}.wav");
            string ttsPath = Path.Combine(outputDir, $"{filename}_tts.wav");
            string finalPath = Path.Combine(outputDir, $"{filename}_final.mp4");

            Console.WriteLine($"🎬 Processing: {filename}");

            // 1️⃣ Extract audio
            RunFFmpeg($"-y -i \"{videoPath}\" -ac 1 -ar 16000 \"{audioPath}\"");

            if (!File.Exists(audioPath))
                throw new FileNotFoundException($"Audio extraction failed: {audioPath}");

            // 2️⃣ Transcribe with Whisper.NET
            string transcript = await TranscribeAsync(audioPath);
            Console.WriteLine($"📝 Transcript: {transcript}");

            // 3️⃣ Generate TTS using Azure Speech
            await GenerateTTSAsync(transcript, ttsPath);

            // 4️⃣ Merge video + new audio
            RunFFmpeg($"-y -i \"{videoPath}\" -i \"{ttsPath}\" -c:v copy -map 0:v:0 -map 1:a:0 \"{finalPath}\"");

            Console.WriteLine($"✅ Final saved: {finalPath}");
            return finalPath;
        }

        private void RunFFmpeg(string args)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = args,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();
        }

        private async Task<string> TranscribeAsync(string audioPath)
        {
            await using var processor = _whisperFactory.CreateBuilder().Build();
            await using var fileStream = File.OpenRead(audioPath);
            var text = new System.Text.StringBuilder();

            await foreach (var segment in processor.ProcessAsync(fileStream))
                text.Append(segment.Text);

            return text.ToString().Trim();
        }

        private async Task GenerateTTSAsync(string text, string outputPath)
        {
            var config = SpeechConfig.FromSubscription("<YOUR_AZURE_KEY>", "<YOUR_REGION>");
            config.SpeechSynthesisVoiceName = _voice;

            using var audioConfig = AudioConfig.FromWavFileOutput(outputPath);
            using var synthesizer = new SpeechSynthesizer(config, audioConfig);
            await synthesizer.SpeakTextAsync(text);
        }
    }
}
