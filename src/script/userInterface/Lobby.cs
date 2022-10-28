using Godot;
using System;

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
        //var message= GetParent().GetNode<LineEdit>("Panel/ChatLineEdit").Text;
        networkUtilities.SendMessage("hola");
    }
    
    private async void ReceiveMessages()
    {
        await ToSignal(networkUtilities, "MessageReceived");
        GetParent().GetNode<TextEdit>("Panel/ChatBox").Text += networkUtilities.Messages[0];
        ReceiveMessages();
    }
}


