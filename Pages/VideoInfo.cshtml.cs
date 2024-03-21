using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN211FinalProj.Model;
using PRN211FinalProj.Repo;

namespace PRN211FinalProj.Pages
{
    public class VideoInfoModel : PageModel
    {
        [BindProperty]
        public List<VideoInfo> videos { get; set; }
        [BindProperty]
        public int VidInfoId { get; set; }
        [BindProperty]
        public string UpdateNote { get; set; }
        public async Task<IActionResult> OnPostDelete(string returnUrl = null)
        {
            Delete();
            return Page();
        }

        public IActionResult Delete()
        {
            try
            {
                VIDLContext context = new VIDLContext();
                VideoInfo videoInfo = context.VideoInfos.FirstOrDefault(x => x.Id == VidInfoId);
                if (videoInfo != null)
                {
                    DriveRepo.DriveDelete(videoInfo.DriveId);
                    context.VideoInfos.Remove(videoInfo);
                    context.SaveChanges();
                }
                videos = context.VideoInfos.ToList();
                return Page();
            }
            catch (Exception ex)
            {
                return Page();
            }
        }
        public IActionResult OnGet()
        {
            VIDLContext context = new VIDLContext();
            videos = context.VideoInfos.ToList();
            return Page();
        }

        public IActionResult OnPost()
        {
            VIDLContext context = new VIDLContext();
            VideoInfo videoInfo = context.VideoInfos.FirstOrDefault(x => x.Id == VidInfoId);
            if (videoInfo != null)
            {
                videoInfo.Notes = UpdateNote;
                context.Entry<VideoInfo>(videoInfo).State = EntityState.Modified;
                context.SaveChanges();
            }
            videos = context.VideoInfos.ToList();
            return Page();
        }
    }
}
