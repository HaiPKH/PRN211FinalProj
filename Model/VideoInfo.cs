using System;
using System.Collections.Generic;

namespace PRN211FinalProj.Model
{
    public partial class VideoInfo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string VideoUrl { get; set; } = null!;
        public string DriveId { get; set; } = null!;
        public string? Notes { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
