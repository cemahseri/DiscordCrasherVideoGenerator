# DiscordCrasherVideoGenerator
You see those crasher videos going around, such as Trava_discord.mp4, Indian tech support saying "Your computer has virus" or Mario saying "Nice computer you have. Can I have it?". You want to make one? Great. Keep reading.

# Prerequirement
Be sure that you have installed .NET 5 runtime.

### Windows
Download [Windows 32-bit installer](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-5.0.8-windows-x86-installer) or [Windows 64-bit installer](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-5.0.8-windows-x64-installer), corresponding to your version.

### Linux
Just install it from your package manager.

### macOS
If you really use macOS, go fuck yourself. Haha, just kidding. Just download [macOS 64-bit installer](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-5.0.8-macos-x64-installer). And seriously, go fuck yourself.

Don't use a video that's too short. Like, use 8 or 9 seconds at least.
Also, be sure that the file's name is full of alphanumeric letters. Don't put any space, special character, number, etc.

# Usage
Just drag-and-drop the video you want to make crasher, to the executable file.
Or alternatively, you can specify the path of your input and/or output file from the command line.
```
DiscordCrasherVideoGenerator.exe C:\path\to\your\mom\input.mp4
DiscordCrasherVideoGenerator.exe C:\path\to\your\mom\input.mp4 C:\path\to\your\mom\output.mp4
```

# How this black magic works?
First of all, this is not something directly related to Discord. Discord's desktop allication is developed with Electron, which uses Chromium for rendering engine.
Chromium's video playback is really awful. Actually it's the problem. If you download those old crasher videos and play it on your VLC or FFplay, you will notice that it'll not be able to crash the software.
So, why Chromium fails to play the video? If you remember, we just divided our video into two parts. First part was from 0 second to 0.1 second. The second part was the rest. And as you will remember, we changed second part's pixel format to YUV444p. Then we just combine them.
When Chromium opens the video, it only checks for the first part.  It's YUV420p, which your CPU and GPU can decode. Then Chromium decides to decode the video on your hardware. But after 0.1 second, YUV444p part stars. Chromium doesn't support changing resolution or pixel format while playing the video. So this is the first killer.
Second bad thing is, it's really high likely that your hardware doesn't support decoding YUV444p. But Chromium decided to decode the video on your hardware anyways. So Chromium fails to decode the video, which results in freezing the whole Discord desktop application.

# How can I avoid crasher videos?
Open Discord, go to User Settings, then navigate to Advanced. You will see an option named "Hardware Acceleration". Disable it and restart your Discord. Now, all of the videos you watch will be decoded by the software. Ta daah! Now you have a talisman for black sorcery!
