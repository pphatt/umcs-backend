using System.Collections.Concurrent;

using Server.Application.Features;

namespace Server.Application.Hubs;

public static class HubConnection
{
    internal static readonly ConcurrentDictionary<string, HashSet<string>> _userConnections = new();

    public static HashSet<string>? GetUserConnections(string userId)
    {
        _userConnections.TryGetValue(userId, out var connections);
        return connections;
    }

    public static void AddUserConnection(string userId, string connectionId)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(connectionId))
        {
            return;
        }

        // two cases here:
        // - if user's connection does not exist, GetOrAdd will add one with the userId as key and an empty HashSet as value.
        // - if user's connection exists, GetOrAdd will get the connections and then can be able to add the currenct connection to it.
        var connections = _userConnections.GetOrAdd(userId, _ => new HashSet<string>());

        lock (connections)
        {
            connections.Add(connectionId);
        }
    }

    public static List<string> GetAllOnlineUsers()
    {
        return _userConnections.Keys.ToList();
    }
}
