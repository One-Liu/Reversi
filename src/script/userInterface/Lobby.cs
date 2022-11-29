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
        static string playerToBeAdded;
        static string playerWantToAdd;
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
          
            GetNode<ConfirmationDialog>("ChallengeNotice").GetCloseButton().Connect("pressed",this,nameof(DeclineChallenge));
        }
        
        public override void _Input(InputEvent inputEvent)
        {
            if (inputEvent.IsActionPressed("lobby_SendMessage"))
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
        }
        
        private void SetOnlinePlayers()
        {
            var playerList = GetNode<ItemList>("OnlinePlayersList/OnlinePlayers");
            playerList.Clear();
            
            var friendsList = GetNode<ItemList>("OnlineFriendsList/OnlineFriends");
            var friends = networkUtilities.Friends;
            friendsList.Clear();

            networkUtilities.UpdateFriends();
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
        
        private void _on_OnlinePlayers_item_activated(int index)
        {
            
            _on_Popup_about_to_show(index);
        }
        
        private void _on_Popup_about_to_show(int index)
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
                    GD.Print(playerWantToAdd+"y abtToShow "+playerToBeAdded);
                }
                    
            }
           //AnchorLeft=GetNode<ItemList>("OnlinePlayersList/OnlinePlayers").GetItemAtPosition(index).AnchorLeft();
            GetNode<Popup>("OnlinePlayersList/OnlinePlayers/Popup").Show();
           // AnchorLeft=911;
            //AnchorTop=774;
            //AnchorRight=911;
           // AnchorBottom=774;
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
                GD.Print("Challenge accepted.");
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
            networkUtilities.SendFriendRequest(networkUtilities.FriendId);
        }
        
        private void ShowFriendRequestNotice()
        {
            var friendRequestNotice = GetNode<ConfirmationDialog>("FriendRequestNotice");
            friendRequestNotice.PopupExclusive = true;
            friendRequestNotice.Visible = true;
        }
        
        private void AddFriend()
        {
            networkUtilities.SendFriendRequest(networkUtilities.FriendId);
        }
        
        private void FriendRequestReplyReceived()
        {
            if(friendRequestStatus)
            {

                networkUtilities.FriendRequestAccepted(playerWantToAdd,playerToBeAdded);
            }
            else
                GD.Print("Friend request declined.");
        }
        
           private void AcceptFriendRequest()
        {
            networkUtilities.ReplyToFriendRequest(true,playerWantToAdd,playerToBeAdded);
        }
        
        private void DeclineFriendRequest()
        {
            networkUtilities.ReplyToFriendRequest(false,null,null);
        }
        
        private void FriendRequestAccepted()
        {
            friendRequestStatus = true;
        }
        
        private void FriendRequestDeclined()
        {
            friendRequestStatus = false;
        }
    }
}






