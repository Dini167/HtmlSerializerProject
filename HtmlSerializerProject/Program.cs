using HtmlSerializerProject;

var serializer = new HtmlSerializer();

var html = await serializer.Load("https://checkin-client-tml7.onrender.com/");
var dom = serializer.Serialize(html);

var selector = Selector.FromQueryString("body");
var result = dom.FindElements(selector);

Console.WriteLine($"Found {result.Count} elements");

foreach (var element in result)
{
    Console.WriteLine($"<{element.Name}> id={element.Id ?? "null"} classes={string.Join(",", element.Classes)}");
}
