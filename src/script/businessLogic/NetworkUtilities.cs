//Autoload class, enters the scene tree as soon as the program starts.

using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using ReversiFEI.UserTools;
using System.Threading.Tasks;

namespace ReversiFEI.Network
{
    public class NetworkUtilities : Node 
    {
        private readonly int SERVER_ID = 1;
        private readonly int DEFAULT_PORT = 4321;
        private readonly int MAX_PLAYERS = 30;
        private readonly string ADDRESS = "localhost"; //for local testing
        //private readonly string ADDRESS = "x.x.x.x"; //for live functionality
        
        [Signal]
        delegate void MessageReceived();
        
        [Signal]
        delegate void LoggedIn();
        
        [Signal]
        delegate void PlayersOnline();
        
        [Signal]
        delegate void ChallengeReceived();
        
        [Signal]
        delegate void ChallengeReplyReceived();
        
        [Signal]
        delegate void StartMatch();
        
        [Signal]
        delegate void CancelMatch();
        
        [Signal]
        delegate void FriendRequestReceived();
        
        [Signal]
        delegate void FriendRequestReplyReceived();
        
        [Signal]
        delegate void PiecePlaced(int x, int y, int piece);
        
        public string Playername { get; set;}
        
        public int OpponentId { get; set;}
        
         public int FriendId { get; set;}
        
        private List<string> messages = new List<string>();
        
        public List<string> Messages
        {
            get {return messages;}
            set {messages = value;}
        }
        
        private Dictionary<int, string> players = new Dictionary<int, string>();
        
        public Dictionary<int, string> Players
        {
            get {return players;}
            set {players = value;}
        }
        
        private List<string> friends = new List<string>();
        
        public List<string> Friends
        {
            get {return friends;}
        }
        
        public override void _Ready()
        {
            GetTree().Connect("network_peer_connected", this, nameof(PlayerConnected));
            GetTree().Connect("network_peer_disconnected", this, nameof(PlayerDisconnected));
            GetTree().Connect("connected_to_server", this, nameof(ConnectedToServer));
            GetTree().Connect("connection_failed", this, nameof(ConnectionFailed));
            GetTree().Connect("server_disconnected", this, nameof(ServerDisconnected));
        }
        
        public bool HostLobby()
        {
            bool hosted;
            var peer = new NetworkedMultiplayerENet();
            var result = peer.CreateServer(DEFAULT_PORT, MAX_PLAYERS);
            if (result == 0)
            { 
                GetTree().NetworkPeer = peer;
                GD.Print($"Hosting server at {ADDRESS}:{DEFAULT_PORT}.");
                hosted = true;
            }
            else
            {
                hosted = false;
            }
            return hosted;
        }
        
        public bool IsHosting()
        {
            bool hosting;
            if(GetTree().NetworkPeer != null) 
                hosting = true;
            else
                hosting = false;
            return hosting;
        }
        
        public void JoinGame()
        {
            GD.Print($"Joining lobby with address {ADDRESS}:{DEFAULT_PORT}");

            var clientPeer = new NetworkedMultiplayerENet();
            clientPeer.CreateClient(ADDRESS, DEFAULT_PORT);
            
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
            Rpc(nameof(ReceiveMessage), Playername, message);
        }
        
        [RemoteSync]
        public void ReceiveMessage(string nickname,string message)
        {
            string receivedMessage = nickname+ ": " + message + "\n";
            Messages.Add(receivedMessage);
            EmitSignal(nameof(MessageReceived));
        }
        
        public void SendChallenge(int playerid)
        {
            GD.Print($"Challenging player {playerid}");
            RpcId(playerid, nameof(ReceiveChallenge));
        }
        
       
        [Remote]
        private void ReceiveChallenge()
        {
            OpponentId = GetTree().GetRpcSenderId();
            
            GD.Print($"Challenged by player {OpponentId}");
            
            EmitSignal(nameof(ChallengeReceived));
        }
        
        public void ReplyToChallenge(bool accept)
        {
            GD.Print("Responding succesfully.");
            if(accept)
            {
                RpcId(OpponentId,nameof(ChallengeAccepted));
            }
            else
            {
                RpcId(OpponentId,nameof(ChallengeDeclined));
                OpponentId = -1;
            }
        }

        [Remote]
        private void ChallengeAccepted()
        {
            if(OpponentId == GetTree().GetRpcSenderId())
                EmitSignal(nameof(StartMatch));
            else
            {
                EmitSignal(nameof(CancelMatch));
                OpponentId = -1;
            }
            EmitSignal(nameof(ChallengeReplyReceived));
        }
        
