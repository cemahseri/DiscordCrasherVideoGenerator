# DiscordCrasherVideoGenerator
~~You see those crasher videos going around, such as Trava_discord.mp4, Indian tech support saying "Your computer has virus" or Mario saying "Hmm. Oh! Nice computer you got here. Can I have it?". You want to make one? Great. Keep reading.~~

Discord has patched crasher videos.

**Update:** However! There is still one way to break Discord compeletely and turning off hardware acceleration doesn't help at all. When I find some free time, I will update this program accordingly.

# Prerequirement
Be sure that you have installed .NET 5 runtime.

### Windows
Download the [Windows 32-bit installer](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-5.0.8-windows-x86-installer) or the [Windows 64-bit installer](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-5.0.8-windows-x64-installer), corresponding to your OS version.

### Linux
Just install it from your package manager.

### macOS
If you really use macOS, go fuck yourself. Haha, just kidding. Download the [macOS 64-bit installer](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-5.0.8-macos-x64-installer). And seriously, go fuck yourself.

# Usage
Just drag-and-drop the video you want to turn into a crasher, to the executable file.
Or alternatively, you can specify the path of your input and output file from the command line.
```
DiscordCrasherVideoGenerator.exe C:\path\to\your\mom\input.mp4
DiscordCrasherVideoGenerator.exe C:\path\to\your\mom\input.mp4 C:\path\to\your\mom\output.mp4
```

# How this black magic works?
First of all, this is not something directly related to Discord. Discord's desktop application is developed with Electron, which uses Chromium for rendering engine.
Chromium's video playback feature is really awful. If you download those old crasher videos and play it on your VLC or FFplay, you will notice that the media player will play it just fine.

So, why Chromium fails to play the video? There are 2 mains problems which we are abusing. If you remember, we just splitted our video into two parts. First part's pixel format was YUV420p and it was from 0 second to 0.1 second. The second part's pixel format was YUV444p and it was the rest of the video. Then we combined them into one video.

When Chromium opens the video, it only checks for the first part. In our case, it's pixel format is YUV420p and your CPU and/or GPU can decode it. Then Chromium decides to decode the video on your hardware, since it's enabled by default. But after 0.1 second, the second part starts and forces changing pixel format to YUV444p. Chromium doesn't support changing pixel format or resolution while playing the video. So this is the first killer.

Second problem is, it's really high likely that your hardware doesn't support decoding YUV444p. But since Chromium decided to decode the video on your hardware, Chromium fails to decode the video, which results in freezing the whole Discord desktop application.

# How can I avoid crasher videos?
~~Open Discord, go to User Settings, then navigate to Advanced. You will see an option named "Hardware Acceleration". Disable it and restart your Discord. Now, all of the videos you watch will be decoded by the software. Ta daah! Now you have a talisman for black sorcery!~~

Don't worry, Discord has patched crasher videos. You are safe. At least for now...


# To-Do
- Update to .NET 6.0.
- Update to the only known remaining Discord video crasher method.
