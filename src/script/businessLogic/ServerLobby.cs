using Godot;
using System;
using ReversiFEI;

public class ServerLobby : Lobby
{
    public override void _Ready()
    {
        if(!IsHosting())
        {
            if(!HostLobby())
            {
                GD.Print("Failed to start server, shutting down");
                GetTree().Quit();
                return;
            }
        }
    }
    
    [Master]
    private bool LogInPlayer(string email, string password)
    {
        if(UserUtilities.LogIn(email, password))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    [Master]
    private bool SignUpPlayer(string email, string username, string password)
    {
        if(UserUtilities.SignUp(email, username, password))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
