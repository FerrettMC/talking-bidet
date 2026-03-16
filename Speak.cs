using System.Diagnostics;

public static class Speaking
{
  private const string ModelPath = "/home/ferret/piper_voices/en_US-amy-medium/en_US-amy-medium.onnx";
  private const string OutputFile = "piper_out.wav";

  public static async Task Speak(string text)
  {
    KeyValues.isSpeaking = true;
    // Generate speech
    var psi = new ProcessStartInfo
    {
      FileName = "piper-tts",
      Arguments = $"--model \"{ModelPath}\" \"{text}\" --output-file \"{OutputFile}\"",
      UseShellExecute = false,
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      CreateNoWindow = true
    };

    var p = Process.Start(psi);
    p?.WaitForExit();

    // Check if Piper actually created the file
    if (!File.Exists(OutputFile))
    {
      Console.WriteLine("Piper did not generate audio!");
      KeyValues.isSpeaking = false;
      return;
    }

    // Now play the audio

    var play = new ProcessStartInfo
    {
      FileName = "aplay",
      Arguments = OutputFile,
      UseShellExecute = false,
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      CreateNoWindow = true
    };

    var playProcess = Process.Start(play);


    // Wait for playback to finish
    await Task.Run(() => playProcess?.WaitForExit());
    await Task.Delay(150);
    // Now it's safe to listen again
    KeyValues.isSpeaking = false;
    ProcessAudio.InitAudio();
  }
}
