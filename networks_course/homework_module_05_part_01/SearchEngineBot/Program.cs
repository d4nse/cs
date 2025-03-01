
using System.Net;
using System.Text.RegularExpressions;
using System.Text.Json;
using static System.Console;
using HtmlAgilityPack;

class Program {
    private static readonly HttpClient client = new();

    public static async Task Main(string[] args) {
        if (args.Length == 0) {
            WriteLine("Usage: program [img] <query>");
            return;
        }
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                                                           "(KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
        client.DefaultRequestHeaders.Add("Accept",
                                         "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
        client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
        WriteLine(" --- SEARCHING ---");
        string searchQuery = string.Join(' ', args);
        if (args[0].Equals("img")) {
            var engines = new List<ImageSearchEngine> {
                new ImageSearchEngine {
                    Name = "Bing Image Search",
                    QueryUrl = "https://www.bing.com/images/search?q={0}&form=HDRSC2&first=1",
                    DocSelectNodes = "//div[@class='imgpt']//a[@class='iusc']",
                    LinkNode = "m",
                },
            };
            var tasks = new List<Task<string[]>>();
            foreach (var engine in engines) { tasks.Add(engine.Search(searchQuery)); }
            var results = await Task.WhenAll(tasks);
            foreach (var engineResults in results) {
                foreach (var result in engineResults) {
                    if (!string.IsNullOrEmpty(result) && result != "No Link") { WriteLine(result); }
                }
            }
        } else {
            var engines = new List<SearchEngine>() {
                new SearchEngine {
                    Name = "Bing",
                    QueryUrl = "https://www.bing.com/search?q=",
                    DocSelectNodes = "//li[@class='b_algo']",
                    TitleNode = ".//h2/a",
                    LinkNode = null,
                    SnippetNode = ".//p",
                },
            };
            var tasks = new List<Task<Result[]>>();
            foreach (var engine in engines) { tasks.Add(engine.Search(searchQuery)); }
            await Task.WhenAll(tasks);
            foreach (var task in tasks) {
                var results = await task;
                PrintResults(results);
            }
        }
    }

    private static void PrintResults(Result[] results) {
        WriteLine($"---  Results ---");
        foreach (var result in results) {
            result.Print();
            WriteLine("---");
        }
    }

    private class SearchEngine {
        public string Name { get; set; } = string.Empty;
        public string QueryUrl { get; set; } = string.Empty;
        public string DocSelectNodes { get; set; } = string.Empty;
        public string TitleNode { get; set; } = string.Empty;
        public string? LinkNode { get; set; } = null;
        public string SnippetNode { get; set; } = string.Empty;

        public async Task<Result[]> Search(string query) {
            string searchUrl = $"{QueryUrl}{query}";
            var response = await client.GetStringAsync(searchUrl);
            File.WriteAllText($"{Name}.html", response);
            if (response == null) throw new Exception($"{Name} search engine returned null for query '{query}'");
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);
            var results = htmlDoc.DocumentNode.SelectNodes(DocSelectNodes);
            if (results == null) return Array.Empty<Result>();
            var searchResults = new Result[results.Count];
            for (int i = 0; i < results.Count; i++) {
                var titleNode = results[i].SelectSingleNode(TitleNode);
                var snippetNode = results[i].SelectSingleNode(SnippetNode);
                string link;
                if (LinkNode != null) {
                    var linkNode = results[i].SelectSingleNode(LinkNode);
                    link = linkNode?.GetAttributeValue("href", string.Empty) ?? "No Link";
                } else {
                    link = titleNode?.GetAttributeValue("href", string.Empty) ?? "No Link";
                }
                searchResults[i] = new Result {
                    Title = titleNode?.InnerText ?? "No Title",
                    Link = link,
                    Snippet = snippetNode?.InnerText ?? "No Snippet",
                };
            }
            return searchResults;
        }
    }

    private class ImageSearchEngine {
        public string Name { get; set; } = string.Empty;
        public string QueryUrl { get; set; } = string.Empty;
        public string DocSelectNodes { get; set; } = string.Empty;
        public string LinkNode { get; set; } = string.Empty;

        public async Task<string[]> Search(string query) {
            try {
                var searchUrl = string.Format(QueryUrl, Uri.EscapeDataString(query));
                var response = await client.GetStringAsync(searchUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);
                var results = htmlDoc.DocumentNode.SelectNodes(DocSelectNodes) ?? new HtmlNodeCollection(null);
                var imageLinks = new List<string>();
                foreach (var result in results) {
                    var encodedJson = result.GetAttributeValue(LinkNode, string.Empty);
                    if (string.IsNullOrEmpty(encodedJson)) continue;
                    try {
                        // DECODE HTML ENTITIES FIRST
                        var jsonString = WebUtility.HtmlDecode(encodedJson);
                        using var jsonDoc = JsonDocument.Parse(jsonString);
                        var root = jsonDoc.RootElement;
                        if (root.TryGetProperty("murl", out var murl) && murl.ValueKind == JsonValueKind.String) {
                            var url = murl.GetString();
                            if (!string.IsNullOrEmpty(url)) { imageLinks.Add(url); }
                        }
                    } catch (JsonException ex) {
                        WriteLine($"JSON Error in {Name}: {ex.Message}");
                        WriteLine($"Problematic JSON: {encodedJson}");
                    }
                }
                return imageLinks.Count > 0 ? imageLinks.ToArray() : Array.Empty<string>();
            } catch (Exception ex) {
                WriteLine($"Error in {Name}: {ex.Message}");
                return Array.Empty<string>();
            }
        }
    }

    private struct Result {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Snippet { get; set; }

        public void FixSnippet() {
            Snippet = Regex.Replace(Snippet, Regex.Escape("&quot;"), "'");
        }

        public void Print() {
            FixSnippet();
            WriteLine($"Title: \x1b[1m{Title}\x1b[0m");
            WriteLine($"Link:  \x1b[3;38;2;173;216;230m{Link}\x1b[0m");
            WriteLine($"\x1b[38;2;128;128;128m\"{Snippet}\"\x1b[0m");
        }
    }
}
