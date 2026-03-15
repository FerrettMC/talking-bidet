class Program
{
  static async Task Main()
  {
    ProcessAudio.InitAudio();

    _ = TurnOffInitialized();   // run async in background

    while (true)
    {
      ProcessAudio.AudioInput();
    }
  }

  static async Task TurnOffInitialized()
  {
    while (true)
    {
      if (KeyValues.TimeLeft > 0)
      {
        KeyValues.TimeLeft--;
      }
      else if (KeyValues.TimeLeft == 0 && KeyValues.Initialized)
      {
        KeyValues.Initialized = false;
        KeyValues.TimeLeft = -1;
        Console.WriteLine("Uninitialized due to inactivity");
      }
      await Task.Delay(1000);
    }
  }
}