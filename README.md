# TradebrainsTranscoder
This project primarily runs on C#. To understand how the code works, Basic understanding of C# syntax is required.

# Objective
To create a simple console application that in-house editors can use to convert content from MP4 format to m3u8 for the purpose of uploading to the server.
  
## Why m3u8?
  In order to provide the users with quality control as well as better load speeds, we took the approach of converting all the video files into a playlist form.
  This approach is commonly seen throughout all major streaming platforms. 
  
## Why this CLI App though?
  Well, for starters, Transcoding videos is expensive. This is but a simple solution that handles transcoding until the requirement for a transcoding server is required. This runs with FFmpeg under the hood.
  
## What is FFmpeg?
  FFmpeg is a culmination of open-source tooling for handling multi-media streams. More about FFmpeg can be found [here](https://ffmpeg.org/).
	
# Implementation
With the primary goal being conversion of videos into multiple qualities, an FFmpeg CLI command is what was initially used. The following commands below can convert a given video into the 4 different qualities and into the .ts format which can be parsed via the video.m3u8 file.

```
ffmpeg -i {file} -c:a aac -strict experimental -c:v libx264 -s 1920x1080 -aspect 16:9 -f hls -hls_list_size 1000000 -hls_time 2 1080_out.m3u8
ffmpeg -i {file} -c:a aac -strict experimental -c:v libx264 -s 1280x720 -aspect 16:9 -f hls -hls_list_size 1000000 -hls_time 2 720_out.m3u8
ffmpeg -i {file} -c:a aac -strict experimental -c:v libx264 -s 720x480 -aspect 16:9 -f hls -hls_list_size 1000000 -hls_time 2 480_out.m3u8
ffmpeg -i {file} -c:a aac -strict experimental -c:v libx264 -s 480x360 -aspect 16:9 -f hls -hls_list_size 1000000 -hls_time 2 360_out.m3u8
```

This project depends on [Xabe.ffmpeg](https://ffmpeg.xabe.net/) and the source .exe files of FFmpeg. XabeFfmpeg is a very useful wrapper for FFmpeg, although the use case of this project limited the use of the wrapper in the most efficient manner, but it still made the implementation much easier.

# Usage
If you're not interested in how it works and just want to use the build files but you're confused how that works, then skip to step 4.
1. To use the project, take a clone of this repo.
``
git clone https://github.com/tradebrains/TradebriansTranscoder/
``
2. Extract 2 exe files from the [FFmpeg Build for windows](https://ffmpeg.org/download.html#build-windows). The exe files are:
 	1.ffmpeg.exe
	2.ffprobe.exe
	The extracted files are placed in the root directory of the project.
3. Generate a build file.
4. Go to your build directory, create a new folder named **videos**. The program has been hardcoded to take videos as the directory, if you intend to change it, the below line needs to be tweaked a little.
``string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"videos");``
5. Copy the files that you intend to convert into the videos directory.
6. Go back to the build exe file, run it and watch the magic happen. Upon completion, you will see a command line message saying conversion completed and you can copy the video files which are now converted and ready to upload.
