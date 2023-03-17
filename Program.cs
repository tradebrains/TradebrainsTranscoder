using System.Text.RegularExpressions;
using System.IO;
using Xabe.FFmpeg;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HelloWorld;

internal class Program
{
    static void Main(string[] args)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"videos");
        string root = Path.GetFileName(path);
        string folder = Path.GetFileName(root);

        

        Console.WriteLine(path);
        Console.WriteLine($"{root} root");
        Console.WriteLine($"{folder} folder");
        DirectoryInfo dir = new DirectoryInfo(root);
        List<string> result = new List<string>();
        //result = ;
        try
        {
            Run(root).GetAwaiter().GetResult();
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Exception was thrown {ex}");
        }
        /**foreach (FileInfo file in dir.GetFiles("*.mp4"))
        {
            string s = file.Name.Replace($"{file.Extension}", "");
            string name = ConvertName(s);
            string filePath = string.Concat(path, name);
            DirectoryInfo NewDir = Directory.CreateDirectory(filePath);
            string newFolder = NewDir.FullName;
        }**/
        Console.WriteLine($"{result}");
        //string[] files = Directory.GetFiles(@"C:\Users\admin\Documents\TradebrainsTranscoder\videos");
        //foreach (string file in files) {    Console.WriteLine($"Filename : {file}");}
        //Console.WriteLine($"{Environment.NewLine}Hello, {name}, on {currentDate:d} at {currentDate:t}!");
        Console.Write($"{Environment.NewLine}Press any key to exit...");
        Console.ReadKey(true);
    }
    private static List<FileInfo> GetFilesToConvert(string directoryPath)
    {
        //Return all files excluding mp4 because I want convert it to mp4
        return new DirectoryInfo(directoryPath).GetFiles().Where(x => x.Extension == ".mp4").ToList();
    }
    private static async Task Run(string root)
    {
        //Set directory where app should look for FFmpeg 
        //FFmpeg.ExecutablesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FFmpeg");
        //Get latest version of FFmpeg. It's great idea if you don't know if you had installed FFmpeg.
        //await FFmpeg.GetLatestVersion();
        Queue<FileInfo> files = new Queue<FileInfo>(GetFilesToConvert(root));
        await Console.Out.WriteLineAsync($"Find {files.Count()} files to convert.");
        await RunConversion(files, root);
    }
    private static async Task RunConversion(Queue<FileInfo> files, string path)
    {
        while (files.TryDequeue(out FileInfo fileToConvert))
        {
            //IMediaInfo mediaInfo = new MediaInfo();
            string s = fileToConvert.Name.Replace($"{fileToConvert.Extension}", "");
            string name = ConvertName(s);
            string filePath = string.Concat(path, $"/{name}");
            DirectoryInfo NewDir = Directory.CreateDirectory(filePath);
            string newPath = Path.Combine(NewDir.FullName, name);
            File.Copy(fileToConvert.FullName, newPath);
            Console.WriteLine(newPath);
            var mediaInfo = await FFmpeg.GetMediaInfo(newPath);
            var videoStream = mediaInfo.VideoStreams.First();
            var audioStream = mediaInfo.AudioStreams.First();
            //File.Copy($"{path}/video.m3u8", newPath);

            Console.WriteLine($"Folder has been created : {NewDir.FullName}");
            SaveByteArrayToFileWithFileStream(TradebrainsTranscoder.Properties.Resources.video, Path.Combine(NewDir.FullName,"video.m3u8"));
            
            await convert(fileToConvert, NewDir, mediaInfo, "1920x1080", "1080_out.m3u8");
            await convert(fileToConvert, NewDir, mediaInfo, "1280x720", "720_out.m3u8");
            await convert(fileToConvert, NewDir, mediaInfo, "720x480", "480_out.m3u8");
            await convert(fileToConvert, NewDir, mediaInfo, "480x360", "360_out.m3u8");
            await Console.Out.WriteLineAsync($"Finished converion file [{fileToConvert.Name}]");
            
            //File.Copy(TradebrainsTranscoder, newPath);
        }  
    }
    public static void SaveByteArrayToFileWithFileStream(byte[] data, string filePath)
    {
        using var stream = File.Create(filePath);
        stream.Write(data, 0, data.Length);

    }
    static async Task convert(FileInfo fileToConvert, DirectoryInfo NewDir, IMediaInfo mediaInfo,string resolution,string outFile)
    {
        Console.WriteLine($"{NewDir.FullName}/{outFile}");
        var outFilePath = Path.Combine(NewDir.FullName, outFile);
        var conversion = FFmpeg.Conversions.New()
                        .AddStream(mediaInfo.Streams)
                        .AddParameter($"-c:a aac -strict experimental -c:v libx264 -s {resolution} -aspect 16:9 -f hls -hls_list_size 1000000 -hls_time 2 {outFilePath}");
        conversion.OnProgress += async (sender, args) =>
        {
            //Show all output from FFmpeg to console
            await Console.Out.WriteLineAsync($"[{args.Duration}/{args.TotalLength}][{args.Percent}%] {fileToConvert.Name} Resolution: {resolution}");
        };
        await conversion.Start();
      
    }
    static string ConvertName(string name)
    {
        string ReName = Regex.Replace(name, "[^a-zA-Z0-9_]+", "_", RegexOptions.Compiled);
        return ReName;
    }
}