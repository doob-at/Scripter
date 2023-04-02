using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using doob.Scripter.Shared;

namespace doob.Scripter.Module.AngleSharp
{
    public class AngleSharpModule: IScripterModule
    {
        public IHtmlDocument ParseHtml(string html)
        {
            var parser = new HtmlParser();
            return parser.ParseDocument(html);
        }

        public IHtmlDocument ParseHtmlFromFile(string filePath)
        {
            var html = File.ReadAllText(filePath);
            return ParseHtml(html);
        }

    }
}