using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DiscordCrasherVideoGenerator
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            if (args.Length != 1 && args.Length != 2)
            {
                Console.WriteLine("Usage #1: Drag-and-drop video to DiscordCrasherVideoGenerator   (Output file name: OriginalFileName_crasher.mp4)"); // See comment line below.
                Console.WriteLine("Usage #2: DiscordCrasherVideoGenerator input.mp4                (Output file name: input_crasher.mp4)");
                Console.WriteLine("Usage #3: DiscordCrasherVideoGenerator input.mp4 output.mp4");
                return;
            }

            // If there is only 1 argument, which means the usage is #1 or #2, defaults output file name to OriginalFileName_crasher.mp4.
            var outputFileName = args.Length == 1 ? Path.GetFileNameWithoutExtension(args[0]) + "_crasher.mp4" : args[1];

            // Since concatenating files with concat video filter (-filter_complex) or concat protocol (concat:input.mp4|data.bin) won't work
            //   due to differences in ratio aspects and resolutions (and also corrupting the crasher data), we are going to use concat demuxer (-f concat).
            // Sadly, this way do not support passing file names as parameters. Well, FFmpeg has built in query executor but it won't work on every platform.
            // So, we are going to create a temporary file called "files", write the file we are going to make crasher and our crasher data, use it, then delete it.
            await File.WriteAllTextAsync("files", $"file '{Path.GetFileName(args[0])}'" + "\n" +
                                                   "file data.bin");

            var process = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    CreateNoWindow = true, // For hiding the window that's going to be created.
                    WindowStyle = ProcessWindowStyle.Hidden, // For hiding the window that's going to be created.
                    FileName = "ffmpeg.exe",
                    Arguments = "-loglevel quiet " // -loglevel quiet stop FFmpeg to print stuff to the console. "-c copy"
                              + "-f concat " // Concat demuxer.
                              + "-i files " // The file that contains input file names.
                              + "-c copy " // Equals to "-c:v copy -c:a copy". It copies the codecs from the input files.
                              + "-y " // Overwrites the output file, if it exists.
                              + outputFileName
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            File.Delete("files");
        }
    }
}