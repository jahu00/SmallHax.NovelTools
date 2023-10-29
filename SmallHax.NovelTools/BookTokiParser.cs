using HtmlAgilityPack;
using SmallHax.NovelTools.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SmallHax.NovelTools
{
    public class BookTokiParser : INovelParser
    {
        public Index ParseIndex(Stream stream, string novelId, string url)
        {
            var document = new HtmlDocument();
            document.Load(stream);
            var titleNode = document.DocumentNode.SelectSingleNode(".//meta[@name=\"subject\"]");
            var result = new Index
            {
                NovelId = novelId,
                Name = titleNode.GetAttributeValue("content", ""),
                Url = url,
                Chapters = new List<Chapter>(),
                Volumes = new List<Volume>()
            };
            var indexNodes = document.DocumentNode.SelectNodes(".//*[contains(concat(\" \",normalize-space(@class),\" \"),\" serial-list \")]//ul[contains(concat(\" \",normalize-space(@class),\" \"),\" list-body \")]//li[contains(concat(\" \",normalize-space(@class),\" \"),\" list-item \")][@data-index]");
            foreach (var indexNode in indexNodes)
            {
                var anchor = indexNode.SelectSingleNode(".//a");
                var chapterUrl = anchor.GetAttributeValue("href", "");
                var chapterId = indexNode.GetAttributeValue("data-index", "");
                var chatper = new Chapter
                {
                    ChapterId = chapterId,
                    Url = chapterUrl,
                    Name = anchor.InnerHtml.Trim(),
                };
                result.Chapters.Add(chatper);
            }
            result.Chapters = result.Chapters.OrderBy(ch => int.Parse(ch.ChapterId)).ToList();
            return result;
        }

        public string GetChapterHtml(Stream stream)
        {
            var document = new HtmlDocument();
            document.Load(stream);
            var nodes = document.DocumentNode.SelectNodes(".//*[@id=\"novel_content\"]//p");
            var stringBuilder = new StringBuilder();
            foreach (var node in nodes)
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
