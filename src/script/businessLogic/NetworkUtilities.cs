//Autoload class, enters the scene tree as soon as the program starts.

using Godot;
using System;
using System.Collections.Generic;
using ReversiFEI;
using System.Threading;

public class NetworkUtilities : Node 
{
    private readonly int DEFAULT_PORT = 4321;
    private readonly int MAX_PLAYERS = 30;
    private readonly string ADDRESS = "localhost"; //for local testing
    //private static readonly string ADDRESS = "x.x.x.x"; //for live functionality
    
    [Signal]
    delegate void MessageReceived();
    
    public string PlayerName { get; set;}
    
    public List<string> Messages = new List<string>();
    
    private Dictionary<int, string> players = new Dictionary<int, string>();
    
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
            GD.Print($"Hosting server at {ADDRESS}:{DEFAULT_PORT}.");
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
        GD.Print($"Joining lobby with address {ADDRESS}:{DEFAULT_PORT}");

        var clientPeer = new NetworkedMultiplayerENet();
        var result = clientPeer.CreateClient(ADDRESS, DEFAULT_PORT);

        GetTree().NetworkPeer = clientPeer;
    }
    
    public void LeaveGame()
    {
        GD.Print("Leaving current game");

        players.Clear();

        Rpc(nameof(RemovePlayer), GetTree().GetNetworkUniqueId());

        ((NetworkedMultiplayerENet)GetTree().NetworkPeer).CloseConnection();
        GetTree().NetworkPeer = null;
    }

    public void SendMessage(string message)
    {
        var peerId=GetTree().GetNetworkUniqueId();
        Rpc("ReceiveMessage",peerId,message);
    }
    
    [RemoteSync]
     public void ReceiveMessage(int peerId,string message)
    {
        string receivedMessage = peerId+": "+message;
        Messages.Add(receivedMessage);
        EmitSignal(nameof(MessageReceived));
    }
    
    private void PlayerConnected(int peerId) //fix duplicate key issue
    {
        GD.Print($"player no.{peerId} has connected.");
        Rpc(nameof(RegisterPlayer),PlayerName);
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
    
    public void LogIn(string email, string password)
    {
        {
            RpcId(1, "LogInPlayer", email, password); // peer id 1 is always the server, RpcId sends Rpc only to specified Id.
            GD.Print("Login request sent");
        }
    }
     
    public void SignUp(string email, string username, string password)
    {
        {
            RpcId(1, "SignUpPlayer", email, username, password);
            GD.Print("Signup request sent");
        }
    }

    [Remote]
    private void RegisterPlayer(string playerName)
    {
        var peerId = GetTree().GetRpcSenderId(); //key duplicated issue
        players.Add(peerId, playerName);
        
        GD.Print($"player {playerName} added with peer ID {peerId}");
    }

    [Remote]
    private void RemovePlayer(int peerId)
        {

        if (players.ContainsKey(peerId))
        {
            players.Remove(peerId);
            GD.Print($"Player no. {peerId} has disconnected.");
        }
    }
    
    [Master]
    private void LogInPlayer(string email, string password)
    {
        int senderId = GetTree().GetRpcSenderId();
        if(UserUtilities.LogIn(email, password))
        {
            RpcId(senderId, "LogInSuccesful");
            GD.Print($"Player no. {senderId} logged in successfully.");
        }
        else
        {
            RpcId(senderId, "LogInFailed");
            GD.Print($"Player no. {senderId} logged in failed.");
        }
    }
    
    [Master]
    private void SignUpPlayer(string email, string username, string password)
    {
        int senderId = GetTree().GetRpcSenderId();
        if(UserUtilities.SignUp(email, username, password))
        {
            RpcId(senderId, "SignUpSuccesful");
            GD.Print($"Player no. {senderId} signed up successfully.");
        }
        else
        {
            RpcId(senderId, "SignUpFailed");
            GD.Print($"Player no. {senderId} sign up failed.");
        }
    }
    
    [Puppet]
    private void SignUpSuccesful()
    {
        GD.Print("Signed up successfully.");
        LeaveGame();
    }
    
    [Puppet]
    private void SignUpFailed()
    {
        GD.Print("Sign up failed.");
        GetTree().NetworkPeer = null;
    }
    
    [Puppet]
    private void LogInSuccesful()
    {
        GD.Print("Logged in successfully.");
    }
    
    [Puppet]
    private void LogInFailed()
    {
        GD.Print("Log in failed.");
        GetTree().NetworkPeer = null;
    }
}