        [Remote]
        private void ChallengeDeclined()
        {
            EmitSignal(nameof(CancelMatch));
            EmitSignal(nameof(ChallengeReplyReceived));
            OpponentId = -1;
        }
        
         public void SendFriendRequest(int playerid)
        {
            GD.Print($"Friend request sent {playerid}");
            RpcId(playerid, nameof(ReceiveFriendRequest));
        }
        
         [Remote]
        private void ReceiveFriendRequest()
        {
            FriendId = GetTree().GetRpcSenderId();
            
            GD.Print($"Friend request sent by player {FriendId}");
            
            EmitSignal(nameof(FriendRequestReceived));
        }
        
        public void ReplyToFriendRequest(bool acceptFriendRequest)
        {
            GD.Print("Responding succesfully.");
            if(acceptFriendRequest)
            {
                RpcId(FriendId,nameof(ChallengeAccepted));
            }
            else
            {
                RpcId(FriendId,nameof(ChallengeDeclined));
                OpponentId = -1;
            }
        }
        
        [Remote]
        private void FriendRequestAccepted()
        {
            if(FriendId == GetTree().GetRpcSenderId())
               GD.Print("Añade amigo");
            else
            {
                //Add friend
                FriendId = -1;
            }
            EmitSignal(nameof(FriendRequestReplyReceived));
        }
        
        [Remote]
        private void FriendRequestDeclined()
        {
            //friend request declined
            EmitSignal(nameof(FriendRequestReplyReceived));
            OpponentId = -1;
        }
        
        public void SendMove(int x, int y, int piece, int opponentId)
        {
            RpcId(opponentId,nameof(ReceiveMove),x,y,piece);
        }
        
        [RemoteSync]
        private void ReceiveMove(int x, int y, int piece)
        {
            EmitSignal(nameof(PiecePlaced),x,y,piece);
        }
        
        private void PlayerConnected(int peerId)
        {
            GD.Print($"player no.{peerId} has connected.");
            if(!OS.HasFeature("Server"))
                Rpc(nameof(RegisterPlayer), Playername);
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
            RpcId(SERVER_ID, nameof(LogInPlayer), email, password);
            GD.Print("Login request sent");
        }
        
        public void SignUp(string email, string username, string password)
        {
            RpcId(SERVER_ID, nameof(SignUpPlayer), email, username, password);
            GD.Print("Signup request sent");
        }

        [RemoteSync]
        private void RegisterPlayer(string playername)
        {
            var peerId = GetTree().GetRpcSenderId();
            bool alreadyRegistered = false;
            
            foreach(string player in players.Select(player => player.Value))
            {
                if(player == playername)
                    alreadyRegistered = true;
            }
            
            if(!alreadyRegistered)
            {
                players.Add(peerId, playername);
                EmitSignal(nameof(PlayersOnline));
                GD.Print($"player {playername} added with peer ID {peerId}");
            }
        }
        
        [Remote]
        private void RemovePlayer(int peerId)
        {
            if (players.ContainsKey(peerId))
            {
                players.Remove(peerId);
                EmitSignal(nameof(PlayersOnline));
                GD.Print($"Player no.{peerId} has disconnected.");
            }
        }
        
        [Master]
        private void LogInPlayer(string email, string password)
        {
            int senderId = GetTree().GetRpcSenderId();
            
            string nickname = UserUtilities.LogIn(email, password);
            
            if(nickname != null)
            {
                RpcId(senderId, nameof(LogInSuccesful), nickname);
                GD.Print($"Player no. {senderId} logged in successfully.");
            }
            else
            {
                RpcId(senderId, nameof(LogInFailed));
                GD.Print($"Player no. {senderId} logged in failed.");
            }
        }
        
        [Master]
        private void SignUpPlayer(string email, string username, string password)
        {
            int senderId = GetTree().GetRpcSenderId();
            if(UserUtilities.SignUp(email, username, password))
            {
                RpcId(senderId, nameof(SignUpSuccesful));
                GD.Print($"Player no. {senderId} signed up successfully.");
            }
            else
            {
                RpcId(senderId, nameof(SignUpFailed));
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
            LeaveGame();
        }
        
        [Puppet]
        private void LogInSuccesful(string nickname)
        {
            GD.Print("Logged in successfully.");
            Playername = nickname;
            LeaveGame();
            EmitSignal(nameof(LoggedIn));
        }
        
        [Puppet]
        private void LogInFailed()
        {
            GD.Print("Log in failed.");
            LeaveGame();
        }

        public void UpdateFriends()
        {
            if(!OS.HasFeature("Server"))
            {
                GD.Print("Updating friends list...");
                friends.Clear();
                friends = UserUtilities.GetFriends(Playername);
            }
        }
    }
}
