using Godot;
using System;
using System.Collections.Generic;

public class Lobby : Node
{
    private readonly int DEFAULT_PORT = 78910;
    private readonly int MAX_PLAYERS = 30;
    private readonly string ADDRESS = "localhost"; //for local testing
    //private readonly string ADDRESS = "x.x.x.x"; //for live functionality
    
    public int playerId { get ; set ;}
    private Dictionary<int, int> players = new Dictionary<int, int>();
    
    public override void _Ready()
    {
        GetTree().Connect("network_peer_connected", this, nameof(PlayerConnected));
        GetTree().Connect("network_peer_disconnected", this, nameof(PlayerConnected));
        GetTree().Connect("connected_to_server", this, nameof(ConnectedToServer));
        GetTree().Connect("connection_failed", this, nameof(ConnectionFailed));
        GetTree().Connect("server_disconnected", this, nameof(ServerDisconnected));
    }

    public bool HostLobby()
    {
        var peer = new NetworkedMultiplayerENet();
        var result = peer.CreateServer(DEFAULT_PORT, MAX_PLAYERS);
        if (result == 0)
        { 
            GetTree().NetworkPeer = peer;
            GD.Print("Hosting server.");
            return true;
        }
        else
        {
            return false;
        }
    }

    
    public bool IsHosting()
    {
        if(GetTree().NetworkPeer != null) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public void JoinGame()
    {
        GD.Print($"Joining lobby with address {ADDRESS}");

        var clientPeer = new NetworkedMultiplayerENet();
        var result = clientPeer.CreateClient(ADDRESS, DEFAULT_PORT);

        GetTree().NetworkPeer = clientPeer;
    }
    
    public void LeaveGame()
    {
        GD.Print("Leaving current game");

        players.Clear();

        GetNode(GetTree().GetNetworkUniqueId().ToString()).QueueFree();

        Rpc(nameof(RemovePlayer), GetTree().GetNetworkUniqueId());

        ((NetworkedMultiplayerENet)GetTree().NetworkPeer).CloseConnection();
        GetTree().NetworkPeer = null;
    }

    private void PlayerConnected(int peerId)
    {
        GD.Print($"player no.{playerId} has connected.");
        RpcId(peerId, nameof(RegisterPlayer), playerId);
    }

    private void PlayerDisconnected(int peerId)
    {
        GD.Print("Player disconnected");
        RemovePlayer(peerId);
    }

    private void ConnectedToServer()
    {
        GD.Print("Successfully connected to the server");
    }

    private void ConnectionFailed()
    {
        GetTree().NetworkPeer = null;

        GD.Print("Failed to connect.");
    }

    private void ServerDisconnected()
    {
        GD.Print($"Disconnected from the server");
    }

    [Remote]
    private void RegisterPlayer(int playerId)
    {
        var peerId = GetTree().GetRpcSenderId();

        players.Add(peerId, playerId);

        GD.Print($"player no. {playerId} added with peer ID {peerId}");
    }

    [Remote]
    private void RemovePlayer(int peerId)
        {

        if (players.ContainsKey(peerId))
        {
            players.Remove(peerId);
        }
    }
}
