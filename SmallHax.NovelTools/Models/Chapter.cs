using System;
using System.Collections.Generic;
using System.Text;

namespace SmallHax.NovelTools.Models
{
    public class Chapter
    {
        public string ChapterId { get; set; }
        public int? VolumeId { get; set; }
        public string Name { get; set; }
        //public string Author { get; set; }
        public string Description { get; set; }
        public string Revision { get; set; }
        public string Url { get; set; }
    }
}
