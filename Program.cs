using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DiscordCrasherVideoGenerator
{
    internal static class Program
    {
        private static string _inputFilePath;
        private static string _outputFilePath;

        private static string _duration;
        
        private static async Task Main(string[] args)
        {
            if (args.Length != 1 && args.Length != 2)
            {
                Console.WriteLine("Usage #1: Drag-and-drop video to DiscordCrasherVideoGenerator   (Output file name: OriginalFileName_crasher.mp4)"); // See comment line below.
                Console.WriteLine("Usage #2: DiscordCrasherVideoGenerator input.mp4                (Output file name: input_crasher.mp4)");
                Console.WriteLine("Usage #3: DiscordCrasherVideoGenerator input.mp4 output.mp4");
                return;
            }

            _inputFilePath = args[0];

            // Getting the video's duration is important because we will need it on step 2.
            await GetVideoDuration().ConfigureAwait(false);

            // If there is only 1 argument, which means the usage is #1 or #2, defaults output file name to OriginalFileName_crasher.mp4.
            _outputFilePath = args.Length == 1 ? Path.GetFileNameWithoutExtension(_inputFilePath) + "_crasher.mp4" : args[1];

            // First, we are going to take between 0 and 0.1 seconds.
            await RunFfmpegCommand("-loglevel quiet "        // -loglevel quiet stop FFmpeg to print stuff to the console.
                                   + $"-i {_inputFilePath} " // Input file.
                                   + "-ss 0 "                // Start time offset.
                                   + "-t 0.1 "               // Recording duration.
                                   + "FirstPart.mp4");       // Output file.

            // Now we are taking between from 0.1 to the very end.
            await RunFfmpegCommand("-loglevel quiet "
                                   + $"-i {_inputFilePath} "
                                   + "-ss 0.1 "
                                   + $"-t {_duration} "
                                   + "SecondPart.mp4");

            // Then we are changin the pixel format. For further information why it's necessary to use YUV444p, check README.md.
            await RunFfmpegCommand("-loglevel quiet "
                                   + "-i SecondPart.mp4 "
                                   + "-pix_fmt yuv444p " // Changing the pixel format to YUV444p.
                                   + "SecondPart_YUV444p.mp4");

            // Since concatenating files with concat video filter (-filter_complex) or concat protocol (concat:FirstPart.mp4|SecondPart_YUV444p.mp4) won't work
            //   due to differences in ratio aspects, resolutions, and also the pixel format, we are going to use concat demuxer (-f concat).
            // Sadly, this way do not support passing file names as parameters. Well, FFmpeg has built in query executor but it won't work on every platform.
            // So, we are going to create a temporary file called "files", write name of the video and the crasher part to that file, use it, and then delete it.
            await File.WriteAllTextAsync("files", "file FirstPart.mp4\n" +
                                                  "file SecondPart_YUV444p.mp4");

            // Finally, we are creating our crasher video with concat demuxer.
            await RunFfmpegCommand("-loglevel quiet " // -loglevel quiet stop FFmpeg to print stuff to the console.
                                   + "-f concat "     // Concat demuxer.
                                   + "-i files "      // The file that contains input file names.
                                   + "-c copy "       // Equals to "-c:v copy -c:a copy". It copies the codecs from the input files to keep the original codecs.
                                   + "-y "            // Overwrites the output file, if it exists.
                                   + _outputFilePath);

            // After the process, we are cleaning leftover files.
            File.Delete("FirstPart.mp4");
            File.Delete("SecondPart.mp4");
            File.Delete("SecondPart_YUV444p.mp4");
            File.Delete("files");
        }

        private static async Task GetVideoDuration() => await RunFfmpegCommand("-i " + _inputFilePath, true).ConfigureAwait(false);

        private static async Task RunFfmpegCommand(string command, bool getDuration = false)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = "ffmpeg.exe",
                    Arguments = command,

                    UseShellExecute = false,
                    CreateNoWindow = true, // For hiding the window that's going to be created.
                    WindowStyle = ProcessWindowStyle.Hidden // For hiding the window that's going to be created.
                }
            };

            if (getDuration)
            {
                process.StartInfo.RedirectStandardError = true; // Gotta redirect the error output for reading the output of target process.
            }

            process.Start();

            if (getDuration)
            {
                while (!process.StandardError.EndOfStream)
                {
                    var line = await process.StandardError.ReadLineAsync();
                    if (string.IsNullOrEmpty(line) || !line.StartsWith("  Duration: "))
                    {
                        continue;
                    }

                    var commaIndex = line.IndexOf(',');
                    _duration = line[12..commaIndex];
                }
            }

            await process.WaitForExitAsync();
        }
    }
}