using System;
using System.Collections.Generic;

namespace PRN211FinalProj.Model
{
    public partial class User
    {
        public User()
        {
            VideoInfos = new HashSet<VideoInfo>();
        }

        public int UserId { get; set; }
        public string PhoneNum { get; set; } = null!;
        public string? Otp { get; set; }
        public bool GuestFlag { get; set; }

        public virtual ICollection<VideoInfo> VideoInfos { get; set; }
    }
}
