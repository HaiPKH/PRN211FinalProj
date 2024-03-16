using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using YoutubeExplode;


namespace PRN211FinalProj.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        [BindProperty]
        public string Url { get; set; }

        [BindProperty]
        public string Format { get; set; }
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                //HttpContext.Session.SetString("EmbedUrl", EmbedUrl);
                Console.WriteLine("Begin Download");
                await DownloadYouTubeVideo(Url, Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                Url = Url.Replace("watch?v=", "embed/");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while downloading the videos: " + ex.Message);
            }
            return Page();
        }

        public async Task DownloadYouTubeVideo(string videoUrl, string outputDirectory)
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
                string filePath = outputFilePath;
                //FileInfo file = new FileInfo(filePath);
                //if (file.Exists)
                //{
                //    Response.Clear();
                //    Response.Headers.Clear();
                //    Response.Headers.Add("Content-Disposition", "attachment; filename=" + file.Name);
                //    Response.Headers.Add("Content-Length", file.Length.ToString());
                //    Response.ContentType = "text/plain";
                //    await Response.SendFileAsync(file.FullName);
                //    Response.CompleteAsync();
                //}
            }
            else
            {
                Console.WriteLine($"No suitable video stream found for {video.Title}.");
            }
        }
    }
}