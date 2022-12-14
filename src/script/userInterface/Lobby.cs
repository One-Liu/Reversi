using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ReversiFEI.Controller;

namespace ReversiFEI.Network
{
    public class Lobby : Control
    {
        private NetworkUtilities networkUtilities;
        private Controls controls;
        
        private bool challengeStatus; 
        private bool friendRequestStatus;
        private string playerToBeAdded;
        private string playerWantToAdd;
        private string friendToBeDeleted;
        private string friendWantToDelete;

        public override void _Ready()
        {
            controls = GetNode("/root/Controls") as Controls;
            networkUtilities = GetNode("/root/NetworkUtilities") as NetworkUtilities;

            if(!networkUtilities.IsHosting())
            {
                if(OS.HasFeature("Server"))
                {
                    if(!networkUtilities.HostLobby())
                    {
                        GD.Print("Failed to start server, shutting down");
                        GetTree().Quit();
                        return;
                    }
                }
                else 
                {
                    networkUtilities.JoinGame();
                }
            }
            else
            {
                SetOnlinePlayers();
            }
            
            networkUtilities.Connect("MessageReceived",this,nameof(ReceiveMessages));
            networkUtilities.Connect("PlayersOnline",this,nameof(SetOnlinePlayers));
            networkUtilities.Connect("ChallengeReceived",this,nameof(ShowChallengeNotice));
            networkUtilities.Connect("StartMatch",this,nameof(ChallengeAccepted));
            networkUtilities.Connect("CancelMatch",this,nameof(ChallengeDeclined));
            networkUtilities.Connect("ChallengeReplyReceived",this,nameof(ReplyReceived));
            networkUtilities.Connect("FriendRequestReceived",this,nameof(ShowFriendRequestNotice));
            networkUtilities.Connect("FriendRequestReplyReceived",this,nameof(FriendRequestReply));
            networkUtilities.Connect("DeleteFriendUpdate",this,nameof(UpdateFriendsDeleted));

            GetNode<ConfirmationDialog>("ChallengeNotice").GetCloseButton().Connect("pressed",this,nameof(DeclineChallenge));
        }
        
        public override void _Input(InputEvent inputEvent)
        {
            if (inputEvent.IsActionPressed("EnterSendMessage"))
            {
                SendMessage();
            }
        }    
        
        private void SendMessage()
        {
            var message = GetNode<LineEdit>("Panel/ChatLineEdit").Text;
            networkUtilities.SendMessage(message);
            GetNode("Panel").GetNode<LineEdit>("ChatLineEdit").Clear();
        }
        
        
        private void ReceiveMessages()
        {
            GetNode("Panel").GetNode<TextEdit>("ChatBox").Text += networkUtilities.Messages.Last();
            var lineCount = GetNode<TextEdit>("Panel/ChatBox").GetLineCount();
            GetNode<TextEdit>("Panel/ChatBox").CursorSetLine(lineCount);
        }
        
        private void SetOnlinePlayers()
        {
            var playerList = GetNode<ItemList>("OnlinePlayersList/OnlinePlayers");
            playerList.Clear();
            
            var friendsList = GetNode<ItemList>("OnlineFriendsList/OnlineFriends");
            friendsList.Clear();
            networkUtilities.UpdateFriends();
            var friends = networkUtilities.Friends;

            foreach(string player in networkUtilities.Players.Select(player => player.Value))
            {
                if(player != null && player != networkUtilities.Playername)
                {
                    if(friends.Contains(player))
                        friendsList.AddItem(player);
                    else
                        playerList.AddItem(player);
                }
            }

            friendsList.SortItemsByText();
            playerList.SortItemsByText();
        }
        
        private void OnlinePlayersActivated(int index)
        {
            PopupPlayers(index);
        }
        
