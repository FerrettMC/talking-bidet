## A simple voice recognition project for a coworker. He wanted a talking bidet.

# Offline Speech Recognition (C# + Vosk)

## Overview

This project is a **real-time offline speech recognition system** written in C#.
It uses the Vosk speech recognition engine and Linux audio tools to convert microphone input into text without any internet connection.

The application continuously listens to the system microphone and outputs recognized speech in real time.

---

## Features

* Real-time speech recognition
* Fully offline (no cloud APIs)
* Lightweight Vosk model support
* Linux-native audio capture using `arecord`

---

## Requirements

### System Dependencies (Linux)

You must install:

```bash
sudo pacman -S alsa-utils
```

This provides:

* `arecord` → microphone capture
* `aplay` → audio playback testing

### .NET

Requires:

* .NET 8 or newer

Install from:
https://dotnet.microsoft.com

### NuGet Packages

This project uses:

* Vosk

Install with:

```bash
dotnet add package Vosk
```

---

## Speech Model

This project requires a Vosk speech model.

Download:

https://alphacephei.com/vosk/models

Recommended model:

```
vosk-model-small-en-us-0.15
```

Extract the model folder into the project root.

IMPORTANT:
Speech models are **not included in the repository** and are ignored via `.gitignore` because they are large binary assets.

---

## Running the Program

1. Ensure microphone works:

```bash
arecord -f S16_LE -r 16000 -c 1 test.wav
aplay test.wav
```

2. Run the application:

```bash
dotnet run
```

3. Speak into the microphone.

Recognized speech will be printed to the console.

---

## Architecture

Audio pipeline:

```
Microphone → ALSA → arecord → C# Stream → Vosk → Text Output
```

Key components:

* ALSA handles hardware audio capture
* `arecord` streams raw PCM audio
* C# reads audio stream in real time
* Vosk performs local speech recognition

---

## Notes

* Designed for Linux (tested on Arch-based systems)
* Offline recognition accuracy depends on chosen model
* Larger models improve accuracy but increase RAM usage

---

## Future Improvements

Potential extensions:

* Add to a toilet or something

---

## License

Licenses are for nerds.
