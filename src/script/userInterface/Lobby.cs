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
            
            ReceiveMessages();
            SetOnlinePlayers();
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
            var message = GetNode("Panel").GetNode<LineEdit>("ChatLineEdit").Text;
            networkUtilities.SendMessage(message);
            GetNode("Panel").GetNode<LineEdit>("ChatLineEdit").Clear();
        }
        
        private async Task ReceiveMessages()
        {
            while(GetTree().NetworkPeer != null)
            {
                await ToSignal(networkUtilities, "MessageReceived");
                GetNode("Panel").GetNode<TextEdit>("ChatBox").Text += networkUtilities.Messages.Last();
            }
        }
        
        private async Task SetOnlinePlayers()
        {
            while(GetTree().NetworkPeer != null)
            {
            
                var playerList = GetNode("OnlinePlayersList").GetNode<ItemList>("OnlinePlayers");
                playerList.Clear();
                
                foreach(string player in networkUtilities.Players.Select(player => player.Value))
                {
                    if(player != null && player != networkUtilities.Playername)
                        playerList.AddItem(player);
                }
                
                playerList.SortItemsByText();
                
                await ToSignal(networkUtilities,"PlayersOnline");
            }
        }
        
        private void _on_OnlinePlayers_item_selected(int index)
        {
            _on_Popup_about_to_show();
        }
        
        private void _on_Popup_about_to_show()
        {
            GetNode("OnlinePlayersList").GetNode("OnlinePlayers").GetNode<Popup>("Popup").Visible = true;
    
        }
    }
}