        private void PopupPlayers(int index)
        {
            string selectedPlayer = GetNode<ItemList>("OnlinePlayersList/OnlinePlayers").GetItemText(index);
            foreach(KeyValuePair<int, string> player in networkUtilities.Players)
            {
                if(player.Value == selectedPlayer)
                {
                    networkUtilities.OpponentId = player.Key;
                    networkUtilities.FriendId = player.Key;
                    playerToBeAdded= player.Value;
                    playerWantToAdd = networkUtilities.Playername;
                }
            }
            GetNode<Popup>("OnlinePlayersList/OnlinePlayers/Popup").Popup_();
        }
        
        private void ShowChallengeNotice()
        {
            var challengeNotice = GetNode<ConfirmationDialog>("ChallengeNotice");
            challengeNotice.PopupExclusive = true;
            challengeNotice.Visible = true;
        }
        
        private void ChallengePlayer()
        {
            networkUtilities.SendChallenge(networkUtilities.OpponentId);
        }
        
        private void ReplyReceived()
        {
            if(challengeStatus)
            {
                controls.GoToMatch();
            }
            else
                GD.Print("Challenge declined.");
        }

        private void AcceptChallenge()
        {
            networkUtilities.ReplyToChallenge(true);
            controls.GoToMatch();
        }
        
        private void DeclineChallenge()
        {
            networkUtilities.ReplyToChallenge(false);
        }
        
        private void ChallengeAccepted()
        {
            challengeStatus = true;
        }
        
        private void ChallengeDeclined()
        {
            challengeStatus = false;
        }
        
        private void AddFriend()
        {
            networkUtilities.SendFriendRequest(networkUtilities.FriendId,playerToBeAdded,playerWantToAdd);
        }
        
        private void ShowFriendRequestNotice()
        {
            var friendRequestNotice = GetNode<ConfirmationDialog>("FriendRequestNotice");
            friendRequestNotice.PopupExclusive = true;
            friendRequestNotice.Visible = true;
        }
        
         private void AcceptFriendRequest()
        {
            friendRequestStatus = true;
            networkUtilities.ReplyToFriendRequest(friendRequestStatus);
            SetOnlinePlayers();
        }

        private void FriendRequestReply()
        {
            SetOnlinePlayers();
        }

        private void DeclineFriendRequest()
        {
             friendRequestStatus = false;
            networkUtilities.ReplyToFriendRequest(friendRequestStatus);
        }
        
        private void DeleteFriend()
        {
            var deleteFriendNotice = GetNode<ConfirmationDialog>("DeleteFriendNotice");
            deleteFriendNotice.PopupExclusive = true;
            deleteFriendNotice.Visible = true;
        }
        
        private void OnlineFriendSelected(int index)
        {
            string selectedPlayer = GetNode<ItemList>("OnlineFriendsList/OnlineFriends").GetItemText(index);
            foreach(KeyValuePair<int, string> player in networkUtilities.Players)
            {
                if(player.Value == selectedPlayer)
                {
                    networkUtilities.OpponentId = player.Key;
                    networkUtilities.FriendId = player.Key;
                    friendToBeDeleted= player.Value;
                    friendWantToDelete = networkUtilities.Playername;
                }
            }
            GetNode<Popup>("OnlineFriendsList/OnlineFriends/Popup").Popup_();
        }
        
        private void AcceptDeleteFriend()
        {
             var deleteFriendNotice = GetNode<ConfirmationDialog>("DeleteFriendNotice");
             networkUtilities.DeleteFriendAccepted(friendToBeDeleted,friendWantToDelete);
             SetOnlinePlayers();
            networkUtilities.GetFriendToDelete(networkUtilities.FriendId);
            deleteFriendNotice.Visible = false;
        }
        
        private void UpdateFriendsDeleted()
        {
            SetOnlinePlayers();
        }
        
        private void FriendRequestAccepted()
        {
            friendRequestStatus = true;
        }
        
        private void FriendRequestDeclined()
        {
            friendRequestStatus = false;
        }
        
        private void KickPlayer()
        {
            networkUtilities.KickPlayer();
        }
    }
}
