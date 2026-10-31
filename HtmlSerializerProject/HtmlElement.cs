using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlSerializerProject
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Attributes { get; set; } = new();
        public List<string> Classes { get; set; } = new();
        public string InnerHtml { get; set; } = "";

        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; } = new();


        public IEnumerable<HtmlElement> Descendants()
        {
            var queue = new Queue<HtmlElement>();
            var result = new List<HtmlElement>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                result.Add(current);

                foreach (var child in current.Children)
                    queue.Enqueue(child);
            }
            return result;
        }
        public List<HtmlElement> Ancestors()
        {
            var result = new List<HtmlElement>();
            var current = this.Parent;

            while (current != null)
            {
                result.Add(current);
                current = current.Parent;
            }

            return result;
        }


    }
}
