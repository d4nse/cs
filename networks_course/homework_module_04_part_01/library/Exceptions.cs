
namespace library;

public class ConnectionLost : Exception {
    public ConnectionLost() : base("Lost connection to the server") { }
}

public class ResponseError : Exception {
    public ResponseError(string message) : base(message) { }
}

public class MalformedResponse : ResponseError {
    public MalformedResponse(string[] expected, string[] got) : base($"Expected '{expected}' response, but got '{got}'") { }
}

public class UnknownResponse : ResponseError {
    public UnknownResponse(string response) : base(response) { }
}

public class ClientNotConnected : Exception {
    public ClientNotConnected() : base("Connect() was not called") { }
}


