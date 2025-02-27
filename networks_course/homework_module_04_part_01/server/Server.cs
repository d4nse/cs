using library;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace server;

public class Server {
    private TcpListener listener;
    private Logger logger;

    public Server(ref Logger logger) {
        listener = new(IPAddress.Any, 6969);
        this.logger = logger;
        ChatroomService.TryCreateChatroom("Hall");
    }

    public async Task Start() {
        listener.Start();
        logger.Info($"Server started on {listener.Server.LocalEndPoint as IPEndPoint}");
        try {
            while (true) {
                var tcpClient = await listener.AcceptTcpClientAsync();
                var client = new ClientHandler(tcpClient);
                logger.Info($"Client connected from {client.RemoteEndPoint}");
                _ = HandleClientAsync(client);
            }
        } catch (Exception ex) {
            logger.Error($"Caught exception: {ex.Message}");
        } finally {
            listener.Stop();
        }
    }

    private async Task HandleClientAsync(ClientHandler client) {
        try {
            while (true) {
                var request = await client.ListenForRequests();
                logger.Info($"Received request from {client.RemoteEndPoint}: {request.ToJson()}");
                ResponseWrapper wrapper = request switch {
                    AuthenticateUserRequest authRequest => HandleAuthenticateUser(authRequest, client),
                    SendMessageRequest messageRequest => await HandleSendMessage(messageRequest),
                    CreateChatRequest createChatRequest => HandleCreateChat(createChatRequest),
                    FindChatRequest findChatRequest => HandleFindChat(findChatRequest, client),
                    _ => Response.Failure(ResponseType.Null, "Unknown request").Wrap(true)
                };
                await client.Respond(wrapper.Response);
                logger.Info($"Responded '{wrapper.Response.ToJson()}' to {client.RemoteEndPoint}");
                if (wrapper.TerminateClient) break;
            }
        } catch (IOException ex) {
            logger.Info($"Client forcibly closed connection: {client.RemoteEndPoint}");
        } catch (Exception ex) {
            logger.Error($"Unexpected exception caught: {ex.GetType()} : {ex.Message} : {ex.StackTrace}");
        } finally {
            logger.Info($"Closing connection with {client.RemoteEndPoint}");
            AuthService.RemoveOnlineUser(client);
            client.Close();
        }
    }

    private ResponseWrapper HandleAuthenticateUser(AuthenticateUserRequest request, ClientHandler client) {
        if (AuthService.TryAuthenticateUser(request.Username, request.Hashword, out UserProfile profile)) {
            AuthService.AddOnlineUser(client, profile);
            return new Response() {
                Success = true,
                Content = profile
            }.Wrap(false);
        }
        return Response.Failure(ResponseType.Authenticated, "Invalid credentials").Wrap(true);
    }

    private async Task<ResponseWrapper> HandleSendMessage(SendMessageRequest request) {
        try {
            var chat = ChatroomService.GetChatroom(request.ChatTitle);
            await ChatroomService.BroadcastInChat(chat, request.Message);
            return new Response() {
                Success = true,
                Type = ResponseType.SentMessageReceived,
            }.Wrap();
        } catch (Exception ex) {
            return Response.Failure(ResponseType.SentMessageReceived, $"Could not post message: {ex.Message}").Wrap();
        }
    }

    private ResponseWrapper HandleCreateChat(CreateChatRequest request) {
        if (ChatroomService.TryCreateChatroom(request.ChatTitle, request.Whitelist)) {
            return Response.Successful(ResponseType.ChatCreated).Wrap();
        }
        return Response.Failure(ResponseType.ChatCreated, "Chat already exists").Wrap();
    }

    private ResponseWrapper HandleFindChat(FindChatRequest request, ClientHandler client) {
        try {
            var profile = AuthService.GetOnlineUserProfile(client);
            ChatroomService.AddToChatroom(request.ChatTitle, profile);
            return Response.Successful(ResponseType.ChatFound).Wrap();
        } catch (Exception ex) {
            return Response.Failure(ResponseType.ChatFound, ex.Message).Wrap();
        }
    }

    // Services
    private static class ChatroomService {
        private static Dictionary<string, Chatroom> Rooms = new();

        public static Chatroom GetChatroom(string title) {
            if (Rooms.TryGetValue(title, out var room)) {
                return room;
            }
            throw new Exception("Chat not found");
        }

