using SmallHax.NovelTools;
using SmallHax.NovelTools.Downloader;

if (args.Length == 0)
{
    Console.WriteLine("SmallHax.NovelTools.Downloader");
    Console.WriteLine("");
    Console.WriteLine("SmallHax.NovelTools.NovelDownloader [url]");
    return;
}
var downloadHelper = new DownloadHelper();
var basePath = ".";
var service = new ShousetsukaParser();
var url = args[0];
var id = service.GetNovelId(url);
if (id == null)
{
    Console.WriteLine("Unable to find id in url");
    return;
}

var seriesPath = Path.Combine(basePath, id);
var indexPath = Path.Combine(seriesPath, "index.html");
await downloadHelper.DownloadFromUrl(url, indexPath);
var indexStream = File.OpenRead(indexPath);
var index = service.ParseIndex(indexStream, id, url);

var random = new Random();
await Task.Delay(Convert.ToInt32(10000));
var i = 0;
foreach (var chapter in index.Chapters)
{
    i++;
    Console.WriteLine($"Downloading chapter {i}/{index.Chapters.Count}");
    var chapterPath = Path.Combine(seriesPath, chapter.ChapterId);
    await downloadHelper.DownloadFromUrl(seriesPath, chapterPath);
    await Task.Delay(Convert.ToInt32(5000 + random.NextDouble() * 5000));
}

return;