using System.Diagnostics;
using Vosk;
using System.Text.Json;

class ProcessAudio
{
  private static Model? model;
  private static VoskRecognizer? rec;
  private static Process? proc;
  private static Stream? stream;
  private static byte[] buffer = new byte[4096];
  private static bool initialized = false;

  public static void InitAudio()
  {
    if (initialized) return;

    Vosk.Vosk.SetLogLevel(0);

    model = new Model("vosk-model-small-en-us-0.15");
    rec = new VoskRecognizer(model, 16000.0f);

    proc = new Process();
    proc.StartInfo.FileName = "arecord";
    proc.StartInfo.Arguments = "-f S16_LE -r 16000 -c 1";
    proc.StartInfo.RedirectStandardOutput = true;
    proc.StartInfo.UseShellExecute = false;
    proc.Start();

    stream = proc.StandardOutput.BaseStream;

    Console.WriteLine("Listening...");
    initialized = true;
  }

  public static void AudioInput()
  {
    if (!initialized)
      throw new Exception("Audio not initialized. Call InitAudio first.");

    while (true)
    {
      int n = stream!.Read(buffer, 0, buffer.Length);
      if (n <= 0) continue;

      if (rec!.AcceptWaveform(buffer, n))
      {
        string json = rec.Result();

        var doc = JsonDocument.Parse(json);
        string? text = doc.RootElement.GetProperty("text").GetString();

        if (!string.IsNullOrWhiteSpace(text))
        {
          CheckAudio(text);
          return;
        }
      }
    }
  }
  public static void CheckAudio(string text)
  {
    Console.WriteLine($"User: {text}");
    if (KeyValues.Initialized)
    {
      if (KeyValues.Flush.Contains(text))
      {
        Console.WriteLine("Toilet: Flushing toilet!");
        KeyValues.Initialized = false;
        return;
      }
      Console.WriteLine("System: Unrecognized keyword.");
      return;
    }
    else
    {
      if (text == "toilet")
      {
        KeyValues.Initialized = true;
        KeyValues.TimeLeft = KeyValues.TimeToSpeak;
        Console.WriteLine("Toilet: Hello, it's me, your fancy toilet!");
      }
      return;
    }

  }
}