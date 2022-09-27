using Godot;
using System;

public class Entry : Node
{
    public override void _Ready()
    {
    #if GODOT_SERVER
        GetTree().ChangeScene("res://src/scene/businessLogic/ServerLobby.tscn");
    #elif GODOT_32 || GODOT_64
        GetTree().ChangeScene("res://src/scene/userInterface/ReversiMenu.tscn");
    #else
        GD.Print("Failed to autodetect platform, defaulting to client.");
        GetTree().ChangeScene("res://src/scene/userInterface/ReversiMenu.tscn");
    #endif
    }
}
