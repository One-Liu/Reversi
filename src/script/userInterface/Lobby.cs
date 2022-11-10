using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ReversiFEI.Network
{
    public class Lobby : Control
    {
        private NetworkUtilities networkUtilities;
        
        Task<bool> challengeAccepted;
        
        public override void _Ready()
        {
            networkUtilities = GetNode("/root/NetworkUtilities") as NetworkUtilities;
            if(OS.HasFeature("Server"))
            {
                if(!networkUtilities.IsHosting())
                {
                    if(!networkUtilities.HostLobby())
                    {
                        GD.Print("Failed to start server, shutting down");
                        GetTree().Quit();
                        return;
                    }
                }
            }
            else 
            {
                networkUtilities.JoinGame();
            }
            
            networkUtilities.Connect("MessageReceived",this,nameof(ReceiveMessages));
            networkUtilities.Connect("PlayersOnline",this,nameof(SetOnlinePlayers));
            networkUtilities.Connect("StartMatch",this,nameof(ChallengeAccepted));
            networkUtilities.Connect("CancelMatch",this,nameof(ChallengeDeclined));
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
            
            foreach(string player in networkUtilities.Players.Select(player => player.Value))
            {
                if(player != null && player != networkUtilities.Playername)
                    playerList.AddItem(player);
            }
            
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
                    networkUtilities.OpponentId = player.Key;
            }
            GetNode<Popup>("OnlinePlayersList/OnlinePlayers/Popup").Show();
        }
        
        private async Task ChallengePlayer()
        {
            networkUtilities.SendChallenge(networkUtilities.OpponentId);
            await challengeAccepted;
            if(challengeAccepted.Result)
                GD.Print("Challenge accepted.");
            else
                GD.Print("Challenge declined.");
        }
        
        private void ChallengeAccepted()
        {
            challengeAccepted = Task<bool>.FromResult(true);
        }
        
        private void ChallengeDeclined()
        {
            challengeAccepted = Task<bool>.FromResult(false);
        }
    }
}
