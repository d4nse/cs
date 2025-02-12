using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Console;

namespace quoteserver;

public static class Program {
    private const int max_clients = 2;
    private const int max_quotes_per_user = 5;
    private const int port = 8888;

    private static int cur_clients = 0;
    private static readonly object clients_lock = new();

    private static readonly CancellationTokenSource cts = new();
    private static ConcurrentDictionary<string, int> user_quote_counts = new();

    private static readonly List<string> quotes = [
        "The only way to do great work is to love what you do. — Steve Jobs",
        "Innovation distinguishes between a leader and a follower. — Steve " +
            "Jobs",
        "Your time is limited, so don't waste it living someone else's life. " +
            "— Steve Jobs",
        "Stay hungry, stay foolish. — Steve Jobs",
        "The journey is the reward. — Steve Jobs"
    ];

    public static async Task Main() {
        var listener = new TcpListener(IPAddress.Any, port);
        Console.CancelKeyPress += (sender, e) => {
            e.Cancel = true;
            Logger.Log("Initiating graceful shutdown...");
            cts.Cancel();
        };
        listener.Start();
        Logger.Log($"Server started on {IPAddress.Any}:{port}");
        try {
            while (!cts.IsCancellationRequested) {
                var client = await listener.AcceptTcpClientAsync(cts.Token);
                _ = handle_client_async(client, cts.Token);
            }
        } catch (OperationCanceledException) {
            Logger.Log("Operation canceled");
        } catch (SocketException ex) {
            if (ex.ErrorCode == 125) {
                Logger.Log("Socket operation canceled");
            } else {
                Logger.Log($"Unknown socket exception caught: {ex.Message}");
            }
        } catch (Exception ex) {
            Logger.Log($"Exception occured `{ex.GetType()}`: {ex.Message}");
        } finally {
            listener.Stop();
            Logger.Log("Server shutdown.");
        }
    }

    private static async Task handle_client_async(TcpClient client,
                                                  CancellationToken tok) {
        var client_ep = client.Client.RemoteEndPoint!.ToString() ?? "Unkown";
        Logger.LogConn(client_ep);

        // prepare client io
        using var stream = client.GetStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        using var writer = new StreamWriter(stream, Encoding.UTF8);
        writer.AutoFlush = true;

        bool is_server_full = cur_clients >= max_clients;
        bool is_auth_successful = false;
        bool can_serve = false;
        string? username = string.Empty;
        string? request = string.Empty;
        string? client_id = client_ep;
        try {

            // always send an initial message
            if (is_server_full) {
                await serve(writer, client_id, "SERVER_FULL");
            } else {
                await serve(writer, client_id, "ACCESS_GRANTED");
                lock (clients_lock) cur_clients++;
                Logger.Log($"Number of active clients: {cur_clients}");

                // anticipate auth request
                request = (await reader.ReadLineAsync(tok));
                (username, string? passwd) = process_auth_request(request);
                if (!AuthenticationService.validate(username, passwd)) {
                    await serve(writer, client_id, "AUTH_FAILURE");
                } else {
                    await serve(writer, client_id, "AUTH_SUCCESS");
                    is_auth_successful = true;
                    Logger.LogAuth(client_ep, username!);
                    client_id = $"{username} ({client_ep})";
                }
            }

            // serve
            can_serve = !is_server_full && is_auth_successful &&
                        client.Connected && !tok.IsCancellationRequested;
            while (can_serve) {
                request = (await reader.ReadLineAsync(tok));
                Logger.Log($"{client_id} made `{request}` request");
                if (request == "exit")
                    break;
                if (request == "ping") {
                    await serve(writer, client_id, "pong");
                } else if (request == "quote") {
                    if (!has_user_reached_limit(username!)) {
                        await serve(writer, client_id, get_rnd_quote());
                        incr_user_limit(username!);
                    } else {
                        await serve(writer, client_id, "LIMIT_REACHED");
                        break;
                    }
                } else {
                    await serve(writer, client_id, "INVALID_REQUEST");
                }
                can_serve = client.Connected && !tok.IsCancellationRequested;
            }
        } catch (Exception ex) {
            Logger.Log(
                $"Caught exception during communicaion with client {client_id}: {ex.Message}");
        } finally {
            Logger.LogDisconn(client_id);
            client.Dispose();
            if (!is_server_full)
                lock (clients_lock) cur_clients--;
        }
    }

    private static bool has_user_reached_limit(string username) {
        int value = user_quote_counts.GetOrAdd(username, 0);
        return value >= max_quotes_per_user;
    }

    private static void incr_user_limit(string username) {
        user_quote_counts.AddOrUpdate(username, 1, (k, cur) => cur + 1);
    }

    private static (string? username, string? password)
        process_auth_request(string? auth_request) {
        if (string.IsNullOrEmpty(auth_request))
            return (null, null);
        if (!auth_request.Contains(':'))
            return (null, null);
        var parts = auth_request.Split(':')!;
        if (parts.Length > 2)
            return (null, null);
        return (parts[0], parts[1]);
    }

    private static async Task serve(StreamWriter client_writer,
                                    string client_ep, string response) {
        Logger.Log($"Serving `{response}` to {client_ep}");
        await client_writer.WriteLineAsync(response);
    }

    private static string get_rnd_quote() {
        var rnd = new Random();
        return quotes[rnd.Next(quotes.Count)];
    }
}

public static class AuthenticationService {
    private static readonly Dictionary<string, string> valid_users = new() {
        { "admin", "admin" },
        { "user", "user" },
        { "test", "test" },
    };

    public static bool validate(string? username, string? password) {
        return username != null && password != null &&
               valid_users.TryGetValue(username, out var validPass) &&
               validPass == password;
    }
}

public static class Logger {
    private static readonly object loglock = new();
    public static void Log(DateTime time, object message) {
        lock (loglock) {
            WriteLine($"{time:HH:mm:ss} {message}");
        }
    }
    public static void Log(object message) {
        Log(DateTime.UtcNow, message);
    }
    public static void LogConn(string endpoint) {
        Log($"{endpoint} connected");
    }
    public static void LogDisconn(string endpoint) {
        Log($"{endpoint} disconnected");
    }
    public static void LogAuth(string endpoint, string username) {
        Log($"{endpoint} authenticated as `{username}`");
    }
}
