using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading.Tasks;

namespace HtmlSerializerProject
{


    public class HtmlSerializer
    {
        public async Task<string> Load(string url)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            var html = await response.Content.ReadAsStringAsync();
            return html;
        }

        private List<string> SplitToParts(string html)
        {
            var cleanHtml = Regex.Replace(html, @"\s+", " ");

            var parts = Regex.Split(cleanHtml, @"(<[^>]*>)")
                             .Select(s => s.Trim())
                             .Where(s => s.Length > 0)
                             .ToList();
            return parts;
        }
        public HtmlElement Serialize(string html)
        {
            var parts = SplitToParts(html);
            var root = new HtmlElement { Name = "root" };
            var current = root;

            foreach (var part in parts)
            {
                if (!part.StartsWith("<"))
                {
                    current.InnerHtml += part;
                    continue;
                }
                var content = part.Substring(1, part.Length - 2).Trim();
                if (content.Length == 0)
                    continue;
                var firstWord = content.Split(' ')[0].ToLower();
                if (firstWord == "/html")
                {
                    return root;
                }
                if (firstWord.StartsWith("/"))
                {
                    var closingName = firstWord.Substring(1);
                    if (HtmlHelper.Instance.HtmlTags.Contains(closingName) && current.Parent != null)
                        current = current.Parent;
                    continue;
                }
                var name = firstWord.TrimEnd('/');
                if (HtmlHelper.Instance.HtmlTags.Contains(name) || HtmlHelper.Instance.HtmlVoidTags.Contains(name))
                {
                    HtmlElement element = new HtmlElement();
                    element.Parent = current;
                    element.Name = name;
                    current.Children.Add(element);


                    var attributesString = content.Substring(firstWord.Length);
                    var attrMatches = Regex.Matches(attributesString,
                        @"([^\s=/]+)\s*=\s*(""[^""]*""|'[^']*')");
                    foreach (Match match in attrMatches)
                    {
                        var attrName = match.Groups[1].Value.ToLower();
                        var attrValue = match.Groups[2].Value.Trim('"', '\'');
                        element.Attributes.Add($"{attrName}=\"{attrValue}\"");
                        if (attrName == "class")
                            element.Classes = attrValue
                                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                .ToList();
                        else if (attrName == "id")
                            element.Id = attrValue;

                    }
                    if (!content.EndsWith("/") && !HtmlHelper.Instance.HtmlVoidTags.Contains(name))
                    {
                        current = element;
                    }

                }
             


            }
            return root;
        }
    }
}