        public static async Task BroadcastInChat(Chatroom chat, ChatMessage message) {
            var clients = AuthService.GetClientsByOnlineInChat(chat, except: message.From);
            foreach (var client in clients) {
                var response = new Response {
                    Success = true,
                    Type = ResponseType.IncomingMessage,
                    Content = message,
                };
                await client.WriteAsync(response.ToJson());
            }
        }

        public static bool TryCreateChatroom(string title, List<string>? whitelist = null) {
            var chatroom = new Chatroom() {
                Title = title,
                ChatUsernames = new(),
            };
            if (whitelist != null) {
                chatroom.ChatUsernames.AddRange(whitelist);
                chatroom.IsWhitelisted = true;
            }
            return Rooms.TryAdd(chatroom.Title, chatroom);
        }

        public static void AddToChatroom(string chatTitle, UserProfile profile) {
            var username = profile.Username;
            var room = GetChatroom(chatTitle);
            if (room.IsWhitelisted && !room.ChatUsernames.Exists((name) => name.Equals(username)))
                throw new Exception($"Chat is whitelisted and you're not on it");
            if (!room.ChatUsernames.Exists((name) => name.Equals(username))) {
                room.ChatUsernames.Add(username);
                profile.SubscribedChats.Add(room.Title);
            }
        }

    }

    private static class AuthService {
        private static Dictionary<string, UserProfile> UserProfiles = new();
        private static Dictionary<string, string> UserHashwords = new();
        private static Dictionary<ClientHandler, UserProfile> OnlineUsers = new();

        public static void AddOnlineUser(ClientHandler client, UserProfile profile) {
            OnlineUsers.Add(client, profile);
        }

        public static void RemoveOnlineUser(ClientHandler client) {
            OnlineUsers.Remove(client);
        }

        public static List<ClientHandler> GetClientsByOnlineInChat(Chatroom chat, string? except = null) {
            List<ClientHandler> clients = new();
            var onlineProfiles = OnlineUsers.Values;
            foreach (var username in chat.ChatUsernames) {
                foreach (var profile in onlineProfiles) {
                    if (profile.Username.Equals(except)) continue;
                    if (profile.Username.Equals(username)) clients.Add(GetClientByProfile(profile));
                }
            }
            return clients;
        }

        private static ClientHandler GetClientByProfile(UserProfile profile) {
            foreach (var kvp in OnlineUsers) {
                if (kvp.Value.Equals(profile)) return kvp.Key;
            }
            throw new Exception("User is offline");
        }

        public static UserProfile GetOnlineUserProfile(ClientHandler client) {
            if (OnlineUsers.TryGetValue(client, out var profile)) {
                return profile;
            }
            throw new Exception("Client not authenticated");
        }

        private static bool IsUsernameRegistered(string username) {
            bool hasHashStored = UserHashwords.Keys.Contains(username);
            bool hasProfileStored = UserProfiles.Keys.Contains(username);
            if (hasHashStored != hasProfileStored) throw new Exception($"Username storage error");
            return hasHashStored;
        }

        private static bool IsValidCredentials(string username, string hashword) {
            if (UserHashwords.TryGetValue(username, out string? storedHashword)) {
                if (storedHashword == null)
                    throw new Exception($"Unexpected null value for key '{username}'");
                if (storedHashword.Equals(hashword)) {
                    return true;
                }
            }
            return false;
        }

        private static void RegisterNewUser(string username, string hashword) {
            var profile = new UserProfile() {
                Username = username,
                SubscribedChats = new(),
            };
            UserHashwords.Add(username, hashword);
            UserProfiles.Add(username, profile);
            ChatroomService.AddToChatroom("Hall", profile);
        }

        private static UserProfile GetUserProfile(string username) {
            if (UserProfiles.TryGetValue(username, out UserProfile storedProfile)) {
                return storedProfile;
            }
            throw new Exception("Username not registered");
        }

        public static bool TryAuthenticateUser(string username, string hashword, out UserProfile profile) {
            profile = new();
            if (!IsUsernameRegistered(username)) {
                RegisterNewUser(username, hashword);
            }
            if (!IsValidCredentials(username, hashword)) {
                return false;
            }
            profile = GetUserProfile(username);
            return true;
        }


    }
}


