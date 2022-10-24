using Godot;
using System;

public class Entry : Node
{
    public override void _Ready()
    {
        if(OS.HasFeature("Server"))
        {
            GD.Print("Server executable detected, initiating server lobby...");
            GetTree().ChangeScene("res://src/scene/businessLogic/ServerLobby.tscn");
        }
        else 
        {
            GD.Print("Client executable detected, initiating client...");
            GetTree().ChangeScene("res://src/scene/userInterface/ReversiMenu.tscn");
        }
    }
}
