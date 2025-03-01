using System.Net;
using static System.Console;
using System.Text.Json;

class Program {
    static async Task Main(string[] args) {
        if (args.Length == 0) {
            WriteLine("Usage: program <movie_title>");
            return;
        }
        string title = string.Join(' ', args);
        var movie = await SearchMovie(title);
        if (movie != null) {
            WriteLine($"Title: {movie?.Title}");
            WriteLine($"Year: {movie?.Year}");
            WriteLine($"Plot: {movie?.Plot}");
            WriteLine($"IMDB Rating: {movie?.imdbRating}");

            // sending an email requires setting up an smtp server, im not going to do that.
            // i know how its done nevertheless.

        } else {
            WriteLine("Movie not found!");
        }
    }

    static async Task<Movie?> SearchMovie(string title) {
        using var client = new HttpClient();
        string apiKey = "c5ae555"; // again, i dont care. Throwaway api.
        string url = $"http://www.omdbapi.com/?apikey={apiKey}&t={WebUtility.UrlEncode(title)}";
        var response = await client.GetStringAsync(url);
        return JsonSerializer.Deserialize<Movie>(response)!;
    }
}

struct Movie {
    public string Title { get; set; }
    public string Year { get; set; }
    public string Plot { get; set; }
    public string imdbRating { get; set; }
    public string Response { get; set; }
}
