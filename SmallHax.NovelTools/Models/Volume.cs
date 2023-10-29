using System;
using System.Collections.Generic;
using System.Text;

namespace SmallHax.NovelTools.Models
{
    public class Volume
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Chapter> Chapters { get; set; }
    }
}
