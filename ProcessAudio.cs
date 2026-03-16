using System.Diagnostics;
using Vosk;
using System.Text.Json;

class ProcessAudio
{
  private static Model? model;
  private static VoskRecognizer? rec;
  private static Process? proc;
  private static Stream? stream;
  private static readonly byte[] buffer = new byte[4096];
  private static bool initialized = false;

  public static void InitAudio()
  {
    if (initialized) return;

    Vosk.Vosk.SetLogLevel(-1);

    model = new Model("vosk-model-small-en-us-0.15");
    rec = new VoskRecognizer(model, 16000.0f);

    proc = new Process();
    proc.StartInfo.FileName = "arecord";
    proc.StartInfo.Arguments = "-f S16_LE -r 16000 -c 1";
    proc.StartInfo.RedirectStandardOutput = true;
    proc.StartInfo.RedirectStandardError = true;
    proc.StartInfo.UseShellExecute = false;
    proc.Start();

    stream = proc.StandardOutput.BaseStream;

    initialized = true;
  }

  public static void AudioInput()
  {
    if (!initialized)
      throw new Exception("Audio not initialized. Call InitAudio first.");

    while (true)
    {
      if (!KeyValues.isSpeaking && stream != null)
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
            continue;
          }
        }
      }
      else
      {
        StopAudio();
      }
    }
  }

  public static async void CheckAudio(string text)
  {
    if (KeyValues.isSpeaking) return;
    Console.WriteLine($"User: {text}");
    if (KeyValues.Initialized)
    {
      // Flush
      if (KeyValues.Flush.Contains(text))
      {
        await Speaking.Speak("Flushing toilet");
        Console.WriteLine("Toilet: Flushing toilet!");
        KeyValues.Initialized = false;
        return;
      }
      if (!KeyValues.BidetOn)
      {
        // Bidet on
        for (int i = 0; i < KeyValues.Bidet.Length; i++)
        {
          for (int x = 0; x < KeyValues.On.Length; x++)
          {
            string testingText = $"{KeyValues.Bidet[i]} {KeyValues.On[x]}";
            if (testingText == text)
            {
              await Speaking.Speak("Activating bidet");
              Console.WriteLine("Toilet: Activating bidet!");
              KeyValues.Initialized = false;
              KeyValues.BidetOn = true;
              return;
            }
          }
        }
      }
      if (KeyValues.BidetOn)
      {
        // Bidet off
        for (int i = 0; i < KeyValues.Bidet.Length; i++)
        {
          for (int x = 0; x < KeyValues.Off.Length; x++)
          {
            string testingText = $"{KeyValues.Bidet[i]} {KeyValues.Off[x]}";
            if (testingText == text)
            {
              await Speaking.Speak("Deactivating bidet");
              Console.WriteLine("Toilet: Deactivating bidet!");
              KeyValues.Initialized = false;
              KeyValues.BidetOn = false;
              return;
            }
          }
        }
      }
      Console.WriteLine("System: Unrecognized keyword.");
      KeyValues.TimeLeft = KeyValues.TimeToSpeak;
      return;
    }
    else
    {
      if (text == "toilet")
      {
        KeyValues.Initialized = true;
        KeyValues.TimeLeft = KeyValues.TimeToSpeak;
        var rand = new Random();
        string randomGreeting = KeyValues.Greetings[rand.Next(KeyValues.Greetings.Length)];
        await Speaking.Speak(randomGreeting);
        Console.WriteLine($"Toilet: {randomGreeting}");
      }
      return;
    }

  }
  public static void StopAudio()
  {
    try
    {
      proc?.Kill();
      proc?.WaitForExit();
      initialized = false;
    }
    catch { }
  }

}