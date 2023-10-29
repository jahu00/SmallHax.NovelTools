using SmallHax.NovelTools;
using SmallHax.NovelTools.Models;
using System.Text;

var srcPath = args[0];
var novelId = args[1];
var dstPath = args[2];

var parser = new BookTokiParser(); //new ShousetsukaParser();
var indexPath = Path.Combine(srcPath, "index.html");
var indexStream = File.OpenRead(indexPath);
var index = parser.ParseIndex(indexStream, novelId, "");

var volumes = index.Volumes;
var chapterGrouping = 10;


if (index.Volumes.Count == 0)
{
    volumes = new List<Volume>();
    Volume lastVolume = null;
    var i = 1;
    foreach (var chapter in index.Chapters)
    {
        if (lastVolume == null || lastVolume.Chapters.Count == chapterGrouping)
        {
            lastVolume = new Volume
            {
                Chapters = new List<Chapter>()
            };
            volumes.Add(lastVolume);
        }
        lastVolume.Chapters.Add(chapter); 
    }
    volumes.ForEach(volume => { volume.Name = $"{i} - {i + volume.Chapters.Count - 1}"; i += volume.Chapters.Count; });
}

foreach(var volume in volumes)
{
    var volumePath = Path.Combine(dstPath, volume.Name + ".html");
    var stringBuilder = new StringBuilder();
    var title = index.Name + " - " + volume.Name;
    stringBuilder.AppendLine($"<head><title>{title}</title><meta charset=\"UTF-8\"></head>");
    stringBuilder.AppendLine("<body>");
    stringBuilder.AppendLine($"<h1>{title}</h1>");
    stringBuilder.AppendLine($"<div id=\"TableOfContents\"><h2>Table of contents</h2>");
    foreach (var chapter in volume.Chapters)
    {
        stringBuilder.AppendLine($"<a href=\"#{chapter.ChapterId}\">{chapter.Name}</a><br/>");
    }
    stringBuilder.AppendLine($"</div>");
    foreach (var chapter in volume.Chapters)
    {
        var chapterPath = Path.Combine(srcPath, chapter.ChapterId + ".html");
        using var chapterStream = File.OpenRead(chapterPath);
        var chapterContent = parser.GetChapterHtml(chapterStream);
        stringBuilder.AppendLine($"<div id=\"{chapter.ChapterId}\"><h2>{chapter.Name}</h2>");
        stringBuilder.AppendLine(chapterContent);
        stringBuilder.AppendLine($"</div>");
    }
    stringBuilder.AppendLine("</body>");
    File.WriteAllText(volumePath, stringBuilder.ToString(), Encoding.UTF8);
}