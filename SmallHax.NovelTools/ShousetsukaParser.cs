using HtmlAgilityPack;
using SmallHax.NovelTools.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SmallHax.NovelTools
{
    public class ShousetsukaParser: INovelParser
    {
        private Regex novelIdRegex = new Regex("^https:\\/\\/ncode\\.syosetu\\.com\\/(?<id>[^\\/]*)(\\/*.)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private Regex chapterIdRegex = new Regex("^.*\\/(?<id>\\d+)\\/$", RegexOptions.Compiled);

        public string GetNovelId(string url)
        {
            var idMatch = novelIdRegex.Match(url);
            if (!idMatch.Success)
            {
                throw new Exception($"Faild to find syosetu index in url: {url}");
            }
            var id = idMatch.Groups["id"].Value;
            return id;
        }

        public Index ParseIndex(Stream stream, string novelId, string url)
        {
            var document = new HtmlDocument();
            document.Load(stream);
            var titleNode = document.DocumentNode.SelectSingleNode(".//*[contains(concat(\" \",normalize-space(@class),\" \"),\" novel_title \")]");
            var result = new Index
            {
                NovelId = novelId,
                Name = titleNode.InnerText,
                Url = url,
                Chapters = new List<Chapter>(),
                Volumes = new List<Volume>()
            };
            var indexNodes = document.DocumentNode.SelectSingleNode(".//*[contains(concat(\" \",normalize-space(@class),\" \"),\" index_box \")]").ChildNodes;
            Volume lastVolume = null;
            foreach ( var indexNode in indexNodes )
            {
                if (indexNode is HtmlTextNode)
                {
                    continue;
                }
                if (indexNode.HasClass("chapter_title"))
                {
                    var volumeId = (lastVolume?.Id ?? 0) + 1;
                    var volume = new Volume
                    {
                        Id = volumeId,
                        Name = indexNode.InnerHtml.Trim(),
                        Chapters = new List<Chapter>()
                    };
                    result.Volumes.Add(volume);
                    lastVolume = volume;
                    continue;
                }
                if (indexNode.HasClass("novel_sublist2"))
                {
                    var anchor = indexNode.SelectSingleNode(".//a");
                    var chapterUrl = anchor.GetAttributeValue("href", "");
                    var idMatch = chapterIdRegex.Match(chapterUrl);
                    if (!idMatch.Success)
                    {
                        throw new Exception($"Cant find id in url {chapterUrl}");
                    }
                    var chapterId = idMatch.Groups["id"].Value;
                    var chatper = new Chapter
                    {
                        ChapterId = chapterId,
                        Url = $"https://ncode.syosetu.com/{novelId}/{chapterId}/",
                        VolumeId = lastVolume?.Id,
                        Name = anchor.InnerHtml.Trim(),
                    };
                    result.Chapters.Add(chatper);
                    lastVolume?.Chapters.Add(chatper);
                    continue;
                }
                throw new Exception("Can't parse node");
            }
            return result;
        }

        public string GetChapterHtml(Stream stream)
        {
            var document = new HtmlDocument();
            document.Load(stream);
            var nodes = document.DocumentNode.SelectSingleNode(".//*[@id=\"novel_honbun\"]").ChildNodes;
            var stringBuilder = new StringBuilder();
            foreach ( var node in nodes )
            {
                if (node is HtmlTextNode)
                {
                    continue;
                }
                if (node.Name == "p" && string.IsNullOrWhiteSpace(node.InnerText))
                {
                    continue;
                }
                stringBuilder.Append(node.OuterHtml);
            }
            return stringBuilder.ToString();
        }
    }
}
