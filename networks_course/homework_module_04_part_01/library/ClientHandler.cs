using System.Net;
using System.Net.Sockets;
using System.Text;

namespace library;

public class ClientHandler {
    private TcpClient Client;
    private NetworkStream? _Stream;

    public IPEndPoint? RemoteEndPoint {
        get { return Client.Client.RemoteEndPoint as IPEndPoint; }
    }

    public NetworkStream Stream {
        get { return _Stream ?? throw new ClientNotConnectedError(); }
        set { _Stream = value; }
    }

    public ClientHandler() {
        Client = new();
    }

    public ClientHandler(TcpClient client) {
        Client = client;
        Stream = client.GetStream();
    }

    public async Task ConnectAsync(IPEndPoint instance) {
        if (Client.Connected) {
            throw new ClientConnectedError();
        }
        await Client.ConnectAsync(instance);
        Stream = Client.GetStream();
    }

    public async Task WriteAsync(string data) {
        var bytes = Encoding.UTF8.GetBytes(data);
        await Stream.WriteAsync(bytes);
    }

    public async Task<string> ReadAsync() {
        var buffer = new byte[1024];
        int bytesRead = await Stream.ReadAsync(buffer);
        return Encoding.UTF8.GetString(buffer, 0, bytesRead);
    }

    public async Task<Response> Request(IRequest request) {
        await WriteAsync(request.ToJson());
        var responseJson = await ReadAsync();
        return JsonParser.ParseResponse(responseJson);
    }

    public async Task<IRequest> ListenForRequests() {
        var requestJson = await ReadAsync();
        return JsonParser.ParseRequest(requestJson);
    }

    public async Task<Response> ListenForResponse() {
        var responseJson = await ReadAsync();
        return JsonParser.ParseResponse(responseJson);
    }

    public async Task Respond(Response response) {
        await WriteAsync(response.ToJson());
    }

    public void Close() {
        if (_Stream != null) _Stream.Dispose();
        Client.Close();
    }

}

class ClientConnectedError : Exception { public ClientConnectedError() : base("Client already connected") { } }
class ClientNotConnectedError : Exception { public ClientNotConnectedError() : base("Client not connected") { } }
