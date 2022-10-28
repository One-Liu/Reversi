using Godot;
using System;

public class Lobby : Control
{
    
    public override void _Ready()
    {
        var networkUtilities = GetNode("/root/NetworkUtilities") as NetworkUtilities;

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
    }
}

