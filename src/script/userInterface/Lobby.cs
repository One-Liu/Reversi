using Godot;
using System;
using System.Linq;

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
        
    }
    
    private void SendMessage()
    {
        var message = GetNode("Panel").GetNode<LineEdit>("ChatLineEdit").Text;
        networkUtilities.SendMessage(message);
    }
    
    private async void ReceiveMessages()
    {
        await ToSignal(networkUtilities, "MessageReceived");
        GetNode("Panel").GetNode<TextEdit>("ChatBox").Text += networkUtilities.Messages.Last();
        ReceiveMessages();
    }
}
