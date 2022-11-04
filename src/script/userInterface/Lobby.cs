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
    
    private async void ReceiveMessages()
    {
        await ToSignal(networkUtilities, "MessageReceived");
        GetNode("Panel").GetNode<TextEdit>("ChatBox").Text += networkUtilities.Messages.Last();
        ReceiveMessages();
    }
    
    private async void SetOnlinePlayers()
    {
        await ToSignal(networkUtilities,"PlayersOnline");
        
        
        
        GetNode("OnlinePlayersList").GetNode<ItemList>("OnlinePlayers").AddItem((String)networkUtilities.players.ElementAt(0),null,true);
        SetOnlinePlayers();
        
        
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

   



