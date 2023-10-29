using System;
using System.Collections.Generic;
using System.Text;

namespace SmallHax.NovelTools.Models
{
    public class Index
    {
        public string NovelId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public List<Volume> Volumes { get; set; }
        public List<Chapter> Chapters { get; set; }
    }
}
