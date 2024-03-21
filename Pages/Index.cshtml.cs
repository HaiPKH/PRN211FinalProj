using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Specialized;
using FFMpegSharp.FFMPEG;
using System.Net;
using System.IO;
using YoutubeExplode;
using FFMpegSharp;
using PRN211FinalProj.Repo;
using PRN211FinalProj.Model;

namespace PRN211FinalProj.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        [BindProperty]
        public string Url { get; set; }

        [BindProperty]
        public string Format { get; set; }

        [BindProperty]
        public string FilePath { get; set; }
        [BindProperty]
        public string DrivePath { get; set; }
        char[] delimiterChars = { '/', '?', '=' };
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        public async Task<IActionResult> OnPost()
        {
            try
            {
                if(HttpContext.Session.GetString("phonenumber") == null)
                {
                    return RedirectToPage("Login");
                }
            }catch(Exception ex)
            {
                return RedirectToPage("Login");
            }

            try
            {
                VIDLContext context = new VIDLContext();
                Console.WriteLine("Begin Download");
                var path = await DownloadYouTubeVideo(Url, Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                var driveid = await DriveRepo.DriveUpload(path, Url);
                if (Url.Contains("youtu.be"))
                {
                    string[] words = Url.Split(delimiterChars);
                    Url = words[3];
                }
                else
                {
                    string[] words = Url.Split(delimiterChars);
                    Url = words[5];
                }
                Console.WriteLine(Url);
                context.VideoInfos.Add(new Model.VideoInfo 
                {User = context.Users.FirstOrDefault(e => e.PhoneNum == HttpContext.Session.GetString("phonenumber")),
                VideoUrl = @"https://www.youtube.com/"+ Url,
                DriveId = driveid});
                Url = @"https://www.youtube.com/embed/"+ Url;
                FilePath = path.ToString();
                DrivePath = @"https://drive.google.com/file/d/"+driveid;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while downloading the videos: " + ex.Message);
            }
            return Page();
        }

        public async Task<string> DownloadYouTubeVideo(string videoUrl, string outputDirectory)
        {
            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(videoUrl);

            // Sanitize the video title to remove invalid characters from the file name
            string sanitizedTitle = string.Join("_", video.Title.Split(Path.GetInvalidFileNameChars()));

            // Get all available muxed streams
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
            var muxedStreams = streamManifest.GetMuxedStreams();
            if (muxedStreams.Any())
            {
                var streamInfo = muxedStreams.First();
                using var httpClient = new HttpClient();
                var stream = await httpClient.GetStreamAsync(streamInfo.Url);
                var datetime = DateTime.Now;
                string outputFilePath = Path.Combine(outputDirectory, $"{sanitizedTitle}.{streamInfo.Container}");
                using var outputStream = System.IO.File.Create(outputFilePath);
                await stream.CopyToAsync(outputStream);

                Console.WriteLine("Download completed!");
                Console.WriteLine($"Video saved as: {outputFilePath}{datetime}");
                return outputFilePath;

            }
            else
            {
                Console.WriteLine($"No suitable video stream found for {video.Title}.");
                return null;
            }
        }
    }
}