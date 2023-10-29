using HtmlAgilityPack;
using SmallHax.NovelTools.Models;
using System;
using System.IO;

namespace SmallHax.NovelTools
{
    public interface INovelParser
    {
        Index ParseIndex(Stream stream, string novelId, string url);
        string GetChapterHtml(Stream stream);
    }
}
