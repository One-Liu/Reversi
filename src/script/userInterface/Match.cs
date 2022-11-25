using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ReversiFEI.Controller;

namespace ReversiFEI.Network
{
    public class Match : Control
    {
        private NetworkUtilities networkUtilities;
        private Controls controls;
        
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
            
            networkUtilities.Connect("MessageReceivedMatch",this,nameof(ReceiveMessages));
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
            networkUtilities.SendMessageMatch(message);
            GetNode<LineEdit>("Panel/ChatLineEdit").Clear();
      }
      private void ReceiveMessages()
      {
         GetNode("Panel").GetNode<TextEdit>("Chat").Text += networkUtilities.MessagesMatch.Last();
      }
        
   }
}


