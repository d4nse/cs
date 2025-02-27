

using System.Text.Json;

namespace library;

public static class RequestAction {
    public const string SendMessage = "send_message";
    public const string AuthenticateUser = "authenticate_user";
    public const string CreateChat = "create_chat";
    public const string FindChat = "find_chat";
}

public interface IRequest {
    public string Action { get; }
    public string ToJson();
}

public struct SendMessageRequest : IRequest {
    public string Action => RequestAction.SendMessage;
    public string ChatTitle { get; set; }
    public ChatMessage Message { get; set; }
    public string ToJson() => JsonSerializer.Serialize(this);
}

public struct AuthenticateUserRequest : IRequest {
    public string Action => RequestAction.AuthenticateUser;
    public string Username { get; set; }
    public string Hashword { get; set; }
    public string ToJson() => JsonSerializer.Serialize(this);
    public AuthenticateUserRequest(string username, string hashword) {
        Username = username;
        Hashword = hashword;
    }
}

public struct CreateChatRequest : IRequest {
    public string Action => RequestAction.CreateChat;
    public string ChatTitle { get; set; }
    public List<string>? Whitelist { get; set; }
    public string ToJson() => JsonSerializer.Serialize(this);
}

public struct FindChatRequest : IRequest {
    public string Action => RequestAction.FindChat;
    public string ChatTitle { get; set; }
    public string ToJson() => JsonSerializer.Serialize(this);
}

public enum ResponseType {
    Null,
    IncomingMessage,
    SentMessageReceived,
    ChatCreated,
    ChatFound,
    Authenticated,

};

public struct Response {
    public bool Success { get; set; }
    public string Text { get; set; }
    public ResponseType Type { get; set; }
    public object Content { get; set; }
    public string ToJson() => JsonSerializer.Serialize(this);

    public static Response Failure(ResponseType type, string text) {
        var response = new Response() {
            Success = false,
            Type = type,
            Text = text,
        };
        return response;
    }

    public static Response Successful(ResponseType type, string? text = null) {
        var response = new Response() {
            Success = true,
            Type = type,
        };
        if (text != null) response.Text = text;
        return response;
    }

    public ResponseWrapper Wrap(bool terminateClient = false) {
        return new ResponseWrapper(this, terminateClient);
    }

    public T ContentAs<T>() {
        string? jsonString = Content.ToString();
        if (jsonString == null) throw new Exception("Content is null");
        T? value = JsonParser.ParseAs<T>(jsonString);
        if (value == null) throw new Exception($"Could not parse Content as {typeof(T).FullName} ");
        return value;
    }

}

public struct ResponseWrapper {
    public Response Response { get; set; }
    public bool TerminateClient { get; set; }
    public ResponseWrapper(Response response, bool terminateClient) {
        Response = response;
        TerminateClient = terminateClient;
    }
}

public static class JsonParser {
    public static IRequest ParseRequest(string json) {
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
        var action = jsonElement.GetProperty("Action").GetString();
        IRequest request = action switch {
            RequestAction.SendMessage => JsonSerializer.Deserialize<SendMessageRequest>(json),
            RequestAction.AuthenticateUser => JsonSerializer.Deserialize<AuthenticateUserRequest>(json),
            RequestAction.CreateChat => JsonSerializer.Deserialize<CreateChatRequest>(json),
            RequestAction.FindChat => JsonSerializer.Deserialize<FindChatRequest>(json),
            _ => throw new NotSupportedException($"Unknow request type: {action}")
        };
        return request;
    }

    public static Response ParseResponse(string json) {
        return JsonSerializer.Deserialize<Response>(json);
    }

    public static T? ParseAs<T>(string json) {
        return JsonSerializer.Deserialize<T>(json);
    }


}
