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
        private readonly string ADDRESS = "localhost";
        
        [Signal]
        delegate void MessageReceived();
        
        [Signal]
        delegate void MessageReceivedMatch();
        
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
        delegate void DeleteFriendUpdate();
        
        [Signal]
        delegate void PiecePlaced(int x, int y, int piece);
        
        [Signal]
        delegate void OpponentTurnSkipped();
        
        [Signal]
        delegate void MatchEnded();
        
        public string Playername { get; set;}
        
        public int PlayerId{get; set;}

        public int PlayerAvatar { get; set;}
        
        public int OpponentAvatar { get; set;}
        
        public int PlayerSet { get; set;}
        
        public int OpponentSet { get; set;}
        
        public bool IsGuest { get; set;}
        
        public int OpponentId { get; set;}
        
        public int FriendId { get; set;}
        
        public bool MyTurn { get; set;}
        
        public int MyPiece { get; set;}
        
        public int friendSender;
        public string addFriend1;
        public string addFriend2;
        
        private List<string> messages = new List<string>();
        
        public List<string> Messages
        {
            get {return messages;}
            set {messages = value;}
        }
        
        private List<string> messagesMatch = new List<string>();
        
        public List<string> MessagesMatch
        {
            get {return messagesMatch;}
            set {messagesMatch = value;}
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

        public bool SoundEnabled { get; set;}

        private List<string> leaderboard = new List<string>();
        
        public List<string> Leaderboard
        {
            get {return leaderboard;}
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
            players.Clear();
            Rpc(nameof(RemovePlayer), GetTree().GetNetworkUniqueId());
            ((NetworkedMultiplayerENet)GetTree().NetworkPeer).CloseConnection();
            GetTree().NetworkPeer = null;
        }

        public void SendMessage(string message)
        {
            Rpc(nameof(ReceiveMessage), Playername, message);
        }
        
        public void SendMessageMatch(string message)
        {
            var peerId = Playername;
            Rpc(nameof(ReceiveMessageMatch), peerId, message);
        }
        
        [RemoteSync]
        public void ReceiveMessageMatch(string nickname,string message)
        {
            string receivedMessage = nickname+ ": " + message + "\n";
            MessagesMatch.Add(receivedMessage);
            EmitSignal(nameof(MessageReceivedMatch));
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
            RpcId(playerid, nameof(ReceiveSet), PlayerSet);
            RpcId(playerid, nameof(ReceiveAvatar), PlayerAvatar);
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
            bool firstTurnDecide = false;
            int firstTurn = (int) GD.Randi() % 2;
            
            MyPiece = -1;
            int piece = 1;
            
            if(firstTurn == 1)
            {
                firstTurnDecide = true;
                MyPiece = 1;
                piece = -1;
            }
            
            if(accept)
            {
                RpcId(OpponentId, nameof(ReceiveSet), PlayerSet);
                RpcId(OpponentId, nameof(ReceiveAvatar), PlayerAvatar);
                RpcId(OpponentId, nameof(ChallengeAccepted), firstTurnDecide, piece);
                MyTurn = !firstTurnDecide;
            }
            else
            {
                RpcId(OpponentId, nameof(ChallengeDeclined));
                OpponentId = -1;
                OpponentSet = -1;
                OpponentAvatar = -1;
            }
        }
        
        [Remote]
        private void ChallengeAccepted(bool turn, int piece)
        {
            if(OpponentId == GetTree().GetRpcSenderId())
            {
                EmitSignal(nameof(StartMatch));
                MyTurn = turn;
                MyPiece = piece;
            }
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
            OpponentSet = -1;
            OpponentAvatar = -1;
        }
        
        [Remote]
        private void ReceiveSet(int setOfPieces)
        {
            OpponentSet = setOfPieces;
        }
        
        [Remote]
        private void ReceiveAvatar(int avatar)
        {
            OpponentAvatar = avatar;
        }
        
         public void SendFriendRequest(int playerid,string playerNumberOne,string playerNumberTwo)
        {
            GD.Print($"Friend request sent to {playerid}");
            addFriend1=playerNumberOne;
            addFriend2=playerNumberTwo;
            friendSender=playerid;
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
            RpcId(1,nameof(FriendRequestConfirmation),acceptFriendRequest);
        }
        
        [Remote]
        public void FriendRequestConfirmation(bool acceptFriendRequest)
        {
            if(acceptFriendRequest)
            {
                RpcId(FriendId,nameof(FriendRequestAccepted));
            }
            else
            {
                RpcId(FriendId,nameof(FriendRequestDeclined));
                FriendId = -1;
            }
        }
        
        [Remote]
        public void FriendRequestAccepted()
        {
           RpcId(1,nameof(ReceiveFriends),addFriend1,addFriend2);
        }
        
        [Master]
        private void ReceiveFriends(string friend1,string  friend2)
        {
            if(friend1!=null || friend2!=null)
                UserUtilities.AddFriend(friend1, friend2);
        }
        
          [Remote]
        private void FriendRequestDeclined()
        {
            EmitSignal(nameof(FriendRequestReplyReceived));
            OpponentId = -1;
        }
        
        public void GetFriendToDelete(int playerid)
        {
            RpcId(playerid, nameof(FriendToBeDeleted));
        }
        
        [Remote]
        private void FriendToBeDeleted()
        {
            FriendId = GetTree().GetRpcSenderId();
            EmitSignal(nameof(DeleteFriendUpdate));
        }
        
        public void DeleteFriendAccepted(string playerOne,string playerTwo)
        {
                UserUtilities.DeleteFriend(playerOne,playerTwo);
        }

        public void SendMove(int x, int y, int piece)
        {
            RpcId(OpponentId,nameof(ReceiveMove),x,y,piece);
        }
        
        [RemoteSync]
        private void ReceiveMove(int x, int y, int piece)
        {
            EmitSignal(nameof(PiecePlaced),x,y,piece);
        }
        
        public void SkipTurn(bool lastTurnSkipped)
        {
            if(lastTurnSkipped)
            {
                RpcId(OpponentId,nameof(EndMatch));
                EmitSignal(nameof(MatchEnded));
            }
            else
            {
                RpcId(OpponentId,nameof(TurnSkipped));
            }
        }
        
        [Remote]
        private void TurnSkipped()
        {
            EmitSignal(nameof(OpponentTurnSkipped));
        }
        
        [Remote]
        private void EndMatch()
        {
            EmitSignal(nameof(MatchEnded));
        }
        
        public void RequestVictoryRegistration()
        {
            RpcId(1,nameof(RegisterVictory));
        }
        
        [Master]
        private void RegisterVictory()
        {
            string username = players[GetTree().GetRpcSenderId()];
            
            UserUtilities.AddVictory(username);
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
        }
        
        public void SignUp(string email, string username, string password)
        {
            RpcId(SERVER_ID, nameof(SignUpPlayer), email, username, password);
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
            if(Players.ContainsKey(peerId))
            {
                Players.Remove(peerId);
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
                int setOfPieces = UserUtilities.GetPlayerPieceSet(nickname);
                RpcId(senderId, nameof(SetPieces), setOfPieces);
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
            PlayerAvatar = 1;
            LeaveGame();
            EmitSignal(nameof(LoggedIn));
        }
        
        [Puppet]
        private void LogInFailed()
        {
            GD.Print("Log in failed.");
            LeaveGame();
        }
        
        [Puppet]
        private void SetPieces(int setOfPieces)
        {
            PlayerSet = setOfPieces;
        }

        [Puppet]
        private void SetAvatar(int avatar)
        {
            PlayerAvatar = avatar;
        }

        public void UpdateFriends()
        {
            if(!OS.HasFeature("Server"))
            {
                friends.Clear();
                friends = UserUtilities.GetFriends(Playername);
            }
        }
        
        public void ChangeNickname(string newNickname)
        {
            RpcId(1,nameof(ReceiveNicknameChange),Playername,newNickname);
            Playername = newNickname;
        }
        
        [Remote]
        private void ReceiveNicknameChange(string name, string newNickname)
        {
            UserUtilities.ChangeNickname(Playername, newNickname);
        }
        
        public void ChangePassword(string newPassword)
        {
            RpcId(1,nameof(ReceivePasswordChange),Playername,newPassword);
        }
        
        [Remote]
        private void ReceivePasswordChange(string name, string newPassword)
        {
            UserUtilities.ChangePassword(Playername, newPassword);
        }
        
        public void ChangeSetOfPieces(int setOfPieces)
        {
            RpcId(1,nameof(ReceiveSetOfPiecesChange),Playername,setOfPieces);
            PlayerSet = setOfPieces;
        }
        
        [Remote]
        private void ReceiveSetOfPiecesChange(string name, int setOfPieces)
        {
            UserUtilities.ChangeSetOfPieces(name, setOfPieces);
        }
        
        public bool ChangeAvatar(int avatar)
        {
            var avatarUpdated = false;
            
            if(PlayerAvatar != avatar)
            {
                PlayerAvatar = avatar;
                avatarUpdated = true;
                GD.Print("Avatar updated");
            }
            else
            {
                GD.Print("Avatar was not updated");
            }
                
            return avatarUpdated;
        }
        
        public void UpdateLeaderboard()
        {
            if(!OS.HasFeature("Server"))
            {
                GD.Print("Updating leaderboard...");
                leaderboard.Clear();
                leaderboard = UserUtilities.GetLeaderboard();
            }
        }
        
        public void KickPlayer()
        {
            RpcId(OpponentId, nameof(PlayerKicked));
        }
        
        [Puppet]
        private void PlayerKicked()
        {
            GetTree().Quit();
        }
    }
}
